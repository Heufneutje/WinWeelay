using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WinWeelay.Configuration;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// The view model for the input control.
    /// </summary>
    public class BufferInputViewModel : NotifyPropertyChangedBase
    {
        private BufferInputControl _inputControl;
        private NickCompleter _nickCompleter;
        private MessageHistory _history;
        private SpellingManager _spellingManager;
        private IrcMessageBuilder _messageBuilder;

        /// <summary>
        /// The default text color based on the current theme.
        /// </summary>
        public Color DefaultColor { get; set; }

        /// <summary>
        /// The buffer to interact with.
        /// </summary>
        public RelayBuffer Buffer { get; set; }

        /// <summary>
        /// The main configuration loaded from the config file.
        /// </summary>
        public RelayConfiguration RelayConfiguration => Buffer?.Connection?.Configuration;

        /// <summary>
        /// Representation of the current nickname on the server and the currently set usermodes.
        /// </summary>
        public string CurrentNickAndModes
        {
            get
            {
                if (Buffer != null && !string.IsNullOrEmpty(Buffer.IrcServer.CurrentNick))
                    return $"{Buffer.IrcServer.CurrentNick} (+{Buffer.IrcServer.CurrentUserModeString})";
                return null;
            }
        }

        /// <summary>
        /// Whether the nickname and modes should be shown in front of the input box.
        /// </summary>
        public bool CurrentNickAndModesVisible => CurrentNickAndModes != null;

        /// <summary>
        /// Default constructor for designer.
        /// </summary>
        public BufferInputViewModel() { }

        /// <summary>
        /// Create a new view model for the input control.
        /// </summary>
        /// <param name="buffer">The buffer to interact with.</param>
        /// <param name="spellingManager">Spell checker for the input box.</param>
        /// <param name="inputControl">The control containing the input box.</param>
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

        /// <summary>
        /// Send a message to the attached buffer.
        /// </summary>
        /// <param name="richTextBox">The RichTextBox to parse the text from. IRC formatting will be applied.</param>
        public void SendMessage(RichTextBox richTextBox)
        {
            Buffer.SendMessage(_messageBuilder.BuildMessage(richTextBox.Document, DefaultColor));
            _history.AddHistoryEntry(richTextBox.GetXaml());
            richTextBox.SetPlainText(string.Empty);
        }

        /// <summary>
        /// Send a message to the attached buffer.
        /// </summary>
        /// <param name="textBox">The TextBox to parse the text from.</param>
        public void SendMessage(TextBox textBox)
        {
            Buffer.SendMessage(textBox.Text);
            _history.AddHistoryEntry(textBox.Text);
            textBox.Clear();
        }

        /// <summary>
        /// Subscribe a given text box to the spell checker. Enables spell checking, customizes the context menu to show suggestions and updates the spell checker when the custom diciontary is modified.
        /// </summary>
        /// <param name="textBox">The text box to subscribe.</param>
        public void SubscribeSpellingManager(TextBoxBase textBox)
        {
            _spellingManager.Subscribe(textBox);
        }

        /// <summary>
        /// Unsubscribe a given text box from the spell checker.
        /// </summary>
        /// <param name="textBox">The text box to unsubscribe.</param>
        public void UnsubscribeSpellingManager(TextBoxBase textBox)
        {
            _spellingManager.Unsubscribe(textBox);
        }

        /// <summary>
        /// Fill the input box with the message before the current index.
        /// </summary>
        public void SetPreviousHistoryEntry(TextBoxBase textBox)
        {
            if (RelayConfiguration.IsFormattingToolbarVisible)
                ((RichTextBox)textBox).SetXaml(_history.GetPreviousHistoryEntry());
            else
                SetPlainText(textBox as TextBox, _history.GetPreviousHistoryEntry());
            UpdateFont();
        }

        /// <summary>
        /// Fill the input box with the message after the current index.
        /// </summary>
        public void SetNextHistoryEntry(TextBoxBase textBox)
        {
            if (RelayConfiguration.IsFormattingToolbarVisible)
                ((RichTextBox)textBox).SetXaml(_history.GetNextHistoryEntry());
            else
                SetPlainText(textBox as TextBox, _history.GetNextHistoryEntry());
            UpdateFont();
        }

        /// <summary>
        /// Clear all history entries and set the configured font.
        /// </summary>
        public void ReinitializeInputBox()
        {
            _history.ClearHistory();
            UpdateFont();

            if (RelayConfiguration.IsSpellCheckEnabled)
                _inputControl.RefreshSpellCheckSubscription();
        }

        /// <summary>
        /// Try to complete the nickname based on a given test string.
        /// </summary>
        /// <param name="textBox">The textbox to output the result to.</param>
        /// <param name="text">The given text string.</param>
        public void HandleNickCompletion(TextBoxBase textBox, string text)
        {
            _nickCompleter.IsNickCompleting = true;

            if (RelayConfiguration.IsFormattingToolbarVisible)
                ((RichTextBox)textBox).SetPlainText(_nickCompleter.HandleNickCompletion(text));
            else
            {
                TextBox box = textBox as TextBox;
                box.Text = _nickCompleter.HandleNickCompletion(text);
                box.CaretIndex = box.Text.Length;
            }
            _nickCompleter.IsNickCompleting = false;
        }

        /// <summary>
        /// Stop trying to complete the nickname and clear the search.
        /// </summary>
        public void ResetNickCompletion()
        {
            if (!_nickCompleter.IsNickCompleting)
                _nickCompleter.Reset();
        }

        /// <summary>
        /// Update the font settings.
        /// </summary>
        public void UpdateFont()
        {
            SetDefaultColor();
            FontFamily fontFamily = new FontFamily(RelayConfiguration.FontFamily);
            _inputControl.UpdateFont(RelayConfiguration.FontSize, fontFamily, DefaultColor);
        }

        /// <summary>
        /// Update the current nickname and user modes in the UI.
        /// </summary>
        public void UpdateCurrentNickAndModes()
        {
            NotifyPropertyChanged(nameof(CurrentNickAndModes));
        }

        private void SetDefaultColor()
        {
            if (App.CurrentTheme == Themes.Dark)
                DefaultColor = Color.FromArgb(255, 255, 255, 254);
            else
                DefaultColor = Color.FromArgb(255, 0, 0, 1);
        }

        private void SetPlainText(TextBox textBox, string text)
        {
            textBox.Text = text;
            textBox.CaretIndex = textBox.Text.Length;
        }
    }
}
