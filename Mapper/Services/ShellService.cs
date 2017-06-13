using System;
using System.Linq;
using Mapper.Extensions;
using Microsoft.Win32;

namespace Mapper.Services
{
    public class ShellService
    {
        const string ShellKeyLocation = @"Software\Microsoft\Windows\CurrentVersion\Explorer\MountPoints2";
        const string TrustedSitesKeyLocation = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains";
        const string SPParentKeyLabel = "sharepoint.com";
        const string AADKeyLabel = "microsoftonline.com";

        public void RenameDrive(string drive, string name)
        {
           var dav = drive
                .Replace("https:","")
                .TrimEnd('/')
                .Replace("/","#")
                .Replace("#personal", "@SSL#DavWWWRoot#personal");

            RegistryKey currentUserKey = Registry.CurrentUser;
            var davKey = currentUserKey.GetOrCreateSubKey(ShellKeyLocation, dav, true);
            davKey.SetValue("_LabelFromReg", name, RegistryValueKind.String);
        }

        public void VerifyAndAddPathAsTrusted(string personalUrl)
        {
            Uri uri = new Uri(personalUrl);
            var subdomain = uri.Host.Split('.').First(); //only the subdomain is interesting.
            var protocol = uri.Scheme;

            RegistryKey currentUserKey = Registry.CurrentUser;

            //Add https://onsight-my.sharepoint.com as a trusted site
            var trustedSitesSPKey = currentUserKey.GetOrCreateSubKey(TrustedSitesKeyLocation, SPParentKeyLabel, true);
            var SPKey = currentUserKey.GetOrCreateSubKey(TrustedSitesKeyLocation+"\\"+SPParentKeyLabel, subdomain, true);
            SPKey.SetValue(protocol, 2, RegistryValueKind.DWord);

            //Add https://*.microsoftonline.com as a trusted site
            var trustedSitesAADKey = currentUserKey.GetOrCreateSubKey(TrustedSitesKeyLocation, AADKeyLabel, true);
            trustedSitesAADKey.SetValue("https", 2, RegistryValueKind.DWord);
        }
    }
}
