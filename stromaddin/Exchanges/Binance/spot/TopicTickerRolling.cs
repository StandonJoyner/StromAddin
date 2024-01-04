using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Spot.Socket;
using ExcelDna.Integration;
using static ExcelDna.Integration.Rtd.ExcelRtdServer;

namespace stromaddin.Exchanges.Binance.spot
{
    internal class TopicTickRolling : ITopicData<BinanceStreamRollingWindowTick>
    {
        string _indi { get; set; }
        Topic topic;

        public TopicTickRolling(Topic topic, string indi)
        {
            this.topic = topic;
            _indi = indi;
        }

        public void UpdateData(BinanceStreamRollingWindowTick tick)
        {
            var value = GetValue(tick);
            topic.UpdateValue(value);
        }

        public object GetValue(BinanceStreamRollingWindowTick tick)
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

                // rolling window
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
