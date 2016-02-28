using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Fighter
{
    [Serializable]
    public class CombatTemplate
    {
        public Skills skills;
        public Buffs buffs;

        public CombatTemplate()
        {
            skills = new Skills(); buffs = new Buffs();
        }
    }

    public class Skills
    {
        [XmlArrayItem("name")]
        public List<string> rotation = new List<string>();

        [XmlArrayItem("trigger")]
        public List<Combos> combos = new List<Combos>();
    }

    public class Combos
    {
        [XmlAttribute("name")]
        public string triggerName;

        [XmlArrayItem("name")]
        public List<string> combo = new List<string>();
    }


    public class Buffs
    {
        [XmlArrayItem("name")]
        public List<string> preCombat = new List<string>();

        [XmlArrayItem("name")]
        public List<string> combat = new List<string>();
    }
}
