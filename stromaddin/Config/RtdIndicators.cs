using stromaddin.Config;
using stromaddin.Resources;

namespace stromaddin.Config
{
    internal class RtdIndicators : XMLIndicators
    {
        private static RtdIndicators _instance;
        public static RtdIndicators Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RtdIndicators();
                }
                return _instance;
            }
        }
        private RtdIndicators()
            : base(RibbonResources.RtdIndicators)
        {
        }
    }
}
