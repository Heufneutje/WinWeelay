using System;
using System.Windows.Controls;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Control which shows buffers in a list.
    /// </summary>
    public partial class BufferListControl : UserControl, IBufferDockView
    {
        /// <summary>
        /// Event for when the selected buffer changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Initialize the buffer list control.
        /// </summary>
        /// <param name="connection">Connection to the WeeChat host.</param>
        public BufferListControl(RelayConnection connection)
        {
            InitializeComponent();
            DataContext = connection;
        }

        /// <summary>
        /// Get the buffer that is currently selected.
        /// </summary>
        /// <returns>The active buffer.</returns>
        public RelayBuffer GetSelectedBuffer()
        {
            return (RelayBuffer)_bufferListBox.SelectedItem;
        }

        private void BufferListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
