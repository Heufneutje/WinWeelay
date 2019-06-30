using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            Paragraph paragraph = new Paragraph();

            string licenseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LICENSE.txt");
            if (File.Exists(licenseFilePath))
                paragraph.Inlines.Add(File.ReadAllText(licenseFilePath));
            else
                paragraph.Inlines.Add("Licenses file is missing.");
            FlowDocument document = new FlowDocument(paragraph);
            _richTextBox.Document = document;

            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            _versionLabel.Content = $"WinWeelay v{string.Join(".", new int[3] { fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart })}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
