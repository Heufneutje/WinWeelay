using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinWeelay.Core;

namespace WinWeelay
{
    public partial class BufferTreeControl : UserControl, IBufferDockView
    {
        private bool _isManualSelectionChange;
        private bool _isClearingSelection;
        public event EventHandler SelectionChanged;

        public BufferTreeControl(RelayConnection connection)
        {
            InitializeComponent();
            DataContext = connection;
        }

        public void ClearSelection()
        {
            _bufferTreeView.ClearSelection();
        }

        public RelayBuffer GetSelectedBuffer()
        {
            return (RelayBuffer)_bufferTreeView.SelectedItem;
        }

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
