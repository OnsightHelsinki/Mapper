using System;
using System.Windows;
using Mapper.Extensions;
using Mapper.Interfaces;
using Mapper.Services;

namespace Mapper
{
    public partial class MainWindow : Window
    {
        private readonly INetworkDriveService _networkDriveService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;
        private readonly ShellService _shellService;
        private readonly BrowserService _browserService;
        public MainWindow()
        {
            InitializeComponent();
            _loggingService = new LoggingService();
            _settingsService = new SettingsService();
            _loggingService.Initialize(_settingsService.SendAnalyticsToDeveloper(), _settingsService.ApplicationInsightKey());
            _networkDriveService = new NetworkDriveService(_loggingService);
            _browserService = new BrowserService();
            _shellService = new ShellService();
            _browserService.OneDrivePathFiguredOut += _browserService_OneDrivePathFiguredOut;
            Loaded += MainWindow_Loaded;
            Top = -2000;
        }

        private void _browserService_OneDrivePathFiguredOut(object sender, string e)
        {
            Application.Current.Dispatcher.Invoke( () => {
                _settingsService.SaveSettings("OneDrivePath", e);
                MapNetworkDrive();
            });

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.OneDrivePath()))
            {
                _browserService.Navigate(_settingsService.OneDriveBaseUrl());
            }
            else
            {
                MapNetworkDrive();
            }
        }

        private void MapNetworkDrive()
        {
            var result = _networkDriveService.MapNetworkDrive(
                _settingsService.OneDriveLetter(),
                _settingsService.OneDrivePath(),
                200);

            switch (result)
            {
                case MappingNetworkDriveResult.Success:
                    _loggingService.Log("Succeeded on adding the onedrive as mapped drive");
                    _shellService.RenameDrive(_settingsService.OneDrivePath(), _settingsService.OneDriveName());
                    Close();
                    break;
                case MappingNetworkDriveResult.FailedBecauseOfLogin:
                    _loggingService.Log("Failed to add onedrive as mapped drive, url " + _settingsService.OneDriveBaseUrl());
                    _browserService.Navigate(_settingsService.OneDriveBaseUrl());
                    break;
                case MappingNetworkDriveResult.WrongParameters:
                    _loggingService.Log("We failed with wrong parameters");
                    Close();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _browserService.Cleanup();
        }
    }
}
