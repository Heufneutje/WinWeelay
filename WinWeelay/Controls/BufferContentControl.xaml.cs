using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WinWeelay.Configuration;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for BufferContentControl.xaml
    /// </summary>
    public partial class BufferContentControl : UserControl
    {
        private NickCompleter _nickCompleter;
        private MessageHistory _history;
        private FormattingParser _formattingParser;
        private bool _isScrolledToBottom;
        private SpellingManager _spellingManager;

        public RelayBuffer Buffer { get; private set; }

        public BufferContentControl(RelayBuffer buffer, SpellingManager spellingManager)
        {
            _nickCompleter = new NickCompleter(buffer);
            _history = new MessageHistory(buffer.Connection.Configuration);
            _formattingParser = new FormattingParser(buffer.Connection.OptionParser);
            _spellingManager = spellingManager;
            Buffer = buffer;

            InitializeComponent();

            DataContext = buffer;

            Buffer.MessageAdded += Buffer_MessageAdded;
            Buffer.MessagesCleared += Buffer_MessagesCleared;
            Buffer.TitleChanged += Buffer_TitleChanged;

            InitBufferMessages();
            UpdateTitle();
        }

        private void BufferControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isScrolledToBottom)
                _conversationRichTextBox.ScrollToEnd();

            UpdateFont();
            _messageTextBox.Focus();
            _spellingManager.Subscribe(_messageTextBox);
        }

        private void BufferControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _isScrolledToBottom = _conversationRichTextBox.ViewportHeight + _conversationRichTextBox.VerticalOffset == _conversationRichTextBox.ExtentHeight;
            _spellingManager.Unsubscribe(_messageTextBox);
        }

        private void Buffer_MessageAdded(object sender, RelayBufferMessageEventArgs args)
        {
            AddMessage(args.Message, args.AddToEnd, args.IsExpandedBacklog);
        }

        private void Buffer_MessagesCleared(object sender, EventArgs e)
        {
            _conversationDocument.Blocks.Clear();
        }

        private void Buffer_TitleChanged(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (!string.IsNullOrEmpty(_messageTextBox.Text))
                    {
                        Buffer.SendMessage(_messageTextBox.Text);
                        _history.AddHistoryEntry(_messageTextBox.Text);
                        _messageTextBox.Text = string.Empty;
                    }
                    break;
                case Key.Tab:
                    e.Handled = true;
                    _nickCompleter.IsNickCompleting = true;
                    _messageTextBox.Text = _nickCompleter.HandleNickCompletion(_messageTextBox.Text);
                    _messageTextBox.CaretIndex = _messageTextBox.Text.Length;
                    _nickCompleter.IsNickCompleting = false;
                    break;
            }
        }

        private void MessageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    _messageTextBox.Text = _history.GetPreviousHistoryEntry();
                    _messageTextBox.CaretIndex = _messageTextBox.Text.Length;
                    break;
                case Key.Down:
                    _messageTextBox.Text = _history.GetNextHistoryEntry();
                    _messageTextBox.CaretIndex = _messageTextBox.Text.Length;
                    break;
            }
        }

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_nickCompleter.IsNickCompleting)
                _nickCompleter.Reset();
        }

        private void InitBufferMessages()
        {
            if (Buffer.Messages.Any())
            {
                _conversationRichTextBox.BeginChange();
                foreach (RelayBufferMessage message in Buffer.Messages)
                    AddMessage(message, false, false);

                _conversationRichTextBox.EndChange();
            }

            _isScrolledToBottom = true;
        }

        private void AddMessage(RelayBufferMessage message, bool addToEnd, bool isExpandedBacklog)
        {
            bool hasMessages = _conversationDocument.Blocks.Any();
            bool scrollToEnd = !hasMessages || _conversationRichTextBox.ViewportHeight + _conversationRichTextBox.VerticalOffset == _conversationRichTextBox.ExtentHeight;
            Paragraph paragraph = _formattingParser.FormatMessage(message, Buffer.Connection.Configuration.TimestampFormat, Buffer.Connection.Configuration.IsMessageFormattingEnabled);

            if (addToEnd || !hasMessages)
                _conversationDocument.Blocks.Add(paragraph);
            else
                _conversationDocument.Blocks.InsertBefore(_conversationDocument.Blocks.FirstBlock, paragraph);

            if (isExpandedBacklog)
                _conversationRichTextBox.ScrollToHome();
            else if (scrollToEnd)
                _conversationRichTextBox.ScrollToEnd();
        }

        public void UpdateFont()
        {
            FontFamily fontFamily = new FontFamily(Buffer.Connection.Configuration.FontFamily);
            UpdateFlowDocument(_titleDocument, fontFamily);
            UpdateFlowDocument(_conversationDocument, fontFamily);

            _messageTextBox.FontFamily = fontFamily;
            _messageTextBox.FontSize = Buffer.Connection.Configuration.FontSize;
        }

        private void UpdateFlowDocument(FlowDocument document, FontFamily fontFamily)
        {
            document.FontFamily = fontFamily;
            document.FontSize = Buffer.Connection.Configuration.FontSize;
            document.Foreground = new SolidColorBrush()
            {
                // The themes automatically set the text color to gray because editing is not enabled, which is not ideal. Override this behavior.
                Color = Buffer.Connection.Configuration.Theme == Themes.Dark ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0)
            };

            foreach (Block block in document.Blocks)
            {
                block.FontFamily = fontFamily;
                block.FontSize = Buffer.Connection.Configuration.FontSize;
            }
        }

        public void UpdateTitle()
        {
            _titleDocument.Blocks.Clear();
            _titleDocument.Blocks.Add(_formattingParser.FormatString(Buffer.Title, Buffer.Connection.Configuration.IsMessageFormattingEnabled));
        }
    }
}
