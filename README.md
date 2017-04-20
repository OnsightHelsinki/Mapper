# Mapper

This tool maps OneDrive for Business as network drive. It does this automatically using users credentials, if it fails then it will ask user input.

## Steps
1. Figure out your One Drive for Business base URL it looks like something-my.sharepoint.com
2. Install the tool (Requires administrator rights because it sets login.microsoftonline.com, office.com and your company domain as trusted sites to IE) and sets the tool to auto start on login
3. Restart computer and you should see X: mapped as your One Drive for Business 

## Installing
msiexec /i OneDriveMapper.Installer.msi /q DRIVE_LETTER=X DRIVE_NAME=OneDrive SEND_ANALYTICS=True BASE_URL=onsight-my.sharepoint.com

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

## Contributing
Contributing is encouraged! Please submit pull requests, open issues etc. However, to ensure we end up with a good result and to make my life a little easier, could I please request that;

* All changes be made in a feature branch, not in master, and please don't submit PR's directly against master.

Thanks! I look forward to merge your contribution.