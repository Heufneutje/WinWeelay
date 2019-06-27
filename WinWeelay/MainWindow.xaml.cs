using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DockingManagerLayoutHelper.RestoreLayout(_dockingManager);
            GetPanel("buffers").Hiding += BuffersLayoutAnchorable_Hiding;
            GetPanel("nicklist").Hiding += NicklistLayoutAnchorable_Hiding;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ((BufferViewModel)DataContext).DisconnectCommand.Execute(null);
            DockingManagerLayoutHelper.SaveLayout(_dockingManager);
        }

        private void BuffersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RelayBuffer buffer = (RelayBuffer)_buffersListBox.SelectedItem;
            if (buffer == null)
                return;

            LayoutDocument layoutDocument;
            if (!_bufferControls.ContainsKey(buffer))
            {
                BufferControl bufferControl = new BufferControl(buffer);
                layoutDocument = new LayoutDocument
                {
                    Title = buffer.Name,
                    FloatingHeight = 400,
                    FloatingWidth = 500,
                    Content = bufferControl,
                    CanClose = true
                };

                LayoutDocumentPane documentPane = _dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().SingleOrDefault();
                documentPane.Children.Add(layoutDocument);
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

        private void NicklistLayoutAnchorable_Hiding(object sender, CancelEventArgs e)
        {
            _nicklistMenuItem.IsChecked = false;
        }

        private void BuffersLayoutAnchorable_Hiding(object sender, CancelEventArgs e)
        {
            _buffersMenuItem.IsChecked = false;
        }

        private void NicklistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable nicklistLayoutAnchorable = GetPanel("nicklist");
            if (nicklistLayoutAnchorable == null)
                return;

            if (_nicklistMenuItem.IsChecked)
                nicklistLayoutAnchorable.Show();
            else
                nicklistLayoutAnchorable.Hide();
        }

        private void BuffersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable buffersLayoutAnchorable = GetPanel("buffers");
            if (buffersLayoutAnchorable == null)
                return;

            if (_buffersMenuItem.IsChecked)
                buffersLayoutAnchorable.Show();
            else
                buffersLayoutAnchorable.Hide();
        }

        private LayoutAnchorable GetPanel(string contentID)
        {
            return _dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().SingleOrDefault(x => x.ContentId == contentID);
        }
    }
}
