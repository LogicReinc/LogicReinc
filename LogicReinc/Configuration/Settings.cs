using LogicReinc.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Configuration
{
    public abstract class Settings<T> where T : new()
    {
        public static string SettingsPath { get; protected set; } = "Settings";

        public static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Refresh();
                return _instance;
            }
        }

        public static void Refresh()
        {
            if(!File.Exists(SettingsPath))
                File.WriteAllText(SettingsPath, XmlConvert.Serialize(new T()));

            _instance = XmlConvert.DeserializeObject<T>(File.ReadAllText(SettingsPath));
        }

        public static void Save()
        {
            File.WriteAllText(SettingsPath, XmlConvert.Serialize(_instance));
        }
    }
}
