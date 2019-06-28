using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

#if WINDOWS10_SDK
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
#endif

using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for BufferControl.xaml
    /// </summary>
    public partial class BufferControl : UserControl
    {
        private NickCompleter _nickCompleter;
        private MessageHistory _history;
        private OrderedDictionary<RelayBufferMessage, Block> _blocks;
        private bool _isScrolledToBottom;

        public RelayBuffer Buffer { get; private set; }

        public BufferControl(RelayBuffer buffer)
        {
            _nickCompleter = new NickCompleter(buffer);
            _history = new MessageHistory(buffer.Connection.Configuration);
            _blocks = new OrderedDictionary<RelayBufferMessage, Block>();
            Buffer = buffer;

            InitializeComponent();

            DataContext = buffer;

            Buffer.MessageAdded += Buffer_MessageAdded;
        }

        private void BufferControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isScrolledToBottom)
                _conversationRichTextBox.ScrollToEnd();
        }

        private void BufferControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _isScrolledToBottom = _conversationRichTextBox.ViewportHeight + _conversationRichTextBox.VerticalOffset == _conversationRichTextBox.ExtentHeight;
        }

        private void Buffer_MessageAdded(object sender, RelayBufferMessageEventArgs args)
        {
            bool scrollToEnd = _conversationRichTextBox.ViewportHeight + _conversationRichTextBox.VerticalOffset == _conversationRichTextBox.ExtentHeight;
            Block block = new Paragraph(new Run(args.Message.ToString()));

            if (args.AddToEnd || !_blocks.Any())
                _messageDocument.Blocks.Add(block);
            else
            {
                RelayBufferMessage previousMessage = _blocks.Keys.LastOrDefault(x => x.Date < args.Message.Date);
                if (previousMessage == null)
                    _messageDocument.Blocks.InsertBefore(_messageDocument.Blocks.FirstBlock, block);
                else
                    _messageDocument.Blocks.InsertAfter(_blocks[previousMessage], block);
            }

            _blocks.Add(args.Message, block);

            if (!args.AddToEnd || scrollToEnd)
                _conversationRichTextBox.ScrollToEnd();
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
                    break;
                case Key.Down:
                    _messageTextBox.Text = _history.GetNextHistoryEntry();
                    break;
            }

            _messageTextBox.CaretIndex = _messageTextBox.Text.Length;
        }

        private void ConversationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
//            TextChange change = e.Changes.FirstOrDefault();
//            int addedLength = (change?.AddedLength) ?? 0;

//            bool scrollToEnd = _conversationTextBox.CaretIndex >= _conversationTextBox.Text.Length - addedLength;
//            if (scrollToEnd)
//            {
//                _conversationTextBox.CaretIndex = _conversationTextBox.Text.Length;
//                _conversationTextBox.ScrollToEnd();
//            }

//            foreach (RelayBufferMessage message in Buffer.MessagesToHighlight)
//            {
//                message.IsNotified = true;

//#if WINDOWS10_SDK
//                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
//                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
//                stringElements[0].AppendChild(toastXml.CreateTextNode(Buffer.Name));
//                stringElements[1].AppendChild(toastXml.CreateTextNode(message.ToString().Replace("\u001c", "")));

//                ToastNotification toast = new ToastNotification(toastXml);
//                ToastNotificationManager.CreateToastNotifier("WinWeelay").Show(toast);
//#endif
//            }
        }

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_nickCompleter.IsNickCompleting)
                _nickCompleter.Reset();
        }
    }
}
