using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MLib;
using WinWeelay.Configuration;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow window = new MainWindow();
            MainViewModel model = new MainViewModel(window);
            window.DataContext = model;

            window.Show();
        }

        public void ChangeSkin(string newSkin)
        {
            Resources.Clear();
            Resources.MergedDictionaries.Clear();

            switch (newSkin)
            {
                case Themes.Light:
                    ApplyResources("/Xceed.Wpf.AvalonDock.Themes.VS2013;component/LightTheme.xaml");
                    ApplyResources("/Xceed.Wpf.AvalonDock.Themes.VS2013;component/LightBrushs.xaml");
                    break;
                case Themes.Dark:
                    ApplyResources("/Xceed.Wpf.AvalonDock.Themes.VS2013;component/DarkTheme.xaml");
                    ApplyResources("/Xceed.Wpf.AvalonDock.Themes.VS2013;component/DarkBrushs.xaml");
                    break;
            }
        }

        private void ApplyResources(string src)
        {
            ResourceDictionary dict = new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) };
            foreach (ResourceDictionary mergeDict in dict.MergedDictionaries)
                Resources.MergedDictionaries.Add(mergeDict);

            foreach (object key in dict.Keys)
                Resources[key] = dict[key];
        }
    }
}
