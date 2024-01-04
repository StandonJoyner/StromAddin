using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Interfaces;
using ExcelDna.Integration;
using static ExcelDna.Integration.Rtd.ExcelRtdServer;

namespace stromaddin.Exchanges.Binance.spot
{
    internal class TopicTicker24 : ITopicData<IBinanceTick>
    {
        string _indi { get; set; }
        Topic topic;

        public TopicTicker24(Topic topic, string indi)
        {
            this.topic = topic;
            _indi = indi;
        }

        public void UpdateData(IBinanceTick tick)
        {
            var value = GetValue(tick);
            topic.UpdateValue(value);
        }

        private DateTime Convert1900DateToLocal(DateTime date1900)
        {
            DateTime localDate = TimeZoneInfo.ConvertTime(date1900, TimeZoneInfo.Local);
            return localDate;
        }

        public object GetValue(IBinanceTick tick)
        {
            switch (_indi)
            {
                // real time
                case "open":
                    return tick.OpenPrice;
                case "high":
                    return tick.HighPrice;
                case "low":
                    return tick.LowPrice;
                case "last_price":
                    return tick.LastPrice;
                case "last_volume":
                    return tick.LastQuantity;

                case "bid":
                    return tick.BestBidPrice;
                case "bid_volume":
                    return tick.BestBidQuantity;
                case "ask":
                    return tick.BestAskPrice;
                case "ask_volume":
                    return tick.BestAskQuantity;
                // 24hr
                case "volume":
                    return tick.Volume;
                case "quote_volume":
                    return tick.QuoteVolume;
                case "count":
                    return tick.TotalTrades;
                case "change":
                    return tick.PriceChange;
                case "change_percent":
                    return tick.PriceChangePercent;
                case "weighted_average":
                    return tick.WeightedAveragePrice;
                // other
                case "open_time":
                    return TimeZoneInfo.ConvertTime(tick.OpenTime, TimeZoneInfo.Local);
                case "time":
                    return TimeZoneInfo.ConvertTime(tick.CloseTime, TimeZoneInfo.Local);
                default:
                    return ExcelErrorUtil.ToComError(ExcelError.ExcelErrorValue);
            }
        }
    }
}
