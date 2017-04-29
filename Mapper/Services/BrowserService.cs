using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using SHDocVw;

namespace Mapper.Services
{
    public class BrowserService
    {
        public event EventHandler<string> OneDrivePathFiguredOut;
        private readonly DispatcherTimer _ssoTimer;
        private readonly InternetExplorer ie;
        public BrowserService()
        {
            _ssoTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(6)};
            _ssoTimer.Tick += ssoTimerTick;
            ie = (SHDocVw.InternetExplorer)Activator.CreateInstance(Type.GetTypeFromProgID("InternetExplorer.Application"));
            ie.BeforeNavigate2 += Ie_BeforeNavigate2;
            ie.NavigateComplete2 += Ie_NavigateComplete2;            
        }

        public void Cleanup()
        {
            ie.Quit();
        }

        public void Navigate(string url)
        {
            ie.Navigate(url);  
        }

        private void Ie_NavigateComplete2(object pDisp, ref object URL)
        {
            Task.Delay(500);
            dynamic doc = ie.Document;
            dynamic htmlText;
            try
            {
                htmlText = doc.documentElement.InnerHtml;
            }
            catch
            {
                return;
            }

            //Trying to set "Keep Me Signed In" checkbox to checked
            try
            {
                doc.GetElementById("cred_keep_me_signed_in_checkbox").SetAttribute("checked", true);
            }
            catch
            {
                //Ignore the error which raises if checkbox was not found on a website
            }


            string value = Convert.ToString(htmlText);
            var indexOfLink = value.IndexOf("MySite.aspx?MySiteRedirect=AllDocuments", StringComparison.Ordinal);
            if (indexOfLink > 0)
            {
                var index = value.LastIndexOf("\"", indexOfLink, StringComparison.Ordinal) + 1;
                var tempUrl = value.Substring(index, indexOfLink - index) + "MySite.aspx?MySiteRedirect=AllDocuments";
                Navigate(tempUrl);
                return;
            }
            var browserUrl = URL as string;
            if (browserUrl == null) return;
            if (value.Contains("/start.aspx#/Documents/Forms/All.aspx"))
            {
                _ssoTimer?.Stop();
                browserUrl = browserUrl.Replace("_layouts/15/onedrive.aspx", "Documents");
                //We got the URL and are good to go
                OneDrivePathFiguredOut?.Invoke(this, browserUrl);
                return;
            }
            if (browserUrl.Contains("https://login.microsoftonline.com/"))
            {
                //Wait 6 seconds so that AD FS has enough time to do SSO
                _ssoTimer?.Start();
            }
        }

        private void Ie_BeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
        {
            var navigatingToUrl = URL as string;
            if (navigatingToUrl == null) return;
            if (!navigatingToUrl.Contains("login.microsoftonline.com")) return;
            if (!navigatingToUrl.Contains("/oauth2/authorize")) return;
            if (navigatingToUrl.Contains("msafed=0")) return;
            //Don't offer login with Microsoft Account
            var url = navigatingToUrl.Replace("?", "?msafed=0&");
            Navigate(url);
        }

        private void ssoTimerTick(object sender, EventArgs e)
        {
            ie.Visible = true;
        }
    }
}
