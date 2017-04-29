# Mapper

This tool maps OneDrive for Business as network drive using WebDAV. It does this automatically using users credentials, if it fails then it will ask user input.

## Steps
1. Figure out your One Drive for Business base URL it looks like something-my.sharepoint.com
2. Install the tool (Requires administrator rights because it sets login.microsoftonline.com, office.com and your company domain as trusted sites to IE) and sets the tool to auto start on login
3. Restart computer and you should see X: mapped as your One Drive for Business 

## Installing
msiexec /i Mapper.Installer.msi /q DRIVE_LETTER=X DRIVE_NAME=OneDrive SEND_ANALYTICS=False BASE_URL={Tenant name}-my.sharepoint.com APPLICATION_INSIGHT_KEY=d1d1c83a-eab5-4822-9b41-4d337b3b649d

### Installing parameters
- DRIVE_LETTER: Which letter we try use for mapping. If letter is not available we try all the others.
- DRIVE_NAME: How we name the share on explorer
- SEND_ANALYTICS: Do we send analytics to Application Insight
- BASE_URL: The url we add into "Trusted Sites" and use to figure out the paths and do login
- APPLICATION_INSIGHT_KEY: The key for application insight where we want to send analytics data

### Registry key changes
- Trusted Sites: We add the base url domain into IE trusted sites category. (Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\)
- Auto start: This makes computer start the application on logon. (SOFTWARE\Microsoft\Windows\CurrentVersion\Run)

## Requirements for development
- Visual Studio 2015
- [Wix Toolset 3.11.0.1528](http://wixtoolset.org/releases/v3-11-0-1528)

## Depencies

### Application Insights
This is used for analytics

### Costura.Fody
This is used to embed dll files into compiled .exe

## Known problems

### User has 2-Factor authentication enabled
The tool will open a window with browser view that asks user to enter the 2-Factor code. User can then select the tool to not ask for the 2-Factor verification for X days, where X depends on corporation settings.

## Frequent asked questions

### Can I use this tool to map sharepoint sites to explorer
Yes. This tool will do the initial mapping and while doing that it refreshes the WebDAV connection. You can then map some the sharepoint sites with using the cmd line command "net use {letter}: {url} /persistent:yes"

## Contributing
Contributing is encouraged! Please submit pull requests, open issues etc. However, to ensure we end up with a good result and to make my life a little easier, could I please request that;

* All changes be made in a feature branch, not in master, and please don't submit PR's directly against master.

Thanks! I look forward to merge your contribution.