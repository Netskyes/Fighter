using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Fighter
{
    [Serializable]
    public class Settings
    {
        public int windowTop = 0;
        public int windowLeft = 0;

        public string templateName = string.Empty;
        public int zoneRadius = 60;
        public bool lootEnabled = false;
        public FightMode fightMode = FightMode.Auto;

        [XmlArrayItem("name")]
        public List<string> ignoredMobs = new List<string>();

        
        public Settings()
        {

        }
    }
}
