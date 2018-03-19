#
# OneDriveMapper.Settings.ps1
#
$configFile = "tenant.xml"
[xml]$config = Get-Content "$PSScriptRoot\$configFile"

$driveLetter           = $config.Settings.DriveLetter
$O365CustomerName      = $config.Settings.Tenant
$userLookupMode        = $config.Settings.UserLookupMode