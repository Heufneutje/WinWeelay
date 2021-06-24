using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using WinWeelay.Configuration;

namespace WinWeelay
{
    /// <summary>
    /// Main class for the application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Name of the current application theme.
        /// </summary>
        public static string CurrentTheme { get; set; }

        /// <summary>
        /// Set up exception handling and initialize the main view model.
        /// </summary>
        /// <param name="e">Event arguments.</param>
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

        /// <summary>
        /// Change the current application theme.
        /// </summary>
        /// <param name="themeName">Name of the theme to change to.</param>
        public void ChangeTheme(string themeName)
        {
            Resources.Clear();
            Resources.MergedDictionaries.Clear();
            CurrentTheme = themeName;

            switch (themeName)
            {
                case Themes.Light:
                    ApplyResources("/AvalonDock.Themes.VS2013;component/LightTheme.xaml");
                    ApplyResources("/AvalonDock.Themes.VS2013;component/LightBrushs.xaml");
                    break;
                case Themes.Dark:
                    ApplyResources("/AvalonDock.Themes.VS2013;component/DarkTheme.xaml");
                    ApplyResources("/AvalonDock.Themes.VS2013;component/DarkBrushs.xaml");
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
