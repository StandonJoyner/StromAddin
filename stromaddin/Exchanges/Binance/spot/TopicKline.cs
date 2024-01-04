using Binance.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExcelDna.Integration.Rtd.ExcelRtdServer;

namespace stromaddin.Exchanges.Binance.spot
{
    internal class TopicKline : ITopicData<IBinanceStreamKline>
    {
        string Indi { get; set; }
        Topic topic;

        public TopicKline(Topic topic, string indi)
        {
            this.topic = topic;
            Indi = indi;
        }

        public void UpdateData(IBinanceStreamKline kline)
        {
            var val = GetValue(kline);
            topic.UpdateValue(val);
        }
        public object GetValue(IBinanceStreamKline kline)
        {
            return kline.OpenPrice;
        }
    }
}
