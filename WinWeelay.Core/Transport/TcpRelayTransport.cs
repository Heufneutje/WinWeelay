﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Plain TCP connection without SSL.
    /// </summary>
    public class TcpRelayTransport : BaseRelayTransport
    {
        private BackgroundWorker _inputWorker;

        /// <summary>
        /// The TCP client for the relay connection.
        /// </summary>
        protected TcpClient _tcpClient;

        /// <summary>
        /// The TCP stream for the relay.
        /// </summary>
        protected Stream _networkStream;

        /// <summary>
        /// Connect to a WeeChat instance with the given configuration.
        /// </summary>
        /// <param name="configuration">Main configuration.</param>
        /// <returns>Async task.</returns>
        public override async Task Connect(RelayConfiguration configuration)
        {
            _configuration = configuration;
            _tcpClient = new TcpClient();

            try
            {
                await _tcpClient.ConnectAsync(_configuration.Hostname, _configuration.Port);
                await InitializeStream();

                IsConnected = true;

                _inputWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
                _inputWorker.DoWork += InputWorker_DoWork;
                _inputWorker.ProgressChanged += InputWorker_ProgressChanged;
                _inputWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _networkStream?.Dispose();
                _tcpClient?.Dispose();
                _tcpClient = null;
                OnErrorReceived(ex);
            }
        }

        /// <summary>
        /// Disconnect from the WeeChat instance.
        /// </summary>
        public override void Disconnect()
        {
            IsConnected = false;
            _inputWorker?.CancelAsync();
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
            _tcpClient = null;
        }

        /// <summary>
        /// Write raw data to the relay connection.
        /// </summary>
        /// <param name="data">The data array to write.</param>
        public override void Write(byte[] data)
        {
            _networkStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Initialize the network stream.
        /// </summary>
        /// <returns>Async task.</returns>
        protected virtual Task InitializeStream()
        {
            _networkStream = _tcpClient.GetStream();
            return Task.FromResult(default(object));
        }

        private void InputWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!(e.UserState is byte[]))
                return;

            RelayMessage relayMessage = new RelayMessage((byte[])e.UserState);
            OnRelayMessageReceived(relayMessage);
        }

        private void InputWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BufferedStream reader;
            try
            {
                reader = new BufferedStream(_networkStream);
            }
            catch (Exception ex)
            {
                if (IsConnected)
                    OnErrorReceived(ex);
                return;
            }

            while (true)
            {
                try
                {
                    List<byte> bytes = new List<byte>();

                    byte[] buffer = new byte[4];
                    int read = reader.Read(buffer, 0, buffer.Length);

                    if (read == 4)
                    {
                        byte[] lengthBytes = buffer.CopyOfRange(0, 4);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(lengthBytes);

                        int length = BitConverter.ToInt32(lengthBytes, 0);
                        bytes.AddRange(buffer);

                        length -= 4;
                        while (length > 0)
                        {
                            bytes.Add((byte)reader.ReadByte());
                            length -= 1;
                        }

                        _inputWorker.ReportProgress(0, bytes.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    if (IsConnected)
                        OnErrorReceived(ex);
                    break;
                }

                if (_inputWorker.CancellationPending)
                {
                    reader.Dispose();
                    break;
                }
            }
        }
    }
}
