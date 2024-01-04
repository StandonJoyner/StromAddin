using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Objects;
using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Clients;
using Binance.Net.Objects.Models.Spot.Socket;
using static ExcelDna.Integration.Rtd.ExcelRtdServer;
using CryptoExchange.Net.Objects;
using ExcelDna.Integration;
using stromaddin.Exchanges.Binance.spot;

namespace stromaddin.Exchanges.Binance
{
    //internal class SymbolObserver : IObserver<>
    internal class MarketData
    {
        static private int subStatus = 0;
        static private readonly IBinanceSocketClient socketClient = new BinanceSocketClient();
        //static Dictionary<string, StreamTicker24> tickMap = new Dictionary<string, StreamTicker24>();
        static Dictionary<Topic, string>       symbolMap = new Dictionary<Topic, string>();

        static MarketStreamManager marketStreamManager = new MarketStreamManager(new TraceLogger());
        static public object Subscribe(Topic topicId, string symbol, string indi, string param)
        {
            symbol = ToBinanceSymbol(symbol);
            if (symbol.Length == 0)
                return ExcelErrorUtil.ToComError(ExcelError.ExcelErrorValue);

            if (param.Length == 0)
            {
                return marketStreamManager.SubscribeTicker(topicId, symbol, indi);
            }
            else
            {
                string[] parms = param.Split('=');
                if (parms.Length == 2 && parms[0] == "period")
                {
                    if (parms[1] == "1d")
                    {
                        return marketStreamManager.SubscribeTicker(topicId, symbol, indi);
                    }
                    bool isRolling = marketStreamManager.RollingIntervals.Contains(parms[1]);
                    if (isRolling)
                    {
                        return marketStreamManager.SubscribeRolling(topicId, symbol, parms[1], indi);
                    }
                    else
                    {
                        return marketStreamManager.SubscribeKline(topicId, symbol, parms[1], indi);
                    }
                }
                else
                {
                    return ExcelErrorUtil.ToComError(ExcelError.ExcelErrorValue);
                }
            }
        }

        static public void Unsubscribe(Topic topicId)
        {
            marketStreamManager.Unsubscribe(topicId);
        }

        static string ToBinanceSymbol(string symbol)
        {
            return symbol.ToLower();
        }

        static int EnsureSubscribed()
        {
            if (subStatus == 0)
            {
                bool succ = SubscribeAllTickerUpdates().Wait(1000);
                if (!succ)
                    subStatus = -1;
            }
            return subStatus;
        }

        static async Task SubscribeAllTickerUpdates()
        {
            var subscribeResult = await socketClient.SpotApi.ExchangeData.SubscribeToAllRollingWindowTickerUpdatesAsync(
                TimeSpan.FromDays(1), data =>
                {
                    foreach (var ud in data.Data)
                    {
                        //if (tickMap.TryGetValue(ud.Symbol, out var tickerData))
                        {
                            // tickerData.UpdateData(ud);
                        }
                    }
                });
            if (subscribeResult.Success)
                subStatus = 1;
            else
                subStatus = -1;
        }
    }
}
