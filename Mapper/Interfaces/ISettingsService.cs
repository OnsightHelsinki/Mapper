namespace Mapper.Interfaces
{
    public interface ISettingsService
    {
        T GetSetting<T>(string key);
        void SaveSettings<T>(string key, T value);
    }
}