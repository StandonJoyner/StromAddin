using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace stromaddin.Exchanges.Binance.spot
{
    public class SocketUsdFuturesStream : SocketStream
    {
        public static readonly string BaseUrl = "wss://fstream.binance.com/stream";
        private static readonly IEnumerable<IRateLimiter> _rateLimiters = new List<IRateLimiter>
        {
                new RateLimiter()
                    .AddConnectionRateLimit("fstream.binance.com", 5, TimeSpan.FromSeconds(1))
        };
        private static readonly WebSocketParameters _param = new WebSocketParameters(new Uri(BaseUrl), true);
        public static int MaxStream = 200;

        public SocketUsdFuturesStream(ILogger log)
            : base(log, _param, MaxStream)
        {
            _param.RateLimiters = _rateLimiters;
        }
    }
}
