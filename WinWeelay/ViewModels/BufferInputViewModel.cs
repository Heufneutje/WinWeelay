using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WinWeelay.Configuration;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class BufferInputViewModel : NotifyPropertyChangedBase
    {
        private BufferInputControl _inputControl;
        private NickCompleter _nickCompleter;
        private MessageHistory _history;
        private SpellingManager _spellingManager;
        private IrcMessageBuilder _messageBuilder;

        public Color DefaultColor { get; set; }
        public RelayBuffer Buffer { get; set; }
        public RelayConfiguration RelayConfiguration => Buffer?.Connection?.Configuration;

        public BufferInputViewModel() { }

        public BufferInputViewModel(RelayBuffer buffer, SpellingManager spellingManager, BufferInputControl inputControl)
        {
            Buffer = buffer;
            _inputControl = inputControl;
            _nickCompleter = new NickCompleter(buffer);
            _spellingManager = spellingManager;
            _history = new MessageHistory(RelayConfiguration);
            _messageBuilder = new IrcMessageBuilder();

            SetDefaultColor();
        }

        public void SendMessage(RichTextBox richTextBox)
        {
            Buffer.SendMessage(_messageBuilder.BuildMessage(richTextBox.Document, DefaultColor));
            _history.AddHistoryEntry(richTextBox.GetXaml());
            richTextBox.SetPlainText(string.Empty);
        }

        public void SendMessage(TextBox textBox)
        {
            Buffer.SendMessage(textBox.Text);
            _history.AddHistoryEntry(textBox.Text);
            textBox.Clear();
        }

        public void SubscribeSpellingManager(TextBoxBase textBox)
        {
            _spellingManager.Subscribe(textBox);
        }

        public void UnsubscribeSpellingManager(TextBoxBase textBox)
        {
            _spellingManager.Unsubscribe(textBox);
        }

        public void SetPreviousHistoryEntry(TextBoxBase textBox)
        {
            if (RelayConfiguration.IsFormattingToolbarVisible)
                ((RichTextBox)textBox).SetXaml(_history.GetPreviousHistoryEntry());
            else
                ((TextBox)textBox).Text = _history.GetPreviousHistoryEntry();
            UpdateFont();
        }

        public void SetNextHistoryEntry(TextBoxBase textBox)
        {
            if (RelayConfiguration.IsFormattingToolbarVisible)
                ((RichTextBox)textBox).SetXaml(_history.GetNextHistoryEntry());
            else
                ((TextBox)textBox).Text = _history.GetNextHistoryEntry();
            UpdateFont();
        }

        /// <summary>
        /// Clear all history entries and set the configured font.
        /// </summary>
        public void ReinitializeInputBox()
        {
            _history.ClearHistory();
            UpdateFont();
        }

        public void HandleNickCompletion(TextBoxBase textBox, string text)
        {
            _nickCompleter.IsNickCompleting = true;

            if (RelayConfiguration.IsFormattingToolbarVisible)
                ((RichTextBox)textBox).SetPlainText(_nickCompleter.HandleNickCompletion(text));
            else
                ((TextBox)textBox).Text = _nickCompleter.HandleNickCompletion(text);

            _nickCompleter.IsNickCompleting = false;
        }

        public void ResetNickCompletion()
        {
            if (!_nickCompleter.IsNickCompleting)
                _nickCompleter.Reset();
        }

        public void UpdateFont()
        {
            SetDefaultColor();
            FontFamily fontFamily = new FontFamily(RelayConfiguration.FontFamily);
            _inputControl.UpdateFont(RelayConfiguration.FontSize, fontFamily, DefaultColor);
        }

        private void SetDefaultColor()
        {
            if (App.CurrentTheme == Themes.Dark)
                DefaultColor = Color.FromArgb(255, 255, 255, 254);
            else
                DefaultColor = Color.FromArgb(255, 0, 0, 1);
        }
    }
}
