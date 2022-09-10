using System.Runtime.InteropServices;
using WinWeelay.Configuration;

namespace WinWeelay
{
    /// <summary>
    /// Helper class for getting theme UI theme based on the system theme.
    /// </summary>
    public static class WindowsThemeManager
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        private static extern bool ShouldSystemUseDarkMode();

        /// <summary>
        /// Get theme based on system settings.
        /// </summary>
        /// <returns>System theme.</returns>
        public static string GetSystemTheme()
        {
            try
            {
                return ShouldSystemUseDarkMode() ? Themes.Dark : Themes.Light;
            }
            catch
            {
                return Themes.Dark;
            }
        }
    }
}
