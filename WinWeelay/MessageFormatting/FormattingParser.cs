using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Parser which converts the formatting codes within a relay message to actual formatting.
    /// </summary>
    public class FormattingParser
    {
        private readonly ColorHelper _colorHelper;
        private readonly OptionParser _optionParser;
        private bool _parseFormatting;

        private int _index;
        private string _formattedString;
        private string _messagePart;
        private List<Inline> _inlines;
        private List<AttributeType> _attributes;
        private int _foreColor;
        private int _backColor;
        private readonly Regex _urlRegex;

        /// <summary>
        /// Create a new instance of the parser.
        /// </summary>
        /// <param name="optionParser">Parser which handles WeeChat colors.</param>
        public FormattingParser(OptionParser optionParser)
        {
            _colorHelper = new ColorHelper();
            _optionParser = optionParser;
            _urlRegex = new Regex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Format an entire received buffer message.
        /// </summary>
        /// <param name="message">The message to format.</param>
        /// <param name="timestampFormat">The timestamp format to use when prepending the timestamp to the message contents.</param>
        /// <param name="parseFormatting">True to convert the formatting codes to actual formatting, false to just strip the formatting codes from the raw message.</param>
        /// <returns>A FlowDocument paragraph which contains the formatted message.</returns>
        public Paragraph FormatMessage(RelayBufferMessage message, string timestampFormat, bool parseFormatting)
        {
            _parseFormatting = parseFormatting;

            string formattedDate = $"[{message.Date.ToString(timestampFormat)}] ";

            string highlightFormatting = null;
            if (message.IsHighlighted)
                highlightFormatting = $"\u001a\u0001\u0019{_optionParser.GetOptionColorCode(OptionNames.WeechatColorChatHighlight)}";

            Paragraph paragraph = new();

            if (message.IsHighlighted)
                paragraph.Inlines.AddRange(HandleFormatting($"{highlightFormatting}{formattedDate}"));
            else
                paragraph.Inlines.Add(new Run(formattedDate));

            string prefix = message.Prefix;
            if (message.IsPrivmsg)
            {
                if (message.IsHighlighted)
                    prefix = $"{highlightFormatting}<{GetUnformattedString(message.Prefix)}>\u0019\u0001\u001a";
                else
                    prefix = $"<{prefix}\u0019\u001c>";
            }
            prefix += ' ';

            paragraph.Inlines.AddRange(HandleFormatting(prefix));
            paragraph.Inlines.AddRange(HandleFormatting(ParseUrls(message.MessageBody)));
            return paragraph;
        }

        /// <summary>
        /// Format a single string which contains formatting codes.
        /// </summary>
        /// <param name="formattedString">The message to format.</param>
        /// <param name="parseFormatting">True to convert the formatting codes to actual formatting, false to just strip the formatting codes from the raw message.</param>
        /// <returns>A FlowDocument paragraph which contains the formatted message.</returns>
        public Paragraph FormatString(string formattedString, bool parseFormatting)
        {
            _parseFormatting = parseFormatting;

            Paragraph paragraph = new();
            paragraph.Inlines.AddRange(HandleFormatting(ParseUrls(formattedString)));
            return paragraph;
        }

        /// <summary>
        /// Strip the formatting codes from a given text string.
        /// </summary>
        /// <param name="formattedString">The text string.</param>
        /// <returns>A new text string with the formatting codes removed.</returns>
        public string GetUnformattedString(string formattedString)
        {
            bool oldValue = _parseFormatting;
            _parseFormatting = false;
            Paragraph paragraph = FormatString(formattedString, false);
            _parseFormatting = oldValue;
            return new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text.Trim();
        }

        private char ConsumeCharacter()
        {
            if (_index >= _formattedString.Length)
                return ' ';
            return _formattedString[_index++];
        }

        private char CheckCharacter()
        {
            if (_index >= _formattedString.Length)
                return ' ';
            return _formattedString[_index];
        }

        private void AddAttributes()
        {
            while (true)
            {
                AttributeType attribute = AttributeTypeFactory.GetAttribute(CheckCharacter());
                if (attribute == AttributeType.None)
                    return;
                if (attribute != AttributeType.KeepExistingAttributes && !_attributes.Contains(attribute))
                    _attributes.Add(attribute);
                ConsumeCharacter();
            }
        }

        private void AddAttribute()
        {
            char attributeChar = CheckCharacter();
            AttributeType attribute = AttributeTypeFactory.GetAttribute(attributeChar);
            if (attribute == AttributeType.None)
                return; // Not actually a formatting character so don't consume anything.

            if (attribute != AttributeType.KeepExistingAttributes && !_attributes.Contains(attribute))
            {
                CreateInline();
                _attributes.Add(attribute);
            }
            ConsumeCharacter();
        }

        private void RemoveAttribute()
        {
            char attributeChar = CheckCharacter();
            AttributeType attribute = AttributeTypeFactory.GetAttribute(attributeChar);
            if (attribute == AttributeType.None)
                return; // Not actually a formatting character so don't consume anything.

            if (attribute != AttributeType.KeepExistingAttributes && _attributes.Contains(attribute))
            {
                CreateInline();
                _attributes.Remove(attribute);
            }
            ConsumeCharacter();
        }

        private void ResetFormatting()
        {
            ResetColors();
            _attributes = new List<AttributeType>();
        }

        private void ResetColors()
        {
            CreateInline();
            _foreColor = -1;
            _backColor = -1;
        }

        private IEnumerable<Inline> HandleFormatting(string formattedString)
        {
            _formattedString = formattedString;
            _index = 0;
            _inlines = new List<Inline>();
            _messagePart = string.Empty;

            ResetFormatting();

            char msgChar;
            while (_index < _formattedString.Length)
            {
                msgChar = ConsumeCharacter();

                switch (msgChar)
                {
                    case '\u001c':
                        ResetFormatting();
                        break;
                    case '\u001a':
                        AddAttribute();
                        break;
                    case '\u001b':
                        RemoveAttribute();
                        break;
                    case '\u0019':
                        msgChar = CheckCharacter();
                        switch (msgChar)
                        {
                            case 'b': // Bar stuff, ignore it but consume the next character since it's a formatting character.
                                ConsumeCharacter();
                                break;
                            case 'E': // Emphasize text, ignore it but consume the next character since it's a formatting character.
                                ConsumeCharacter();
                                break;
                            case 'F': // Foreground color only.
                            case '*': // Foreground color and a background color after the comma or tilde.
                                ConsumeCharacter();
                                SetColor(AttributeType.ForeColor);

                                char nextChar = CheckCharacter();
                                if (msgChar == 'F' || nextChar != ',' && nextChar != '~')
                                    break;
                                goto case 'B';
                            case 'B': // Background color.
                                ConsumeCharacter();
                                SetColor(AttributeType.BackColor);
                                break;
                            case '\u001c': // Reset colors, keep other attributes.
                                ResetColors();
                                break;
                            default: // Option color.
                                SetOptionColor();
                                break;
                        }
                        break;
                    default:
                        _messagePart += msgChar;
                        break;
                }
            }

            CreateInline();
            return _inlines;
        }

        private void SetOptionColor()
        {
            CreateInline();
            _backColor = -1;

            string colorCode = ConsumeCharacters(2);
            bool convertResult = int.TryParse(colorCode, out int color);
            _foreColor = convertResult ? _optionParser.GetOptionColor(color) : -1;
        }

        private string ConsumeCharacters(int numberOfCharacters)
        {
            string value = string.Empty;
            for (int i = 0; i < numberOfCharacters; i++)
                value += ConsumeCharacter();

            return value;
        }

        private void SetColor(AttributeType type)
        {
            if (type != AttributeType.BackColor && type != AttributeType.ForeColor)
                return;

            CreateInline();

            bool isExtended = CheckCharacter() == '@';
            if (isExtended)
                ConsumeCharacter();

            if (type == AttributeType.ForeColor) // Foreground attribute may be followed by 0 or more other formatting attributes.
                AddAttributes();

            string colorValue = ConsumeCharacters(isExtended ? 5 : 2); // Extended colors are 5 characters long, basic ones are 2.
            bool convertResult = int.TryParse(colorValue, out int color);
            if (!convertResult)
                color = -1;

            if (type == AttributeType.ForeColor)
                _foreColor = color;
            else
                _backColor = color;
        }

        private void CreateInline()
        {
            if (string.IsNullOrEmpty(_messagePart))
                return;

            Inline inline = new Run(_messagePart);

            if (_parseFormatting)
            {
                if (_attributes.Contains(AttributeType.Bold))
                    inline = new Bold(inline);
                if (_attributes.Contains(AttributeType.Italic))
                    inline = new Italic(inline);
                if (_attributes.Contains(AttributeType.Underline))
                    inline = new Underline(inline);

                Color color = GetColor(_foreColor);
                if (color != default)
                    inline.Foreground = new SolidColorBrush() { Color = color };

                color = GetColor(_backColor);
                if (color != default)
                    inline.Background = new SolidColorBrush() { Color = color };
            }

            if (_attributes.Contains(AttributeType.Hyperlink) && IsValidUri())
            {
                Hyperlink link = new(inline) { NavigateUri = new Uri(_messagePart) };
                link.RequestNavigate += Link_RequestNavigate;
                inline = link;
            }

            _messagePart = string.Empty;
            _inlines.Add(inline);
        }

        private void Link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessUtils.StartProcess(e.Uri.ToString());
            e.Handled = true;
        }

        private bool IsValidUri()
        {
            try
            {
                _ = new Uri(_messagePart);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Color GetColor(int colorCode)
        {
            if (_colorHelper.IsExtendedColor(colorCode))
                return _colorHelper.GetExtendedColor(colorCode);
            return _colorHelper.GetWeechatColor(colorCode);
        }

        private string ParseUrls(string message)
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;

            return _urlRegex.Replace(message, "\u001a\u0010$1\u001b\u0010");
        }
    }
}
