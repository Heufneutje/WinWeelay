using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Control which contains the buffer messages and input box.
    /// </summary>
    public partial class BufferContentControl : UserControl
    {
        private SpellingManager _spellingManager;
        private BufferInputViewModel _inputViewModel;
        private FormattingParser _formattingParser;
        private bool _isScrolledToBottom;
        private bool _isInputControlInitialized;

        /// <summary>
        /// The buffer to interact with.
        /// </summary>
        public RelayBuffer Buffer { get; private set; }

        /// <summary>
        /// Create a new control which contains the buffer messages and input box.
        /// </summary>
        /// <param name="buffer">The buffer to interact with.</param>
        /// <param name="spellingManager">Spell checker for the input box.</param>
        public BufferContentControl(RelayBuffer buffer, SpellingManager spellingManager)
        {
            _spellingManager = spellingManager;
            _formattingParser = new FormattingParser(buffer.Connection.OptionParser);
            Buffer = buffer;

            InitializeComponent();

            DataContext = buffer;

            Buffer.MessageAdded += Buffer_MessageAdded;
            Buffer.MessageBatchAdded += Buffer_MessageBatchAdded;
            Buffer.MessagesCleared += Buffer_MessagesCleared;
            Buffer.TitleChanged += Buffer_TitleChanged;

            InitBufferMessages();
            UpdateTitle();
        }

        private void BufferControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isScrolledToBottom)
                _conversationRichTextBox.ScrollToEnd();

            if (!_isInputControlInitialized)
            {
                _inputViewModel = new BufferInputViewModel(Buffer, _spellingManager, _inputControl);
                _inputControl.DataContext = _inputViewModel;
                _isInputControlInitialized = true;
            }
            
            UpdateFont();
            _inputControl.FocusEditor();
        }

        private void BufferControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CheckScrolledToBottom();
        }

        private void Buffer_MessageAdded(object sender, RelayBufferMessageEventArgs args)
        {
            AddMessage(args.Message, args.AddToEnd, args.IsExpandedBacklog, false);
        }

        private void Buffer_MessageBatchAdded(object sender, RelayBufferMessageBatchEventsArgs args)
        {
            _conversationRichTextBox.BeginChange();

            foreach (RelayBufferMessage message in args.Messages)
                AddMessage(message, args.AddToEnd, args.IsExpandedBacklog, true);

            if (args.IsExpandedBacklog)
                _conversationRichTextBox.ScrollToHome();
            else
                _conversationRichTextBox.ScrollToEnd();

            _conversationRichTextBox.EndChange();

            AutoShrinkBuffer();
        }

        private void Buffer_MessagesCleared(object sender, EventArgs e)
        {
            _conversationDocument.Blocks.Clear();
        }

        private void Buffer_TitleChanged(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        private void InitBufferMessages()
        {
            if (Buffer.Messages.Any())
            {
                _conversationRichTextBox.BeginChange();
                foreach (RelayBufferMessage message in Buffer.Messages)
                    AddMessage(message, false, false, true);

                _conversationRichTextBox.EndChange();
            }

            _isScrolledToBottom = true;
        }

        private void AddMessage(RelayBufferMessage message, bool addToEnd, bool isExpandedBacklog, bool isBatchedMessage)
        {
            bool hasMessages = _conversationDocument.Blocks.Any();
            bool scrollToEnd = !hasMessages || _conversationRichTextBox.ViewportHeight + _conversationRichTextBox.VerticalOffset == _conversationRichTextBox.ExtentHeight;
            Paragraph paragraph = _formattingParser.FormatMessage(message, Buffer.Connection.Configuration.TimestampFormat, Buffer.Connection.Configuration.IsMessageFormattingEnabled);

            if (addToEnd || !hasMessages)
                _conversationDocument.Blocks.Add(paragraph);
            else
                _conversationDocument.Blocks.InsertBefore(_conversationDocument.Blocks.FirstBlock, paragraph);

            if (!isBatchedMessage)
            {
                AutoShrinkBuffer();
                if (isExpandedBacklog)
                    _conversationRichTextBox.ScrollToHome();
                else if (scrollToEnd)
                    _conversationRichTextBox.ScrollToEnd();
            }
        }

        private void AutoShrinkBuffer()
        {
            if (Buffer.Connection.Configuration.AutoShrinkBuffer)
            {
                while (_conversationDocument.Blocks.Count > Buffer.MaxBacklogSize)
                    _conversationDocument.Blocks.Remove(_conversationDocument.Blocks.FirstBlock);
            }
        }

        /// <summary>
        /// Change the font on all messages after the font settings have been changed.
        /// </summary>
        public void UpdateFont()
        {
            FontFamily fontFamily = new FontFamily(Buffer.Connection.Configuration.FontFamily);
            _titleDocument.UpdateFont(Buffer.Connection.Configuration.FontSize, fontFamily, true);
            _conversationDocument.UpdateFont(Buffer.Connection.Configuration.FontSize, fontFamily, true);
            _inputViewModel.UpdateFont();
        }

        /// <summary>
        /// Update the title box after the buffer title has been changed in the core.
        /// </summary>
        public void UpdateTitle()
        {
            _titleDocument.Blocks.Clear();
            _titleDocument.Blocks.Add(_formattingParser.FormatString(Buffer.Title, Buffer.Connection.Configuration.IsMessageFormattingEnabled));
        }

        private void CheckScrolledToBottom()
        {
            _isScrolledToBottom = _conversationRichTextBox.ViewportHeight + _conversationRichTextBox.VerticalOffset == _conversationRichTextBox.ExtentHeight;
        }

        /// <summary>
        /// Scroll to the bottom if it was previously scrolled to the bottom when the main window is restored or maximixed.
        /// </summary>
        /// <param name="windowState">The new state of the main window.</param>
        public void HandleWindowStateChange(WindowState windowState)
        {
            CheckScrolledToBottom();
            if ((windowState == WindowState.Maximized || windowState == WindowState.Normal) && _isScrolledToBottom)
                _conversationRichTextBox.ScrollToEnd();
        }

        public void ClearHistory()
        {
            _inputViewModel.ClearHistory();
        }
    }
}
