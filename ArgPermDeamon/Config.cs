using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPSDeamon
{
    public class Config
    {
        /// <summary>
        /// Prop das den Pfad zur Config Datei hält
        /// </summary>
        private string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\config.xml";

        /// <summary>
        /// Hält die EInträge für jeden einzelnen Server in der Config
        /// </summary>
        public List<ConfigServer> Servers { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Config()
        {
            Servers = GetServersFromConfig();
        }

        /// <summary>
        /// Liest die Config aus und gibt die Liste von Servern zurück
        /// </summary>
        List<ConfigServer> GetServersFromConfig()
        {
            // Leere RückgabeListe
            List<ConfigServer> retList = new List<ConfigServer>();

            // Liest die config Datei in ein TextReader Objekt
            XmlTextReader xtr = new XmlTextReader(path);

            // Liest jede Zeile der XML Datei
            while (xtr.Read())
            {
                // Wenn die Node ein Element ist, der Name der Node ist "server" und die Node hat 3 Attribute
                if (xtr.NodeType == XmlNodeType.Element &&
                    xtr.Name == "server" &&
                    xtr.AttributeCount == 3)
                {
                    // Speichert die einzelnen Attribute in Variablen
                    string name = xtr.GetAttribute(0);
                    string displayname = xtr.GetAttribute(1);
                    ConfigType type = (ConfigType)(Convert.ToInt32(xtr.GetAttribute(2)));

                    if (name != null && name.Trim().Length > 0)
                    {
                        retList.Add(new ConfigServer(name, displayname, type));
                    }
                }
            }
            return retList;
        }
    }

    /// <summary>
    /// Die Types die in der Config vergeben werden können
    /// </summary>
    public enum ConfigType
    {
        FileServer = 1
    }

    /// <summary>
    /// Die Klasse die aus der Configdatei ausgelesen wird
    /// </summary>
    public class ConfigServer
    {
        public ConfigServer(string name, string displayName, ConfigType type)
        {
            Name = name;
            DisplayName = displayName;
            Type = type;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ConfigType Type { get; set; }
    }
}
