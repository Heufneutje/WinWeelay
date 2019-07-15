using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MLib;
using MLib.Interfaces;
using WinWeelay.Configuration;

namespace WinWeelay
{
    public class ThemeManager
    {
        private IAppearanceManager _appearanceManager;
        private IThemeInfos _themeInfos;

        public ThemeManager()
        {
            _appearanceManager = AppearanceManager.GetInstance();
            _themeInfos = _appearanceManager.CreateThemeInfos();
        }

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

        public void UpdateTheme(string theme, AccentColor accentColor)
        {
            (Application.Current as App).ChangeSkin(theme);
            _appearanceManager.SetTheme(_themeInfos, theme, Color.FromRgb(accentColor.RedValue, accentColor.GreenValue, accentColor.BlueValue));
        }
    }
}
