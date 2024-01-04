using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ExcelDna.Integration.Rtd.ExcelRtdServer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace stromaddin.Exchanges.Binance.spot
{
    public class MarketStreamManager
    {
        private ILogger _log;
        protected virtual JsonSerializer DefaultSerializer { get; set; } = JsonSerializer.Create(SerializerOptions.Default);

        private readonly List<SocketStream> _marketStreams = new List<SocketStream>();
        // ticker
        private Dictionary<string, IStreamSubscribe> _tickMap = new Dictionary<string, IStreamSubscribe>();
        // kline
        private HashSet<string> _klineIntervals;
        private Dictionary<string, IStreamSubscribe> _klineMap = new Dictionary<string, IStreamSubscribe>();
        // rolling window
        private HashSet<string> _rollingIntervals;
        private Dictionary<string, IStreamSubscribe> _rollingMap = new Dictionary<string, IStreamSubscribe>();
        //
        private Dictionary<Topic, string> _streamMap = new Dictionary<Topic, string>();

        public MarketStreamManager(ILogger log)
        {
            _log = log;
            _klineIntervals = new HashSet<string>(new string[] { "1m", "3m", "5m", "15m", "30m",
                           "1h", "2h", "4h", "6h", "8h", "12h",
                           "1d", "3h", "1w", "1M" });
            _rollingIntervals = new HashSet<string>(new string[] { "1h", "4h", "1d" });
        }

        public bool Unsubscribe(Topic topicId)
        {
            if (_streamMap.TryGetValue(topicId, out var stream))
            {
                _streamMap.Remove(topicId);
                return Unsubscribe(topicId, stream);
            }
            else 
            {
                return false;
            }
        }
        private bool Unsubscribe(Topic topicId, string stream)
        {
            if (_tickMap.TryGetValue(stream, out var tickData))
            {
                return tickData.Unsubscribe(topicId);
            }
            else if (_klineMap.TryGetValue(stream, out var klineData))
            {
                return klineData.Unsubscribe(topicId);
            }
            else if (_rollingMap.TryGetValue(stream, out var rollingData))
            {
                return rollingData.Unsubscribe(topicId);
            }
            else
            {
                throw new Exception("Unsubscribe failed " + stream);
            }
        }

        public object SubscribeTicker(Topic topicId, string symbol, string indi)
        {
            var sock = GetMarketStream();

            string stream = symbol + "@ticker";
            if (!_tickMap.TryGetValue(stream, out var tickData))
            {
                tickData = new StreamSubscribe<TopicTicker24, IBinanceTick>(stream, sock);
                _tickMap[stream] = tickData;
            }
            _streamMap[topicId] = stream;
            return tickData.Subscribe(topicId, indi);
        }

        private void OnTickerData(string stream, JToken token)
        {
            if (_tickMap.TryGetValue(stream, out var tickData))
            {
                var tick = token.ToObject<BinanceStreamTick>(DefaultSerializer);
                tickData.UpdateData(tick);
            }
        }
        
        public object SubscribeKline(Topic topicId, string symbol, string window, string indi)
        {
            var client = GetMarketStream();

            string stream = symbol + "@kline_" + window;
            if (!_klineMap.TryGetValue(stream, out var tickData))
            {
                tickData = new StreamSubscribe<TopicKline, IBinanceStreamKline>(stream, client);
                _klineMap[stream] = tickData;
            }
            _streamMap[topicId] = stream;
            return tickData.Subscribe(topicId, indi);
        }

        private void OnKlineData(string stream, JToken token)
        {
            if (_klineMap.TryGetValue(stream, out var tickData))
            {
                var kdata = token["k"];
                var kline = kdata.ToObject<BinanceStreamKline>(DefaultSerializer);
                tickData.UpdateData(kline);
            }
        }

        public object SubscribeRolling(Topic topicId, string symbol, string window, string indi)
        {
            var sock = GetMarketStream();

            string stream = symbol + "@ticker_" + window;
            if (!_rollingMap.TryGetValue(stream, out var tickData))
            {
                tickData = new StreamSubscribe<TopicTickRolling, BinanceStreamRollingWindowTick>(stream, sock);
                _rollingMap[stream] = tickData;
            }
            _streamMap[topicId] = stream;
            return tickData.Subscribe(topicId, indi);
        }

        private void OnRollingData(string stream, JToken token)
        {
            if (_rollingMap.TryGetValue(stream, out var tickData))
            {
                var tick = token.ToObject<BinanceStreamRollingWindowTick>(DefaultSerializer);
                tickData.UpdateData(tick);
            }
        }

        private SocketStream GetMarketStream()
        {
            lock (_marketStreams)
            {
                if (!TryGetMarketStream(out var stream))
                {
                    stream = NewMarketStream();
                    _marketStreams.Add(stream);
                }
                return stream;
            }
        }

        private bool TryGetMarketStream(out SocketStream stream)
        {
            // TODO: check if the stream is still alive
            // reverse iterate
            for (int i = _marketStreams.Count - 1; i >= 0; i--)
            {
                if (_marketStreams[i].Remaining() > 0)
                {
                    stream = _marketStreams[i];
                    // move to the end
                    _marketStreams.RemoveAt(i);
                    _marketStreams.Add(stream);
                    return true;
                }
            }
            stream = null;
            return false;
        }
        public HashSet<string> KlineIntervals
        {
            get => _klineIntervals;
        }
        public HashSet<string> RollingIntervals
        {
            get => _rollingIntervals;
        }

        private SocketStream NewMarketStream()
        {
            var newstream = new SocketMarketStream(_log);
            newstream.RegisterTickerHandler(OnTickerData);
            
            foreach (var interval in _klineIntervals)
            {
                newstream.RegisterKlineHandler(interval, OnKlineData);
            }

            foreach (var interval in _rollingIntervals)
            {
                newstream.RegisterRollingWindowHandler(interval, OnRollingData);
            }
            return newstream;
        }
    }
}
