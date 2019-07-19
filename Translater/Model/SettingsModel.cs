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
        private static string _keyPath;
        public static string KeyPath
        {
            get => _keyPath;
            private set {
                if (_keyPath == value) return;
                _keyPath = value;
            }
        }


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
                    KeyPath = string.IsNullOrEmpty(keyPath.Value) ? "key.json": keyPath.Value;
                }
            }
        }

        public static SettingsModel GetInstance() { return _lazy.Value; }

        public static void SaveKey(string path)
        {
            KeyPath = path;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("AppSettings.xml");
            var xRoot = xDoc.DocumentElement;
            var googleKey = xRoot.SelectSingleNode("GoogleKey");
            googleKey.Attributes[0].Value = path;
            xDoc.Save("AppSettings.xml");
        }
    }
}
