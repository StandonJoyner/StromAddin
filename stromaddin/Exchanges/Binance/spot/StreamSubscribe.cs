using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static ExcelDna.Integration.Rtd.ExcelRtdServer;

namespace stromaddin.Exchanges.Binance.spot
{
    internal interface IStreamSubscribe
    {
        object Subscribe(Topic topicId, string indi);
        bool Unsubscribe(Topic topicId);
        void UpdateData(object data);
    }

    internal interface ITopicData<D>
    {
        object GetValue(D data);
        void UpdateData(D data);
    }

    internal class StreamSubscribe<T, D> : IStreamSubscribe where T : ITopicData<D>
    {
        private readonly string _stream;
        private readonly SocketStream _sock;
        private readonly Dictionary<Topic, T> itemMap = new Dictionary<Topic, T>();
        private D _data;

        public StreamSubscribe(string stream, SocketStream sock)
        {
            _stream = stream;
            _sock = sock;
        }

        public object Subscribe(Topic topicId, string indi)
        {
            if (itemMap.Count == 0)
                _sock.Subscribe(_stream);
            T item = (T)Activator.CreateInstance(typeof(T), topicId, indi);
            itemMap[topicId] = item;
            if (_data != null)
                return item.GetValue(_data);
            else
                return "--";
        }

        public bool Unsubscribe(Topic topicId)
        {
            itemMap.Remove(topicId);
            if (itemMap.Count == 0)
                _sock.Unsubscribe(_stream);
            return true;
        }

        public void UpdateData(object data)
        {
            _data = (D)data;
            foreach (var item in itemMap)
            {
                item.Value.UpdateData(_data);
            }
        }
    }
}
