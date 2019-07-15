﻿using System;
using System.Windows.Controls;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for BufferListControl.xaml
    /// </summary>
    public partial class BufferListControl : UserControl, IBufferView
    {
        public event EventHandler SelectionChanged;

        public BufferListControl(RelayConnection connection)
        {
            InitializeComponent();
            DataContext = connection;
        }

        public void ClearSelection()
        {
            _bufferListBox.SelectedItem = null;
        }

        public RelayBuffer GetSelectedBuffer()
        {
            return (RelayBuffer)_bufferListBox.SelectedItem;
        }

        public void SelectBuffer(RelayBuffer buffer)
        {
            _bufferListBox.SelectedItem = buffer;
        }

        private void BufferListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}