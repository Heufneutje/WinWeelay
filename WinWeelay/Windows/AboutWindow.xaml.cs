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
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : MetroWindow
    {
        public AboutWindow(RelayConfiguration config)
        {
            InitializeComponent();
            Paragraph paragraph = new Paragraph();

            string licenseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LICENSE.txt");
            if (File.Exists(licenseFilePath))
                paragraph.Inlines.Add(File.ReadAllText(licenseFilePath));
            else
                paragraph.Inlines.Add("Licenses file is missing.");
            FlowDocument document = new FlowDocument(paragraph);
            document.Foreground = new SolidColorBrush(config.Theme == Themes.Dark ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
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
