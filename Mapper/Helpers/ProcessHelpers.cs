using System.Diagnostics;

namespace Mapper.Helpers
{
    public static class ProcessHelpers
    {
        public static Process ExecuteCommandHidden(string cmd)
        {
            var pInfo = new ProcessStartInfo("cmd", cmd)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            return Process.Start(pInfo);
        }
    }
}
