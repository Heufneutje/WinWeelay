using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MLib;
using MLib.Interfaces;
using WinWeelay.Configuration;

namespace WinWeelay
{
    /// <summary>
    /// Helper class for registering theme resource dictionaries.
    /// </summary>
    public class ThemeManager
    {
        private IAppearanceManager _appearanceManager;
        private IThemeInfos _themeInfos;

        /// <summary>
        /// Create a new theme manager.
        /// </summary>
        public ThemeManager()
        {
            _appearanceManager = AppearanceManager.GetInstance();
            _themeInfos = _appearanceManager.CreateThemeInfos();
        }

        /// <summary>
        /// Initialize the resource dictionaries and apply the default theme.
        /// </summary>
        /// <param name="relayConfiguration">Configuration containing the default theme.</param>
        public void InitializeThemes(RelayConfiguration relayConfiguration)
        {
            _appearanceManager.SetDefaultThemes(_themeInfos);
            _appearanceManager.AddThemeResources(Themes.Dark, new List<Uri>
            {
                new Uri("/MWindowLib;component/Themes/DarkTheme.xaml", UriKind.RelativeOrAbsolute),
                new Uri("/WinWeelay;component/ResourceDictionaries/DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)

            }, _themeInfos);

            _appearanceManager.AddThemeResources(Themes.Light, new List<Uri>
            {
                new Uri("/MWindowLib;component/Themes/LightTheme.xaml", UriKind.RelativeOrAbsolute),
                new Uri("/WinWeelay;component/ResourceDictionaries/DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)

            }, _themeInfos);

            UpdateTheme(relayConfiguration.Theme, relayConfiguration.AccentColor);
        }

        /// <summary>
        /// Apply the theme with the given name.
        /// </summary>
        /// <param name="theme">The theme to apply.</param>
        /// <param name="accentColor">The accent color to use with the theme.</param>
        public void UpdateTheme(string theme, AccentColor accentColor)
        {
            (Application.Current as App).ChangeSkin(theme);
            _appearanceManager.SetTheme(_themeInfos, theme, Color.FromRgb(accentColor.RedValue, accentColor.GreenValue, accentColor.BlueValue));
        }
    }
}
