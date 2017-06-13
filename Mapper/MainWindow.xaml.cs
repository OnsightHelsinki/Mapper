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
        public MainWindow()
        {
            InitializeComponent();
            _loggingService = new LoggingService();
            _settingsService = new SettingsService();
            _loggingService.Initialize(_settingsService.SendAnalyticsToDeveloper(), _settingsService.ApplicationInsightKey());
            _networkDriveService = new NetworkDriveService(_loggingService);
            var browserService = new BrowserService(browser1);
            _shellService = new ShellService();
            browserService.OneDrivePathFiguredOut += _browserService_OneDrivePathFiguredOut;
            browserService.LoginInputRequired += _browserService_LoginInputRequired;
            Loaded += MainWindow_Loaded;
            Top = -2000;
        }

        private void _browserService_LoginInputRequired(object sender, EventArgs e)
        {
            Top = 200;
            Height = 570;
            Width = 500;
        }

        private void _browserService_OneDrivePathFiguredOut(object sender, string e)
        {
            _settingsService.SaveSettings("OneDrivePath", e);
            MapNetworkDrive();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_settingsService.OneDrivePath()))
            {
                browser1.Navigate(_settingsService.OneDriveBaseUrl());
            }
            else
            {
                MapNetworkDrive();
            }
        }

        private void MapNetworkDrive()
        {
            _shellService.VerifyAndAddPathAsTrusted(_settingsService.OneDrivePath());

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
                    browser1.Navigate(_settingsService.OneDriveBaseUrl());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
