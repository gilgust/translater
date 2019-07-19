using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Translater.Model
{
    public class SettingsModel
    {
        private static readonly Lazy<SettingsModel> _lazy = new Lazy<SettingsModel>(() => new SettingsModel());
        public string KeyPath { get; }

        private SettingsModel() {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("AppSettings.xml");
            var xRoot = xDoc.DocumentElement;

            foreach (XmlNode item in xRoot)
            {
                if (item.Attributes.Count == 0 && item.ChildNodes.Count == 0) continue;

                if(item.Name == "GoogleKey")
                {
                    XmlNode keyPath = item.Attributes.GetNamedItem("path");

                    this.KeyPath = keyPath?.Value ?? "key.json";
                }
            }
        }

        public static SettingsModel GetInstance() { return _lazy.Value; }
    }
}
