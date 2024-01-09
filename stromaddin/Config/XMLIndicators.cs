using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace stromaddin.Config
{
    public struct RtdChoice
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public struct RtdParam
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public List<RtdChoice> Choices { get; set; }
    }

    public class XMLIndicator : ICloneable
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<RtdParam> Params { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    internal class XMLIndicators
    {
        private Dictionary<string, RtdParam> _params = new Dictionary<string, RtdParam>();
        private List<XMLIndicator> _indicators = new List<XMLIndicator>();

        public XMLIndicators(string xml)
        {
            ParseXml(xml);
        }
        public List<XMLIndicator> GetIndicators()
        {
            return _indicators;
        }

        private void ParseXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var root = doc.DocumentElement;
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                var node = root.ChildNodes[i];
                if (node.Name == "Params")
                {
                    ParseParams(node);
                }
                else if (node.Name == "Indicators")
                {
                    ParseIndicators(node);
                }
            }
        }
        private void ParseParams(XmlNode prams)
        {
            for (int i = 0; i < prams.ChildNodes.Count; i++)
            {
                var node = prams.ChildNodes[i];
                if (node.Name == "Param")
                {
                    var id = node.Attributes["ID"].Value;
                    var param = new RtdParam();
                    param.Name = node.Attributes["Name"].Value;
                    param.Key = node.Attributes["Key"].Value;
                    param.Value = node.Attributes["Value"].Value;
                    param.Choices = new List<RtdChoice>();
                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {
                        var choice = new RtdChoice();
                        choice.Name = node.ChildNodes[j].Attributes["Name"].Value;
                        choice.Value = node.ChildNodes[j].Attributes["Value"].Value;
                        param.Choices.Add(choice);
                    }
                    _params.Add(id, param);
                }
            }
        }
        private void ParseIndicators(XmlNode rootIndi)
        {
            for (int i = 0; i < rootIndi.ChildNodes.Count; i++)
            {
                var node = rootIndi.ChildNodes[i];
                if (node.Name == "Indicator")
                {
                    var indicator = new XMLIndicator();
                    indicator.Name = node.Attributes["Name"].Value;
                    indicator.Key = node.Attributes["Key"].Value;
                    indicator.Description = node.Attributes["Help"].Value;
                    indicator.Type = node.Attributes["Type"].Value;
                    indicator.Params = new List<RtdParam>();
                    for (int j = 0; j < node.ChildNodes.Count; j++)
                    {
                        var id = node.ChildNodes[j].Attributes["ID"].Value;
                        var param = _params[id];
                        indicator.Params.Add(param);
                    }
                    _indicators.Add(indicator);
                }
            }
        }
    }
}
