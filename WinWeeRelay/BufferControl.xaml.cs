using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using WinWeeRelay.Core;

namespace WinWeeRelay
{
    /// <summary>
    /// Interaction logic for BufferControl.xaml
    /// </summary>
    public partial class BufferControl : UserControl
    {
        private RelayConnection _connection;
        public RelayBuffer Buffer { get; private set; }

        public BufferControl(RelayConnection connection, RelayBuffer buffer)
        {
            _connection = connection;
            Buffer = buffer;

            InitializeComponent();

            DataContext = buffer;
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || string.IsNullOrEmpty(_messageTextBox.Text))
                return;

            _connection.OutputHandler.Input(Buffer, _messageTextBox.Text);
            _messageTextBox.Text = string.Empty;
        }

        private void ConversationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChange change = e.Changes.FirstOrDefault();
            int addedLength = (change?.AddedLength) ?? 0;

            bool scrollToEnd = _conversationTextBox.CaretIndex >= _conversationTextBox.Text.Length - addedLength;
            if (scrollToEnd)
            {
                _conversationTextBox.CaretIndex = _conversationTextBox.Text.Length;
                _conversationTextBox.ScrollToEnd();
            }
        }
    }
}
