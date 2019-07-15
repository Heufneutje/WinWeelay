using System;
using System.Windows;
using System.Windows.Controls;
using WinWeelay.Core;

namespace WinWeelay
{
    public partial class BufferTreeControl : UserControl, IBufferControl
    {
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
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
