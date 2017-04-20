namespace Mapper.Interfaces
{
    public interface ILoggingService
    {
        void Initialize(bool enabled, string instrumentationKey);
        void Log(string eventName);
    }
}