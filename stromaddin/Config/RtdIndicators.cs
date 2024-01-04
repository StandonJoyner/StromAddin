﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

    public class RtdIndicator : ICloneable
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

    internal class RtdIndicators
    {
        private Dictionary<string, RtdParam> _params = new Dictionary<string, RtdParam>();
        private List<RtdIndicator> _indicators = new List<RtdIndicator>();
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
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith("Resources.RtdIndicators.xml"));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    ParseXml(result);
                }
            }
        }

        public List<RtdIndicator> GetIndicators()
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
                    var indicator = new RtdIndicator();
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
