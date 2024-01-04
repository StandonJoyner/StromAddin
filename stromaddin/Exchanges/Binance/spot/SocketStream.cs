using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* 项目“stromaddin (net472)”的未合并的更改
在此之前:
using System.Threading.Tasks;
在此之后:
using System.Threading.Tasks;
using stromaddin;
using stromaddin.Exchanges;
using stromaddin.Exchanges.Binance;
using stromaddin.Exchanges.Binance.spot;
*/
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace stromaddin.Exchanges.Binance.spot
{
    public class SocketStream : SocketClient
    {
        private Task _processTask = null;
        private bool _stopRequested = false;

        private int _maxStreams = 200;
        // lock
        private object _subLock = new object();
        private readonly AsyncResetEvent _subEvent = new AsyncResetEvent();

        private HashSet<string> _subscribedStreams = new HashSet<string>();
        private HashSet<string> _waitingSubscriptions = new HashSet<string>();
        private HashSet<string> _waitingUnsubscriptions = new HashSet<string>();

        private readonly Dictionary<string, Action<string, JToken>> _handlerMap = new Dictionary<string, Action<string, JToken>>();

        public SocketStream(ILogger log, WebSocketParameters param, int maxStreams)
            : base(log, param)
        {
            _maxStreams = maxStreams;
        }

        private int Count()
        {
            lock (_subLock)
            {
                return _subscribedStreams.Count
                    + _waitingSubscriptions.Count
                    - _waitingUnsubscriptions.Count;
            }
        }
        public int Remaining()
        {
            return _maxStreams - Count();
        }

        private int _count
        {
            get => _subscribedStreams.Count
                + _waitingSubscriptions.Count
                - _waitingUnsubscriptions.Count;
        }

        public void RegisterHandler(string type, Action<string, JToken> handler)
        {
            _handlerMap[type] = handler;
        }

        public bool IsSubscribed(string stream)
        {
            lock (_subLock)
            {
                if ((_subscribedStreams.Contains(stream)
                    || _waitingSubscriptions.Contains(stream))
                    && !_waitingUnsubscriptions.Contains(stream))
                    return true;
                else
                    return false;
            }
        }
        public bool IsRealSubscribed(string stream)
        {
            lock (_subLock)
            {
                if (_subscribedStreams.Contains(stream))
                    return true;
                else
                    return false;
            }
        }

        public void Subscribe(string stream)
        {
            // lock
            lock (_subLock)
            {
                if (_count >= _maxStreams)
                    throw new ArgumentException($"Max subscriptions per connection is {_maxStreams}");

                if (_waitingUnsubscriptions.Contains(stream))
                    _waitingUnsubscriptions.Remove(stream);
                if (_subscribedStreams.Contains(stream))
                    return;
                _waitingSubscriptions.Add(stream);

                if (_processTask == null || _processTask.IsCompleted)
                    _processTask = ProcessAsync();

                _subEvent.Set();
            }
        }

        public void Unsubscribe(string symbol)
        {
            lock (_subLock)
            {
                if (_waitingSubscriptions.Contains(symbol))
                    _waitingSubscriptions.Remove(symbol);
                if (!_subscribedStreams.Contains(symbol))
                    return;
                _waitingUnsubscriptions.Add(symbol);

                _subEvent.Set();
            }
        }

        protected override bool HandleJData(JToken jdata)
        {
            if (jdata.Type != JTokenType.Object)
                return false;

            var jstream = jdata["stream"];
            if (jstream == null)
                return false;
            var stream = jstream.ToString();
            var type = stream.Split('@')[1];

            if (_handlerMap.TryGetValue(type, out var handler))
                handler(stream, jdata["data"]);
            // 
            return true;
        }

        /// <inheritdoc />
        private async Task ProcessAsync()
        {
            var delay = TimeSpan.FromMilliseconds(200);
            var nextSendTime = DateTime.UtcNow + delay;

            while (!_stopRequested)
            {
                if (Status == SocketConnection.SocketStatus.None)
                    await ConnectAsync();

                var now = DateTime.UtcNow;
                if (now < nextSendTime)
                {
                    await Task.Delay(nextSendTime - now);
                }
                nextSendTime = now + delay;

                _logger.Log(LogLevel.Debug, $"Socket {SocketId} starting processing subscribe task");

                await _subEvent.WaitAsync().ConfigureAwait(false);
                lock (_subLock)
                {
                    var req = GetUnsubscribeRequest();
                    if (req.Length > 0)
                        Send(ExchangeHelpers.NextId(), req, 1);

                    req = GetSubscribeRequest();
                    if (req.Length > 0)
                        Send(ExchangeHelpers.NextId(), req, 1);

                    CombineStreams();
                }
            }
        }

        private string GetSubscribeRequest()
        {
            if (_waitingSubscriptions.Count == 0)
                return "";
            var topics = _waitingSubscriptions.ToList();

            var request = new BinanceSocketRequest
            {
                Method = "SUBSCRIBE",
                Params = topics.ToArray(),
                Id = ExchangeHelpers.NextId()
            };
            return JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings());
        }

        private string GetUnsubscribeRequest()
        {
            if (_waitingUnsubscriptions.Count == 0)
                return "";
            var topics = _waitingUnsubscriptions.ToList();

            var request = new BinanceSocketRequest
            {
                Method = "UNSUBSCRIBE",
                Params = topics.ToArray(),
                Id = ExchangeHelpers.NextId()
            };
            return JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings());
        }

        private void CombineStreams()
        {
            _subscribedStreams.UnionWith(_waitingSubscriptions);
            _subscribedStreams.ExceptWith(_waitingUnsubscriptions);

            _waitingUnsubscriptions.Clear();
            _waitingSubscriptions.Clear();
        }
    }

    internal class BinanceSocketMessage
    {
        [JsonProperty("method")]
        public string Method { get; set; } = "";

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    internal class BinanceSocketRequest : BinanceSocketMessage
    {
        [JsonProperty("params")]
        public string[] Params { get; set; } = Array.Empty<string>();
    }
}
