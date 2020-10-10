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
        private bool _isManualSelectionChange;
        private bool _isClearingSelection;

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
            RelayBuffer oldValue = (RelayBuffer)e.OldValue;
            if (oldValue != null && !((RelayConnection)DataContext).Buffers.Contains(oldValue))
            {
                ClearSelection();
                _isClearingSelection = true;
            }
            else if (_isManualSelectionChange)
            {
                SelectionChanged?.Invoke(this, EventArgs.Empty);
                _isManualSelectionChange = false;
            }
            else if (_isClearingSelection)
            {
                ClearSelection();
                _isClearingSelection = false;
            }
        }

        private void _bufferTreeView_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DependencyObject source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);
            source = source as TreeViewItem;
            if (source != null)
                _isManualSelectionChange = true;
        }
    }
}
