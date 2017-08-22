using System;
using System.Linq;
using Mapper.Extensions;
using Microsoft.Win32;

namespace Mapper.Services
{
    public class ShellService
    {
        private const string TrustedSitesKeyLocation = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\\Domains";
        private const string ZoneKeyLocation = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones";
        private const string ShellKeyLocation = @"Software\Microsoft\Windows\CurrentVersion\Explorer\MountPoints2";
        private const string InternetZone = "3";
        private const string ProtectedModeKey = "2500";
        private const string SPParentKeyLabel = "sharepoint.com";
        private const string O365PortalParentKeyLabel = "office.com";
        private const string AADKeyLabel = "microsoftonline.com";

        public void RenameDrive(string drive, string name)
        {
           var dav = drive
                .Replace("https:","")
                .TrimEnd('/')
                .Replace("/","#")
                .Replace("#personal", "@SSL#DavWWWRoot#personal");

            // Win10 
            RegistryKey currentUserKey = Registry.CurrentUser;
            var davKey = currentUserKey.GetOrCreateSubKey(ShellKeyLocation, dav, true);
            davKey.SetValue("_LabelFromReg", name, RegistryValueKind.String);

            // Win 8.1
            dav = dav.Replace("DavWWWRoot#", string.Empty);
            davKey = currentUserKey.GetOrCreateSubKey(ShellKeyLocation, dav, true);
            davKey.SetValue("_LabelFromReg", name, RegistryValueKind.String);
        }

        public void VerifyAndAddPathAsTrusted()
        {
            RegistryKey currentUserKey = Registry.CurrentUser;

            //Add https://onsight-my.sharepoint.com as a trusted site
            var trustedSitesSPKey = currentUserKey.GetOrCreateSubKey(TrustedSitesKeyLocation, SPParentKeyLabel, true);
            trustedSitesSPKey.SetValue("https", 2, RegistryValueKind.DWord);

            //Add https://*.office.com as a trusted site
            var trustedSitesO365Key = currentUserKey.GetOrCreateSubKey(TrustedSitesKeyLocation, O365PortalParentKeyLabel, true);
            trustedSitesO365Key.SetValue("https", 2, RegistryValueKind.DWord);

            //Add https://*.microsoftonline.com as a trusted site
            var trustedSitesAADKey = currentUserKey.GetOrCreateSubKey(TrustedSitesKeyLocation, AADKeyLabel, true);
            trustedSitesAADKey.SetValue("https", 2, RegistryValueKind.DWord);
        }

        public void DisableInternetZoneProtectedMode()
        {
            Registry.CurrentUser.GetOrCreateSubKey(ZoneKeyLocation, InternetZone, true).SetValue(ProtectedModeKey, 3, RegistryValueKind.DWord);
        }
    }
}
