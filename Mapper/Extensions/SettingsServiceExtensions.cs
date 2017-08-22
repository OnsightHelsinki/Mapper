using Mapper.Interfaces;

namespace Mapper.Extensions
{
    public static class SettingsServiceExtensions
    {
        public static string OneDrivePath(this ISettingsService service)
        {
            return service.GetSetting<string>("OneDrivePath");
        }

        public static char OneDriveLetter(this ISettingsService service)
        {
            return service.GetSetting<char>("OneDriveLetter");
        }

        public static string OneDriveBaseUrl(this ISettingsService service)
        {
            return service.GetSetting<string>("OneDriveBaseUrl");
        }

        public static string OneDriveADFSBaseUrl(this ISettingsService service)
        {
            return service.GetSetting<string>("OneDriveADFSBaseUrl");
        }

        public static bool SendAnalyticsToDeveloper(this ISettingsService service)
        {
            return service.GetSetting<bool>("SendAnalyticsToDeveloper");
        }

        public static string OneDriveName(this ISettingsService service)
        {
            return service.GetSetting<string>("OneDriveName");
        }

        public static string ApplicationInsightKey(this ISettingsService service)
        {
            return service.GetSetting<string>("ApplicationInsightKey");
        }
    }
}
