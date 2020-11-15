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
    /// <summary>
    /// Helper functions for RichTextBox objects and FlowDocument objects.
    /// </summary>
    public static class RichTextHelper
    {
        /// <summary>
        /// Export the text box's document to a XAML string.
        /// </summary>
        /// <param name="textBox">The text box containing the document that will be exported.</param>
        /// <returns>A XAML string containing the document.</returns>
        public static string GetXaml(this RichTextBox textBox)
        {
            TextRange range = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            MemoryStream stream = new MemoryStream();
            range.Save(stream, DataFormats.Xaml);
            string xamlText = Encoding.UTF8.GetString(stream.ToArray());
            return xamlText;
        }

        /// <summary>
        /// Set a text box's document from an exported XAML string.
        /// </summary>
        /// <param name="textBox">The text box to load the document into.</param>
        /// <param name="xamlText">The XAML containing the exported document.</param>
        public static void SetXaml(this RichTextBox textBox, string xamlText)
        {
            FlowDocument doc = new FlowDocument();
            if (!string.IsNullOrEmpty(xamlText))
            {
                StringReader stringReader = new StringReader(xamlText);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                Section sec = XamlReader.Load(xmlReader) as Section;
                while (sec.Blocks.Count > 0)
                    doc.Blocks.Add(sec.Blocks.FirstBlock);
            }
            textBox.Document = doc;
            textBox.SetCaretToEnd();
        }

        /// <summary>
        /// Check whether the text box has text in it.
        /// </summary>
        /// <param name="textBox">The text box to check.</param>
        /// <returns>True if text is contained.</returns>
        public static bool IsEmpty(this RichTextBox textBox)
        {
            string text = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd).Text;
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// Export the text box's document in plain text.
        /// </summary>
        /// <param name="textBox">The text box to check.</param>
        /// <returns>Plain text.</returns>
        public static string GetPlainText(this RichTextBox textBox)
        {
            TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            return textRange.Text.TrimEnd();
        }

        /// <summary>
        /// Set the text box's document from plain text.
        /// </summary>
        /// <param name="textBox">The text box to set the text to.</param>
        /// <param name="text">The plain text to set.</param>
        public static void SetPlainText(this RichTextBox textBox, string text)
        {
            textBox.Document.Blocks.Clear();
            textBox.Document.Blocks.Add(new Paragraph(new Run(text)));
            textBox.SetCaretToEnd();
        }

        /// <summary>
        /// Set the caret of a given text box to the end of the text.
        /// </summary>
        /// <param name="textBox">The text box to change.</param>
        public static void SetCaretToEnd(this RichTextBox textBox)
        {
            TextPointer pointer = textBox.Document.ContentEnd;
            textBox.Selection.Select(pointer, pointer);
        }

        /// <summary>
        /// Update the font settings.
        /// </summary>
        /// <param name="document">The document to change.</param>
        /// <param name="fontSize">The font size to set.</param>
        /// <param name="fontFamily">The font family to set.</param>
        /// <param name="changeTextColor">Whether or not the default color should be applied to the text.</param>
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
