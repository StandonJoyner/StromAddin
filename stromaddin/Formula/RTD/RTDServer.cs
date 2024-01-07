using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration.Rtd;

namespace stromaddin.Formula.RTD
{
    [ComVisible(true)]
    [ProgId(RTDServer.ProgId)]
    internal class RTDServer : ExcelRtdServer
    {
        public const string ProgId = "strom.rtd";
        protected override bool ServerStart()
        {
            return true;
        }
        protected override void ServerTerminate()
        {
        }
        protected override object ConnectData(Topic topic, IList<string> topicInfo, ref bool newValues)
        {
            if (newValues == false)
            {
                newValues = true;
                return "--";
            }
            return Exchanges.Binance.MarketData.Subscribe(topic, topicInfo[0], topicInfo[1], topicInfo[2]);
        }
        protected override void DisconnectData(Topic topic)
        {
            Exchanges.Binance.MarketData.Unsubscribe(topic);
        }
        protected override int Heartbeat()
        {
            return 1;
        }
    }
}
