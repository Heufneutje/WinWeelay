using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using WinWeelay.Configuration;

namespace WinWeelay
{
    public static class RichTextHelper
    {
        public static string GetXaml(this RichTextBox textBox)
        {
            TextRange range = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            MemoryStream stream = new MemoryStream();
            range.Save(stream, DataFormats.Xaml);
            string xamlText = Encoding.UTF8.GetString(stream.ToArray());
            return xamlText;
        }

        public static void SetXaml(this RichTextBox textBox, string text)
        {
            FlowDocument doc = new FlowDocument();
            if (!string.IsNullOrEmpty(text))
            {
                StringReader stringReader = new StringReader(text);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                Section sec = XamlReader.Load(xmlReader) as Section;
                while (sec.Blocks.Count > 0)
                    doc.Blocks.Add(sec.Blocks.FirstBlock);
            }
            textBox.Document = doc;
            textBox.SetCaretToEnd();
        }

        public static bool IsEmpty(this RichTextBox textBox)
        {
            string text = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text;
            return string.IsNullOrEmpty(text);
        }

        public static string GetPlainText(this RichTextBox textBox)
        {
            TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            return textRange.Text.TrimEnd();
        }

        public static void SetPlainText(this RichTextBox textBox, string text)
        {
            textBox.Document.Blocks.Clear();
            textBox.Document.Blocks.Add(new Paragraph(new Run(text)));
            textBox.SetCaretToEnd();
        }

        public static void SetCaretToEnd(this RichTextBox textBox)
        {
            TextPointer pointer = textBox.Document.ContentEnd;
            textBox.Selection.Select(pointer, pointer);
        }

        public static void UpdateFont(this FlowDocument document, double fontSize, FontFamily fontFamily, bool changeTextColor)
        {
            document.FontFamily = fontFamily;
            document.FontSize = fontSize;

            if (changeTextColor)
                document.Foreground = new SolidColorBrush()
                {
                    // The themes automatically set the text color to gray because editing is not enabled, which is not ideal. Override this behavior.
                    Color = App.CurrentTheme == Themes.Dark ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0)
                };

            foreach (Block block in document.Blocks)
            {
                block.FontFamily = fontFamily;
                block.FontSize = fontSize;
            }
        }
    }
}
