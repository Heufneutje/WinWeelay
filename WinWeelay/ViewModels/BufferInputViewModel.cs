using System.Windows.Controls;
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

        public void SubscribeSpellingManager(RichTextBox richTextBox)
        {
            _spellingManager.Subscribe(richTextBox);
        }

        public void UnsubscribeSpellingManager(RichTextBox richTextBox)
        {
            _spellingManager.Unsubscribe(richTextBox);
        }

        public void SetPreviousHistoryEntry(RichTextBox richTextBox)
        {
            richTextBox.SetXaml(_history.GetPreviousHistoryEntry());
            UpdateFont();
        }

        public void SetNextHistoryEntry(RichTextBox richTextBox)
        {
            richTextBox.SetXaml(_history.GetNextHistoryEntry());
            UpdateFont();
        }

        public void HandleNickCompletion(RichTextBox richTextBox)
        {
            _nickCompleter.IsNickCompleting = true;
            richTextBox.SetPlainText(_nickCompleter.HandleNickCompletion(richTextBox.GetPlainText()));
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
            NotifyPropertyChanged(nameof(RelayConfiguration));
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
