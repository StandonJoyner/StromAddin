using stromaddin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot;
using stromaddin.Core.Enums;
using CryptoExchange.Net.CommonObjects;

namespace stromaddin.Exchanges.Binance
{
    public class ExchangeInfo
    {
        private static BinanceExchangeInfo[] _cache = new BinanceExchangeInfo[3];
        private static ObjectCache _symbolSpotCache = MemoryCache.Default;
        private static AsyncLock _spotLock = new AsyncLock();
        private static CacheItemPolicy _cachePolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Today.AddDays(1) };

        private static async Task<BinanceExchangeInfo> GetExchangeInfo(MarketType type)
        {
            if (type == MarketType.SPOT)
                return await GetSpotExchangeInfo();
            else
                return null;
        }

        private static async Task<BinanceExchangeInfo> GetSpotExchangeInfo()
        {
            using (await _spotLock.LockAsync())
            {
                if (_cache[0] != null)
                    return _cache[0];
                var client = new BinanceRestClient();
                var info = await client.SpotApi.ExchangeData.GetExchangeInfoAsync();
                _cache[0] = info.Data;
                return info.Data;
            }
        }

        public static IEnumerable<BinanceSymbol> GetAllSymbols(MarketType type, bool tradingOnly)
        {
            lock (_symbolSpotCache)
            {
                if (_symbolSpotCache.Count() > 0)
                {
                    return _symbolSpotCache.Select(s => (BinanceSymbol)s.Value);
                }
                var ts = GetExchangeInfo(type);
                var info = ts.Result;
                if (info == null)
                    return Enumerable.Empty<BinanceSymbol>();
                if (tradingOnly)
                {
                    var all = info.Symbols.Where(s => s.Status == SymbolStatus.Trading);
                    foreach (var s in all)
                    {
                        _symbolSpotCache.Add(s.Name, s, _cachePolicy);
                    }
                }
                else
                {
                    foreach (var s in info.Symbols)
                    {
                        _symbolSpotCache.Add(s.Name, s, _cachePolicy);
                    }
                }
                return _symbolSpotCache.Select(s => (BinanceSymbol)s.Value);
            }
        }

        private static async Task<BinanceSymbol> GetSymbol(MarketType type, string symbol)
        {
            if (type == MarketType.SPOT)
            {
                return await GetSpotSymbols(symbol);
            }
            else
            {
                var info = await GetExchangeInfo(type);
                return info.Symbols.FirstOrDefault(s => s.Name == symbol);
            }
        }

        private static async Task<BinanceSymbol> GetSpotSymbols(string symbol)
        {
            if (_symbolSpotCache.Contains(symbol))
                return (BinanceSymbol)_symbolSpotCache.Get(symbol);

            var info = await GetExchangeInfo(MarketType.SPOT);
            var sym = info.Symbols.FirstOrDefault(s => s.Name == symbol);
            _symbolSpotCache.Add(symbol, sym, _cachePolicy);
            return sym;
        }
    }
}
