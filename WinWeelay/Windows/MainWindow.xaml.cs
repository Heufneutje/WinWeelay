﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AvalonDock;
using AvalonDock.Layout;
using MWindowLib;
using WinWeelay.Core;

namespace WinWeelay
{
    /// <summary>
    /// Main window of the application.
    /// </summary>
    public partial class MainWindow : MetroWindow, IBufferWindow
    {
        private readonly Dictionary<RelayBuffer, LayoutDocument> _bufferControls;
        private SpellingManager _spellingManager;
        private bool _isManualSelection;
        private IBufferDockView _bufferControl;

        /// <summary>
        /// Create a new instance of the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _bufferControls = new Dictionary<RelayBuffer, LayoutDocument>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dockingManager.RestoreLayout();
            GetPanel("buffers").Hiding += BuffersLayoutAnchorable_Hiding;
            GetPanel("nicklist").Hiding += NicklistLayoutAnchorable_Hiding;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MainViewModel vm = (MainViewModel)DataContext;
            vm.DisconnectCommand.Execute(null);
            vm.SaveWindowSize();
            vm.SaveOptionCache();
            _dockingManager.SaveLayout();
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
                BufferContentControl bufferControl = new(buffer, _spellingManager);
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

            LayoutContent selectedContent = _dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().SingleOrDefault().SelectedContent;
            if (selectedContent != null)
                selectedContent.IsActive = true; // Fixes updating of the selected buffer.

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

        /// <summary>
        /// Remove the UI dock for a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer to close.</param>
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

        /// <summary>
        /// Update the font in all open buffer tabs.
        /// </summary>
        public void UpdateFont()
        {
            foreach (LayoutDocument document in _bufferControls.Values)
            {
                BufferContentControl bufferControl = (BufferContentControl)document.Content;
                bufferControl.UpdateFont();
            }
        }

        /// <summary>
        /// Clear all history entries and set the configured font for all open buffer tabs.
        /// </summary>
        public void ReinitializeInputBoxes()
        {
            foreach (LayoutDocument document in _bufferControls.Values)
            {
                BufferContentControl bufferControl = (BufferContentControl)document.Content;
                bufferControl.ReinitializeInputBox();
            }
        }

        /// <summary>
        /// Update the formattion in all open buffer tabs.
        /// </summary>
        public void UpdateFormattingSettings()
        {
            foreach (LayoutDocument document in _bufferControls.Values)
            {
                BufferContentControl bufferControl = (BufferContentControl)document.Content;
                bufferControl.UpdateTitle();
            }
        }

        /// <summary>
        /// Change the visibility of the progress bar.
        /// </summary>
        /// <param name="visible">Whether the progress bar should be visible.</param>
        public void SetProgressBarVisibility(bool visible)
        {
            _progressBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Set the progress percentage of the progress bar.
        /// </summary>
        /// <param name="progressPercentage">The percentage.</param>
        public void SetProgress(int progressPercentage)
        {
            _progressBar.Value = progressPercentage;
        }

        /// <summary>
        /// Set the spelling manager helper.
        /// </summary>
        /// <param name="spellingManager">The spelling manager helper.</param>
        public void SetSpellingManager(SpellingManager spellingManager)
        {
            _spellingManager = spellingManager;
        }

        /// <summary>
        /// Enable or disable the spell checker.
        /// </summary>
        /// <param name="isEnabled">Whether the spell checker should be enabled.</param>
        public void ToggleSpellChecker(bool isEnabled)
        {
            _spellingManager.IsEnabled = isEnabled;
            _spellingManager.SetDictionaryPaths();
        }

        /// <summary>
        /// Change the buffer view.
        /// </summary>
        /// <param name="bufferControl">The control to display.</param>
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
    }
}
