using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
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
        public static string CurrentTheme { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            SetupExceptionHandling();

            MainWindow window = new MainWindow();
            MainViewModel model = new MainViewModel(window);
            window.DataContext = model;
            window.Show();
        }

        public void ChangeSkin(string newSkin)
        {
            Resources.Clear();
            Resources.MergedDictionaries.Clear();
            CurrentTheme = newSkin;

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

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => LogUnhandledException((Exception)e.ExceptionObject);
            Dispatcher.UnhandledException += (s, e) => LogUnhandledException(e.Exception);
            TaskScheduler.UnobservedTaskException += (s, e) => LogUnhandledException(e.Exception);
        }

        private void LogUnhandledException(Exception exception)
        {
            try
            {
                ExceptionWindow exceptionWindow = new ExceptionWindow(exception);
                exceptionWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay");
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                File.WriteAllText(Path.Combine(appDataPath, $"ExceptionLog_{DateTime.Now:yyyy-MM-dd_HHmmss}.txt"), $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}
