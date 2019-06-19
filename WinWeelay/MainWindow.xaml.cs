using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WinWeelay.Core;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace WinWeelay
{
    public partial class MainWindow : Window, IBufferView
    {
        private Dictionary<RelayBuffer, LayoutDocument> _bufferControls;
        
        public MainWindow()
        {
            InitializeComponent();
            _bufferControls = new Dictionary<RelayBuffer, LayoutDocument>();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ((BufferViewModel)DataContext).DisconnectCommand.Execute(null);
        }

        private void BuffersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RelayBuffer buffer = (RelayBuffer)_buffersListBox.SelectedItem;
            if (buffer == null)
                return;

            LayoutDocument layoutDocument;
            if (!_bufferControls.ContainsKey(buffer))
            {
                BufferControl bufferControl = new BufferControl(((BufferViewModel)DataContext).Connection, buffer);
                layoutDocument = new LayoutDocument
                {
                    Title = buffer.Name,
                    FloatingHeight = 400,
                    FloatingWidth = 500,
                    Content = bufferControl,
                    CanClose = true
                };

                _documentPane.Children.Add(layoutDocument);
                _bufferControls.Add(buffer, layoutDocument);
            }
            else
                layoutDocument = _bufferControls[buffer];

            buffer.HandleSelected();
            ((BufferViewModel)DataContext).Connection.NotifyNicklistUpdated();

            layoutDocument.IsActive = true;
            ((BufferViewModel)DataContext).UpdateBufferCommands();
        }

        private void DockingManager_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            RelayBuffer buffer = _bufferControls.FirstOrDefault(x => x.Value == e.Document).Key;
            if (buffer != null)
                _bufferControls.Remove(buffer);

            if (_buffersListBox.SelectedItem == buffer)
            {
                _buffersListBox.SelectedItem = null;
                buffer.HandleUnselected();
            }

            ((BufferViewModel)DataContext).UpdateBufferCommands();
        }

        private void DockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            if (_dockingManager.ActiveContent is BufferControl)
                _buffersListBox.SelectedItem = ((BufferControl)_dockingManager.ActiveContent).Buffer;
        }

        private void NicklistListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RelayNicklistEntry nicklistEntry = (RelayNicklistEntry)GetElementFromPoint(_nicklistListBox, e.GetPosition(_nicklistListBox));
            if (nicklistEntry != null)
                ((BufferViewModel)DataContext).Connection.OutputHandler.Input(nicklistEntry.Buffer, $@"/query {nicklistEntry.Name}");
        }

        private object GetElementFromPoint(ListBox box, Point point)
        {
            UIElement element = (UIElement)box.InputHitTest(point);
            while (true)
            {
                if (element == box)
                    return null;

                object item = box.ItemContainerGenerator.ItemFromContainer(element);
                bool itemFound = !(item.Equals(DependencyProperty.UnsetValue));

                if (itemFound)
                    return item;

                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
        }

        public void CloseBuffer(RelayBuffer buffer)
        {
            if (_bufferControls.ContainsKey(buffer))
                _bufferControls[buffer].Close();
        }

        private void LayoutAnchorable_Hiding(object sender, CancelEventArgs e)
        {
            _nicklistMenuItem.IsChecked = false;
        }

        private void BuffersLayoutAnchorable_Hiding(object sender, CancelEventArgs e)
        {
            _buffersMenuItem.IsChecked = false;
        }

        private void NicklistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_nicklistLayoutAnchorable == null)
                return;

            if (_nicklistMenuItem.IsChecked)
                _nicklistLayoutAnchorable.Show();
            else
                _nicklistLayoutAnchorable.Hide();
        }

        private void BuffersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_buffersLayoutAnchorable == null)
                return;

            if (_buffersMenuItem.IsChecked)
                _buffersLayoutAnchorable.Show();
            else
                _buffersLayoutAnchorable.Hide();
        }
    }
}
