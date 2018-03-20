# Mapper Installer

This tool maps OneDrive for Business as network drive using WebDAV. It does this automatically using users credentials, if it fails then it will ask user input.

## Steps
1. Figure out your One Drive for Business base URL it looks like something-my.sharepoint.com
2. Install the tool (Requires administrator rights because it sets login.microsoftonline.com, office.com and your company domain as trusted sites to IE) and sets the tool to auto start on login as well as starts neccessary services for using WebDav
3. Restart computer and you should see X: mapped as your One Drive for Business 

## Installing
msiexec /i Sulava.MapperPS.Installer.msi TENANT={tenant name} DRIVE_LETTER=X USER_LOOKUP_MODE=1 /q

### Installing parameters
- DRIVE_LETTER: Which letter we try use for mapping. If letter is not available we try all the others.
- DRIVE_NAME: How we name the share on explorer
- USER_LOOKUP_MODE: Parameter for specifying how the user is fetched. Values are:
  -	1 = Active Directory UPN, 
  -	2 = Active Directory Email, 
  -	3 = Azure AD Joined Windows 10, 
  -	4 = query user for his/her login, 
  -	5 = lookup by registry key, 
  -	6 = display full form (ask for both username and login if no cached versions can be found), 
  -	7 = whoami /upn


## Requirements for development
- Visual Studio 2015
- [Wix Toolset 3.11.0.1528](http://wixtoolset.org/releases/v3-11-0-1528)

## Depencies

### OneDriveMapper by Jos Lieben
The Mapper installer wraps the OneDriveMapper tool by Jos Lieben in an installer.
- http://www.lieben.nu/liebensraum/onedrivemapper/

## Known problems and limitations
- MFA support is still pending
- Okta ‘kinda’ works in IE mode, native not supported
- RMUnify ‘kinda’ works in IE mode, native not supported
- If you use redirection and restartExplorer true, make sure the OneDriveMapper runs when the user is fully logged in, restarting explorer during logon can cause hangs. Best practise: don’t enable it, the redirect will work the next logon for roaming profiles
- The OneDriveMapper script does not work Powershell V2 or lower (comes with Windows 7), install V3 or higher

## License
- The installer is released under The MIT License (MIT) by Sulava Oy
- The OneDriveMapper.ps1 is released under its own license by Jos Lieben (OGD)
  - See https://gitlab.com/Lieben/OnedriveMapper_V3/blob/master/OneDriveMapper.ps1 for more details