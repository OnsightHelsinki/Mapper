using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Mapper.Services
{
    public class BrowserService
    {
        private readonly WebBrowser _browser;
        public event EventHandler<string> OneDrivePathFiguredOut;
        public event EventHandler LoginInputRequired;
        private readonly DispatcherTimer _timer;
        public BrowserService(WebBrowser browser)
        {
            _browser = browser;
            HideScriptErrors(_browser, true);
            _browser.Navigated += Browser_Navigated;
            _browser.Navigating += Browser_Navigating;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(6) };
            _timer.Tick += _timer_Tick;
        }

        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var navigatingToUrl = e.Uri.ToString();
            if (!navigatingToUrl.Contains("login.microsoftonline.com")) return;
            if (!navigatingToUrl.Contains("/oauth2/authorize")) return;
            if (navigatingToUrl.Contains("msafed=0")) return;
            //Don't offer login with Microsoft Account
            var url = navigatingToUrl.Replace("?", "?msafed=0&");
            _browser.Navigate(url);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            LoginInputRequired?.Invoke(this, EventArgs.Empty);
        }

        //Make sure we do not show user any annoying script error popups
        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are too early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        private async void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            //Wait 500 ms to make sure page is mostly loaded
            await Task.Delay(500);
            dynamic doc = _browser.Document;
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
                _browser.Navigate(tempUrl);
            }
            var browserUrl = _browser.Source.AbsoluteUri;
            if (value.Contains("/start.aspx#/Documents/Forms/All.aspx"))
            {
                _timer?.Stop();
                browserUrl = browserUrl.Replace("_layouts/15/onedrive.aspx", "Documents");
                //We got the URL and are good to go
                OneDrivePathFiguredOut?.Invoke(this, browserUrl);
            }
            if (browserUrl.Contains("https://login.microsoftonline.com/"))
            {
                //Wait 6 seconds so that AD FS has enough time to do SSO
                _timer?.Start();
            }

        }
    }
}
