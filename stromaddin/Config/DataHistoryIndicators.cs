using stromaddin.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stromaddin.Config
{
    internal class DataHistoryIndicators : XMLIndicators
    {
        private static DataHistoryIndicators _instance;
        public static DataHistoryIndicators Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataHistoryIndicators();
                }
                return _instance;
            }
        }
        private DataHistoryIndicators()
            : base(RibbonResources.DataHistoryIndicators)
        {
        }
    }
}
