#
# Mapper.Settings.ps1
#
$configFile = "tenant.xml"
[xml]$config = Get-Content "$PSScriptRoot\$configFile"

$driveLetter           = $config.Settings.DriveLetter
$O365CustomerName      = $config.Settings.Tenant
$userLookupMode        = $config.Settings.UserLookupMode

$driveLetter = $driveLetter.TrimEnd(":") + ":"

$driveName = "$O365CustomerName drive"
$driveName[0] = $driveName[0].ToString().ToUpper()

#DEFAULT SETTINGS:
$desiredMappings =  @(
    @{"displayName"="$driveName";"targetLocationType"="driveletter";"targetLocationPath"="$driveLetter";"sourceLocationPath"="autodetect";"mapOnlyForSpecificGroup"=""}
)

$showConsoleOutput     = $False 
$authMethod            = "native"