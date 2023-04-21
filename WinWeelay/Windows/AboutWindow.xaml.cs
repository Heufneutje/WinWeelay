using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using MWindowLib;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Window to display info about the application.
    /// </summary>
    public partial class AboutWindow : MetroWindow
    {
        /// <summary>
        /// Create an instance of the window.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
            Paragraph paragraph = new();

            string licenseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LICENSE.txt");
            if (File.Exists(licenseFilePath))
                paragraph.Inlines.Add(File.ReadAllText(licenseFilePath));
            else
                paragraph.Inlines.Add("Licenses file is missing.");
            FlowDocument document = new(paragraph);
            document.Foreground = new SolidColorBrush(App.CurrentTheme == Themes.Dark ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
            _richTextBox.Document = document;

            FileVersionInfo fvi = UpdateHelper.GetCurrentVersion();
            _versionLabel.Content = $"WinWeelay v{string.Join(".", new int[3] { fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart })}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
