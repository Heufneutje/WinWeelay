using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WinWeelay.Core;

namespace WinWeelay
{
    public class FormattingHelper
    {
        private ColorHelper _colorHelper;

        private int _index;
        private string _formattedString;
        private string _messagePart;
        private List<Inline> _inlines;
        private List<AttributeType> _attributes;
        private int _foreColor;
        private int _backColor;

        public FormattingHelper()
        {
            _colorHelper = new ColorHelper();
        }

        public Paragraph FormatMessage(RelayBufferMessage message, string timestampFormat)
        {
            string formattedDate = $"[{message.Date.ToString(timestampFormat)}] ";

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(formattedDate));

            string prefix = message.Prefix;
            if (message.MessageType == "privmsg")
                prefix = $"<{prefix}\u0019\u001c>";
            prefix += ' ';

            paragraph.Inlines.AddRange(new FormattingHelper().HandleFormatting(prefix));
            paragraph.Inlines.AddRange(new FormattingHelper().HandleFormatting(message.Message));
            return paragraph;
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

        private AttributeType GetAttribute(char attributeChar)
        {
            switch (attributeChar)
            {
                case '*':
                case '\u0001':
                    return AttributeType.Bold;
                case '!':
                case '\u0002':
                    return AttributeType.Reverse;
                case '/':
                case '\u0003':
                    return AttributeType.Italic;
                case '_':
                case '\u0004':
                    return AttributeType.Underline;
                case '|':
                    return AttributeType.KeepExistingAttributes;
                default:
                    return AttributeType.None;
            }
        }

        private void AddAttributes()
        {
            while (true)
            {
                AttributeType attribute = GetAttribute(CheckCharacter());
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
            AttributeType attribute = GetAttribute(attributeChar);
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
            AttributeType attribute = GetAttribute(attributeChar);
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
                            default:
                                // TODO: Handle weechat option colors. Consumes 2 characters.
                                ConsumeCharacter();
                                ConsumeCharacter();
                                ResetColors();
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

            _messagePart = string.Empty;
            _inlines.Add(inline);
        }

        private Color GetColor(int colorCode)
        {
            if (_colorHelper.IsExtendedColor(colorCode))
                return _colorHelper.GetExtendedColor(colorCode);
            return _colorHelper.GetWeechatColor(colorCode);
        }

        public static List<string> GetInstalledFonts()
        {
            List<string> fonts = new List<string>();
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                System.Drawing.FontFamily[] fontFamilies = fontsCollection.Families;
                foreach (System.Drawing.FontFamily font in fontFamilies.Where(x => !string.IsNullOrEmpty(x.Name)))
                    fonts.Add(font.Name);
            }

            return fonts;
        }
    }
}
