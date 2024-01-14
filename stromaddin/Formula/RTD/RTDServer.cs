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
            (string indi, string param) = ParseIndicator(topicInfo[1]);
            return Exchanges.Binance.MarketData.Subscribe(topic, topicInfo[0], indi, param);
        }
        protected override void DisconnectData(Topic topic)
        {
            Exchanges.Binance.MarketData.Unsubscribe(topic);
        }
        protected override int Heartbeat()
        {
            return 1;
        }
        private (string, string) ParseIndicator(string indi)
        {
            int paramStart = indi.IndexOf('(');
            string name;
            string strParams = "";
            if (paramStart == -1)
                name = indi;
            else
                name = indi.Substring(0, paramStart);
            if (paramStart > 0)
            {
                int paramEnd = indi.IndexOf(')');
                int len = paramEnd - paramStart - 1;
                strParams = indi.Substring(paramStart+1, len);
            }
            return (name, strParams);
        }
    }
}
