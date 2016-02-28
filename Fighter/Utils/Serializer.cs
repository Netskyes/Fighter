using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace Fighter
{
    class Serializer
    {
        public static bool Save(object obj, string fileName)
        {
            try
            {
                XmlSerializer writer = new XmlSerializer(obj.GetType());

                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    writer.Serialize(stream, obj);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object Load(object obj, string fileName)
        {
            try
            {
                if (Validate(obj, fileName))
                {
                    XmlSerializer reader = new XmlSerializer(obj.GetType());

                    using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        return reader.Deserialize(stream);
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool Validate<T>(T obj, string fileName)
        {
            XmlDocument xml = new XmlDocument();
            T document;

            
            try
            {
                xml.Load(fileName);
                XmlSerializer reader = new XmlSerializer(obj.GetType());

                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    document = (T)reader.Deserialize(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

