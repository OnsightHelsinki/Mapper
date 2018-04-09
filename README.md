# Mapper Installer

This tool maps OneDrive for Business as network drive using WebDAV. It does this automatically using users credentials, if it fails then it will ask user input.

## Steps
1. Figure out your SharePoint Online tenant name, if your tenant URL is https://sulava.sharepoint.com, then your tenant name is "sulava"
2. Install the tool (Requires administrator rights because it sets login.microsoftonline.com, office.com and your company domain as trusted sites to IE) and sets the tool to auto start on login as well as starts neccessary services for using WebDav
3. Restart computer and you should see X: mapped as your One Drive for Business 

## Installing

### Installation command
`msiexec /i Sulava.MapperPS.Installer.msi TENANT=[TENANT] DRIVE_LETTER=X USER_LOOKUP_MODE=1 /q`

### Parameters
- *TENANT*: Name of the SharePoint Online tenant, i.e. [TENANT].sharepoint.com
- *DRIVE_LETTER*: Which letter we try use for mapping. If letter is not available we try all the others.
- *USER_LOOKUP_MODE*: Parameter for specifying how the user is fetched. Values are:
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

## System changes

The installer will perform the configuration changes listed below.

### Registry keys set by the installer
- Add relevant sites as trusted in Internet Explorer. See [this](https://support.microsoft.com/en-us/help/182569/internet-explorer-security-zones-registry-entries-for-advanced-users) page for more details.
  - HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains
    - Name="https://[TENANT].sharepoint.com"
      - Type="integer" Name="https" Value=2 (trusted internet zone)
    - Name="https://[TENANT]-my.sharepoint.com"
      - Type="integer" Name="https" Value="2"
- Create a task run by Windows when starting
  - HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
    - Type="string" Name="Mapper Client" Value='Powershell.exe -WindowStyle Hidden -ExecutionPolicy ByPass -File "[INSTALLFOLDER]Mapper.ps1"'
- Disable the First Run Customize Page in Internet Explorer
  - HKLM\Software\Policies\Microsoft\Internet Explorer\Main
    - Type="integer" Name="DisableFirstRunCustomize" Value="1"
- Set the `webclient` service to automatically start
  - HKLM\System\CurrentControlSet\Services\Webclient
    - Type="integer" Name="Start" Value="2"
- Configure the `webclient` service
  - HKLM\System\CurrentControlSet\Services\Webclient\Parameters
    - Type="integer" Name="FileSizeLimitInBytes" Value="3221225472"
    - Type="integer" Name="ServerNotFoundCacheLifetimeInSec" Value="10"	
    - Type="integer" Name="SupportLocking" Value="0"
 
### Custom actions run by the installer
 - Start the webclient service
  - This require the installer to be started using an administrator account, or an account capable of starting services
  - Command: `NET START WEBCLIENT`
 
## Depencies

### OneDriveMapper by Jos Lieben
The Mapper installer wraps the OneDriveMapper tool by Jos Lieben in an installer.
- http://www.lieben.nu/liebensraum/onedrivemapper/
- Current bundled version 3.14

## Known problems and limitations
- MFA support is still pending / flaky
- Okta ‘kinda’ works in IE mode, native not supported
- RMUnify ‘kinda’ works in IE mode, native not supported
- This script does not work Powershell V2 or lower (comes with Windows 7), install V3 or higher

## License
- The installer is released under The MIT License (MIT) by Sulava Oy
- The OneDriveMapper.ps1 is released under its own license by Jos Lieben (OGD)
  - See https://gitlab.com/Lieben/OnedriveMapper_V3/blob/master/OneDriveMapper.ps1 for more details