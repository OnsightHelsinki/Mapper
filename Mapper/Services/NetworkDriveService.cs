using System;
using Mapper.Helpers;
using Mapper.Interfaces;

namespace Mapper.Services
{
    public class NetworkDriveService : INetworkDriveService
    {
        private readonly ILoggingService _loggingService;

        public NetworkDriveService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [STAThread]
        public MappingNetworkDriveResult MapNetworkDrive(char driveLetter, string path, int waitTimeMs)
        {
            driveLetter = FindAvailableDriveLetter(driveLetter);
            var cmd = "/c net use " + driveLetter + ": "+ path + " /persistent:no";
            var p = ProcessHelpers.ExecuteCommandHidden(cmd);

            if (p != null)
            {
                var output = p.StandardOutput.ReadToEnd();
                var error = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    _loggingService.Log("Error happened: " + error);
                }
                if (error ==
                    "System error 224 has occurred.\r\n\r\nAccess Denied. Before opening files in this location, you must first add the web site to your trusted sites list, browse to the web site, and select the option to login automatically.\r\n\r\n")
                {
                    return MappingNetworkDriveResult.FailedBecauseOfLogin;
                } else if (error.StartsWith("System error 67 has occurred."))
                {
                    return MappingNetworkDriveResult.WrongParameters;
                }
                if (output != "The command completed successfully.\r\n\r\n")
                {
                    return MappingNetworkDriveResult.Success;
                }
                p.WaitForExit();
            }
            p?.Close();
            return MappingNetworkDriveResult.Success;
        }

        [STAThread]
        private static char FindAvailableDriveLetter(char preferableDriveLetter)
        {
            var availableDriveLetter = preferableDriveLetter;
            var cmd = "/c if exist " + preferableDriveLetter + ":/ ( echo LetterTaken )";
            var p = ProcessHelpers.ExecuteCommandHidden(cmd);
            var output = p.StandardOutput.ReadToEnd();
            p.Close();
            if (!output.Contains("LetterTaken")) return availableDriveLetter;
            for (var letter = 'Z'; letter >= 'E'; letter--)
            {
                cmd = "/c if exist " + letter + ":/ ( echo LetterTaken )";
                p = ProcessHelpers.ExecuteCommandHidden(cmd);
                output = p.StandardOutput.ReadToEnd();
                if (!output.Contains("LetterTaken"))
                {
                    availableDriveLetter = letter;
                    break;
                }
                p.Close();
            }
            return availableDriveLetter;
        }


    }
}
