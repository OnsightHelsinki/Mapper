using Mapper.Extensions;
using Microsoft.Win32;

namespace Mapper.Services
{
    public class ShellService
    {
        const string ShellKeyLocation = @"Software\Microsoft\Windows\CurrentVersion\Explorer\MountPoints2";
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
    }
}
