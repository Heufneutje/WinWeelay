using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for BufferControl.xaml
    /// </summary>
    public partial class BufferControl : UserControl
    {
        private RelayConnection _connection;
        private NickCompleter _nickCompleter;

        public RelayBuffer Buffer { get; private set; }

        public BufferControl(RelayConnection connection, RelayBuffer buffer)
        {
            _connection = connection;
            _nickCompleter = new NickCompleter(buffer);
            Buffer = buffer;

            InitializeComponent();

            DataContext = buffer;
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (!string.IsNullOrEmpty(_messageTextBox.Text))
                    {
                        _connection.OutputHandler.Input(Buffer, _messageTextBox.Text);
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

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_nickCompleter.IsNickCompleting)
                _nickCompleter.Reset();
        }
    }
}
