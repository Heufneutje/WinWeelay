using System.Diagnostics;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Helper class for running processes.
    /// </summary>
    public static class ProcessUtils
    {
        /// <summary>
        /// Start a new process with shell execute.
        /// </summary>
        /// <param name="path">The path to the process.</param>
        /// <param name="arguments">The arguments to start the process with.</param>
        /// <param name="runWithElevation">Whether the process that is to be started should request elevation.</param>
        public static void StartProcess(string path, string arguments = null, bool runWithElevation = false)
        {
            ProcessStartInfo psi = new ProcessStartInfo(path) { UseShellExecute = true };
            if (!string.IsNullOrEmpty(arguments))
                psi.Arguments = arguments;

            if (runWithElevation)
                psi.Verb = "runas";

            Process.Start(psi);
        }
    }
}
