using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Requests;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stromaddin.Exchanges.Binance.spot
{
    public class SocketMarketStream : SocketStream
    {
        public static int MaxStreams = 1000;
        public static readonly string BaseUrl = "wss://stream.binance.com:9443/stream";
        private static readonly IEnumerable<IRateLimiter> _rateLimiters = new List<IRateLimiter>{
                new RateLimiter()
                    .AddConnectionRateLimit("stream.binance.com", 5, TimeSpan.FromSeconds(1))
        };
        private static readonly WebSocketParameters _param = new WebSocketParameters(new Uri(BaseUrl), true);

        public SocketMarketStream(ILogger log)
            : base(log, _param, MaxStreams)
        {
            _param.RateLimiters = _rateLimiters;
        }

        public void RegisterTickerHandler(Action<string, JToken> handler)
        {
            RegisterHandler("ticker", handler);
        }
        public void RegisterRollingWindowHandler(string windowSize, Action<string, JToken> handler)
        {
            RegisterHandler("ticker_" + windowSize, handler);
        }
        public void RegisterKlineHandler(string windowSize, Action<string, JToken> handler)
        {
            RegisterHandler("kline_" + windowSize, handler);
        }

    }
}
