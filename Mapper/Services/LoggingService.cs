using Mapper.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Mapper.Services
{
    public class LoggingService : ILoggingService
    {
        private TelemetryClient _telemetryClient;
        private bool _enabled;

        public void Initialize(bool enabled, string instrumentationKey)
        {
            _enabled = enabled;
            if (!_enabled) return;
            var configuration = new TelemetryConfiguration { InstrumentationKey = instrumentationKey };
            _telemetryClient = new TelemetryClient(configuration)
            {
                InstrumentationKey = instrumentationKey
            };
            _telemetryClient.Context.User.AccountId = System.Environment.MachineName;
        }

        public void Log(string eventName)
        {
            if (!_enabled) return;
            _telemetryClient.TrackEvent(new EventTelemetry { Name = eventName });
            _telemetryClient.Flush();
        }
    }
}
