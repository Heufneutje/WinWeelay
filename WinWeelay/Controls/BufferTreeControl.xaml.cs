using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Control which shows buffers in a tree.
    /// </summary>
    public partial class BufferTreeControl : UserControl, IBufferDockView
    {
        /// <summary>
        /// Event for when the selected buffer changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Initialize the buffer tree control.
        /// </summary>
        /// <param name="connection">Connection to the WeeChat host.</param>
        public BufferTreeControl(RelayConnection connection)
        {
            InitializeComponent();
            DataContext = connection;
        }

        /// <summary>
        /// Deselect the active buffer.
        /// </summary>
        public void ClearSelection()
        {
            _bufferTreeView.ClearSelection();
        }

        /// <summary>
        /// Get the buffer that is currently selected.
        /// </summary>
        /// <returns>The active buffer.</returns>
        public RelayBuffer GetSelectedBuffer()
        {
            return (RelayBuffer)_bufferTreeView.SelectedItem;
        }

        /// <summary>
        /// Select a given buffer as the active buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void SelectBuffer(RelayBuffer buffer)
        {
            _bufferTreeView.SelectItem(buffer);
        }

        private void BufferTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RelayConnection connection = (RelayConnection)DataContext;
            if (!connection.IsRefreshingBuffers)
                SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
