using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using static CryptoExchange.Net.Sockets.SocketConnection;

namespace stromaddin.Exchanges.Binance
{
    public class SocketClient
    {
        /// <summary>
        /// Connection lost event
        /// </summary>
        public event Action ConnectionLost = null;

        /// <summary>
        /// Connection closed and no reconnect is happening
        /// </summary>
        public event Action ConnectionClosed = null;

        /// <summary>
        /// Connecting restored event
        /// </summary>
        public event Action<TimeSpan> ConnectionRestored = null;

        private static DateTime _lastConnectTime = DateTime.MinValue;
        /// <summary>
        /// Status of the socket connection
        /// </summary>
        public SocketStatus Status
        {
            get => _status;
            private set
            {
                if (_status == value)
                    return;
                var oldStatus = _status;
                _status = value;
                _logger.Log(LogLevel.Debug, $"Socket {SocketId} status changed from {oldStatus} to {_status}");
            }
        }
        private SocketStatus _status;
        /// <summary>
        /// If connection is made
        /// </summary>
        public bool Connected => _socket.IsOpen;
        /// <summary>
        /// Time of disconnecting
        /// </summary>
        public DateTime? DisconnectTime { get; set; }
        /// <summary>
        /// The unique ID of the socket
        /// </summary>
        public int SocketId => _socket.Id;

        protected ILogger _logger;
        private IWebsocket _socket;

        public SocketClient(ILogger log, WebSocketParameters parameters)
        {
            _logger = log;
            _socket = new CryptoExchangeWebSocketClient(log, parameters);
            _socket.OnMessage += HandleMessage;
            _socket.OnRequestSent += HandleRequestSent;
            _socket.OnOpen += HandleOpen;
            _socket.OnClose += HandleClose;
            _socket.OnReconnecting += HandleReconnecting;
            _socket.OnReconnected += HandleReconnected;
            _socket.OnError += HandleError;
            // _socket.GetReconnectionUrl = GetReconnectionUrlAsync;
        }

        public async Task<bool> ConnectAsync()
        {
            return await _socket.ConnectAsync();
        }

        public void Send(int id, string data, int weight)
        {
            _socket.Send(id, data, weight);
        }

        public async Task CloseAsync()
        {
            await _socket.CloseAsync();
        }

        /// <summary>
        /// Process a message received by the socket
        /// </summary>
        /// <param name="data">The received data</param>
        protected virtual void HandleMessage(string data)
        {
            var timestamp = DateTime.UtcNow;
            _logger.Log(LogLevel.Trace, $"Socket {SocketId} received data: " + data);
            if (string.IsNullOrEmpty(data)) return;

            var tokenData = data.ToJToken(_logger);
            if (tokenData == null)
            {
                data = $"\"{data}\"";
                tokenData = data.ToJToken(_logger);
                if (tokenData == null)
                    return;
            }
            HandleJData(tokenData);
        }

        protected virtual bool HandleJData(JToken jdata)
        {
            return false;
        }

        /// <summary>
        /// Handler for a socket opening
        /// </summary>
        protected virtual void HandleOpen()
        {
            Status = SocketStatus.Connected;
        }

        /// <summary>
        /// Handler for a socket closing without reconnect
        /// </summary>
        protected virtual void HandleClose()
        {
            Status = SocketStatus.Closed;
            Task.Run(() => ConnectionClosed?.Invoke());
        }

        /// <summary>
        /// Handler for a socket losing conenction and starting reconnect
        /// </summary>
        protected virtual void HandleReconnecting()
        {
            Status = SocketStatus.Reconnecting;
            DisconnectTime = DateTime.UtcNow;

            _ = Task.Run(() => ConnectionLost?.Invoke());
        }
        /// <summary>
        /// Handler for a socket which has reconnected
        /// </summary>
        protected virtual async void HandleReconnected()
        {
            Status = SocketStatus.Resubscribing;

            var reconnectSuccessful = await ProcessReconnectAsync().ConfigureAwait(false);
            if (!reconnectSuccessful)
            {
                _logger.Log(LogLevel.Warning, $"Socket {SocketId} Failed reconnect processing: {reconnectSuccessful.Error}, reconnecting again");
                await _socket.ReconnectAsync().ConfigureAwait(false);
            }
            else
            {
                Status = SocketStatus.Connected;
                _ = Task.Run(() =>
                {
                    ConnectionRestored?.Invoke(DateTime.UtcNow - DisconnectTime.Value);
                    DisconnectTime = null;
                });
            }
        }
        private async Task<CallResult<bool>> ProcessReconnectAsync()
        {
            if (!_socket.IsOpen)
                return new CallResult<bool>(new WebError("Socket not connected"));
            _logger.Log(LogLevel.Error, $"Socket {SocketId} ProcessReconnectAsync");
            await Task.Delay(0);

            return new CallResult<bool>(false);
        }

        /// <summary>
        /// Handler for an error on a websocket
        /// </summary>
        /// <param name="e">The exception</param>
        protected virtual void HandleError(Exception e)
        {
            if (e is WebSocketException wse)
                _logger.Log(LogLevel.Warning, $"Socket {SocketId} error: Websocket error code {wse.WebSocketErrorCode}, details: " + e.ToLogString());
            else
                _logger.Log(LogLevel.Warning, $"Socket {SocketId} error: " + e.ToLogString());
        }

        /// <summary>
        /// Handler for whenever a request is sent over the websocket
        /// </summary>
        /// <param name="requestId">Id of the request sent</param>
        protected virtual void HandleRequestSent(int requestId)
        {
        }
    }
}
