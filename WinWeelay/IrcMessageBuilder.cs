using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WinWeelay.CustomControls;

namespace WinWeelay
{
    /// <summary>
    /// Helper class for parsing a FlowDocument to a string that can be sent with IRC formatting.
    /// </summary>
    public class IrcMessageBuilder
    {
        private FlowDocument _document;
        private StringBuilder _stringBuilder;
        private Color _defaultColor;

        /// <summary>
        /// Create a new instance of the helper.
        /// </summary>
        public IrcMessageBuilder()
        {
            _stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Build a message from a given document.
        /// </summary>
        /// <param name="document">The message input.</param>
        /// <param name="defaultColor">The default text color based on the current theme.</param>
        /// <returns>An IRC formatted string.</returns>
        public string BuildMessage(FlowDocument document, Color defaultColor)
        {
            _document = document;
            _defaultColor = defaultColor;

            _stringBuilder.Clear();
            Block block = _document.Blocks.FirstBlock;
            if (block == null)
                return string.Empty;

            foreach (Inline inline in (block as Paragraph).Inlines)
            {
                TextRange textRange = new TextRange(inline.ContentStart, inline.ContentEnd);
                AddFormattingChars(inline, true);
                _stringBuilder.Append(textRange.Text);
                AddFormattingChars(inline, false);
            }

            return _stringBuilder.ToString();
        }

        private void AddFormattingChars(Inline inline, bool isStart)
        {
            if (inline.FontWeight == FontWeights.Bold)
                _stringBuilder.Append("\u0002");
            if (inline.FontStyle == FontStyles.Italic)
                _stringBuilder.Append("\u001D");
            if (inline.TextDecorations == TextDecorations.Underline)
                _stringBuilder.Append("\u001F");

            byte backColor = 99;
            byte textColor = 99;
            if (inline.Background != null && inline.Background is SolidColorBrush backBrush)
                backColor = IrcColor.GetColorCode(backBrush.Color);

            if (inline.Foreground != null && inline.Foreground is SolidColorBrush textBrush)
                textColor = IrcColor.GetColorCode(textBrush.Color);

            if (textColor != 99 || backColor != 99)
            {
                _stringBuilder.Append("\u0003");
                if (isStart)
                {
                    if (backColor != 99 && textColor == 99)
                        textColor = 0;

                    if (backColor != 99)
                        _stringBuilder.Append($"{textColor},{backColor}");
                    else
                        _stringBuilder.Append(textColor.ToString());
                }
            }
        }
    }
}
