using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using MWindowLib;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Window to display unhandled exceptions.
    /// </summary>
    public partial class ExceptionWindow : MetroWindow
    {
        /// <summary>
        /// Create a new instance of the exception window with a given exception.
        /// </summary>
        /// <param name="ex">The exception that has occurred.</param>
        public ExceptionWindow(Exception ex)
        {
            InitializeComponent();
            DataContext = ex;

            // TODO: Properly fix theming to make read-only text boxes actually readable.
            SolidColorBrush brush = new(App.CurrentTheme == Themes.Dark ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
            _document.Foreground = brush;
            _messageTextBox.Foreground = brush;
            _sourceTextBox.Foreground = brush;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessUtils.StartProcess(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
