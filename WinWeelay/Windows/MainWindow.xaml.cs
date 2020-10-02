using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MWindowLib;
using WinWeelay.Core;
using AvalonDock;
using AvalonDock.Layout;

namespace WinWeelay
{
    public partial class MainWindow : MetroWindow, IBufferWindow
    {
        private Dictionary<RelayBuffer, LayoutDocument> _bufferControls;
        private SpellingManager _spellingManager;
        private bool _isManualSelection;
        private IBufferDockView _bufferControl;

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
            MainViewModel vm = (MainViewModel)DataContext;
            vm.DisconnectCommand.Execute(null);
            vm.SaveWindowSize();
            vm.SaveOptionCache();
            DockingManagerLayoutHelper.SaveLayout(_dockingManager);
        }

        private void BufferControl_SelectionChanged(object sender, EventArgs e)
        {
            RelayBuffer buffer = _bufferControl.GetSelectedBuffer();
            if (buffer == null)
                return;

            _isManualSelection = true;
            LayoutDocument layoutDocument;
            if (!_bufferControls.ContainsKey(buffer))
            {
                BufferContentControl bufferControl = new BufferContentControl(buffer, _spellingManager);
                layoutDocument = new LayoutDocument
                {
                    Title = buffer.FullName,
                    FloatingHeight = 400,
                    FloatingWidth = 500,
                    Content = bufferControl,
                    CanClose = true
                };

                LayoutDocumentPane documentPane = _dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().SingleOrDefault();
                documentPane.Children.Add(layoutDocument);
                _bufferControls.Add(buffer, layoutDocument);

                buffer.NameChanged += delegate { layoutDocument.Title = buffer.FullName; };
            }
            else
                layoutDocument = _bufferControls[buffer];

            buffer.HandleSelected();
            ((MainViewModel)DataContext).Connection.NotifyNicklistUpdated();

            layoutDocument.IsActive = true;
            ((MainViewModel)DataContext).UpdateBufferCommands();
            _isManualSelection = false;
        }

        private void NicklistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RelayNicklistEntry nick = (RelayNicklistEntry)_nicklistListBox.SelectedItem;
            ((MainViewModel)DataContext).UpdateActiveNicklistEntry(nick);
        }

        private void DockingManager_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            RelayBuffer buffer = _bufferControls.FirstOrDefault(x => x.Value == e.Document).Key;
            if (buffer != null)
                _bufferControls.Remove(buffer);

            if (_bufferControl.GetSelectedBuffer() == buffer)
            {
                _bufferControl.ClearSelection();
                buffer.HandleUnselected();
            }

            ((MainViewModel)DataContext).UpdateBufferCommands();
        }

        private void DockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            if (_dockingManager.ActiveContent is BufferContentControl && !_isManualSelection)
            {
                RelayBuffer buffer = ((BufferContentControl)_dockingManager.ActiveContent).Buffer;
                buffer.HandleSelected();
                if (_bufferControl.GetSelectedBuffer() != buffer)
                    _bufferControl.SelectBuffer(buffer);
            }
        }

        private void NicklistListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RelayNicklistEntry nicklistEntry = (RelayNicklistEntry)GetElementFromPoint(_nicklistListBox, e.GetPosition(_nicklistListBox));
            if (nicklistEntry != null)
                ((MainViewModel)DataContext).Connection.OutputHandler.Input(nicklistEntry.Buffer, $@"/query {nicklistEntry.Name}");
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

        public void UpdateFont()
        {
            foreach (LayoutDocument document in _bufferControls.Values)
            {
                BufferContentControl bufferControl = (BufferContentControl)document.Content;
                bufferControl.UpdateFont();
            }
        }

        public void UpdateFormattingSettings()
        {
            foreach (LayoutDocument document in _bufferControls.Values)
            {
                BufferContentControl bufferControl = (BufferContentControl)document.Content;
                bufferControl.UpdateTitle();
            }
        }

        public void SetProgressBarVisibility(bool visible)
        {
            _progressBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetProgress(int progressPercentage)
        {
            _progressBar.Value = progressPercentage;
        }

        public void SetSpellingManager(SpellingManager spellingManager)
        {
            _spellingManager = spellingManager;
        }

        public void ToggleSpellChecker(bool isEnabled)
        {
            _spellingManager.IsEnabled = isEnabled;
            _spellingManager.SetDictionaryPaths();
        }

        public void SetBufferControl(IBufferDockView bufferControl)
        {
            _bufferControl = bufferControl;
            _bufferControl.SelectionChanged += BufferControl_SelectionChanged;
            GetPanel("buffers").Content = _bufferControl;
        }

        private void ViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).UpdateViewSettings();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            RelayBuffer activeBuffer = ((MainViewModel)DataContext).Connection.ActiveBuffer;
            if (activeBuffer != null && _bufferControls.ContainsKey(activeBuffer))
            {
                LayoutDocument document = _bufferControls[activeBuffer];
                BufferContentControl bufferControl = (BufferContentControl)document.Content;
                bufferControl.HandleWindowStateChange(WindowState);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).Connection.OutputHandler.RequestOptions("");
        }
    }
}
