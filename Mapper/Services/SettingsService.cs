using Mapper.Interfaces;

namespace Mapper.Services
{
    public class SettingsService : ISettingsService
    {
        public T GetSetting<T>(string key)
        {
            return (T)Properties.Settings.Default[key];
        }

        public void SaveSettings<T>(string key, T value)
        {
            Properties.Settings.Default[key] = value;
            Properties.Settings.Default.Save();
        }
    }
}
