using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WinWeelay.Configuration;
using WinWeelay.Core;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace WinWeelay
{
    public partial class MainWindow : Window
    {
        private RelayConnection _connection;
        private Dictionary<RelayBuffer, LayoutDocument> _bufferControls;
        private RelayConfiguration _relayConfiguration;

        public MainWindow()
        {
            InitializeComponent();

            _bufferControls = new Dictionary<RelayBuffer, LayoutDocument>();

            _relayConfiguration = ConfigurationHelper.LoadConfiguration();
            _connection = new RelayConnection(_relayConfiguration.Hostname, _relayConfiguration.Port, _relayConfiguration.DecryptedRelayPassword);

            DataContext = _connection;

            _connection.Connect();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _connection.Disconnect();
        }

        private void BuffersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RelayBuffer buffer = (RelayBuffer)_buffersListBox.SelectedItem;
            if (buffer == null)
                return;

            LayoutDocument layoutDocument;
            if (!_bufferControls.ContainsKey(buffer))
            {
                BufferControl bufferControl = new BufferControl(_connection, buffer);
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

                if (!buffer.HasBacklog)
                    _connection.OutputHandler.RequestBufferBacklog(buffer, _relayConfiguration.BacklogSize);
            }
            else
                layoutDocument = _bufferControls[buffer];

            layoutDocument.IsActive = true;
        }

        private void DockingManager_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            RelayBuffer buffer = _bufferControls.FirstOrDefault(x => x.Value == e.Document).Key;
            if (buffer != null)
                _bufferControls.Remove(buffer);

            if (_buffersListBox.SelectedItem == buffer)
                _buffersListBox.SelectedItem = null;
        }

        private void DockingManager_ActiveContentChanged(object sender, EventArgs e)
        {
            if (_dockingManager.ActiveContent is BufferControl)
                _buffersListBox.SelectedItem = ((BufferControl)_dockingManager.ActiveContent).Buffer;
        }
    }
}
