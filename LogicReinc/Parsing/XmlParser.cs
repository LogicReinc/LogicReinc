using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Parsing
{
    public class XmlConvert : XmlParser
    {
        public static string SerializeObject(object obj) => Serialize(obj);
        public static T DeserializeObject<T>(string xml) => Deserialize<T>(xml);
        public static object DeserializeObject(string xml, Type type) => Deserialize(type, xml);
    }
    public class XmlParser
    {
        private static Dictionary<Type, XmlSerializer> serializers { get; } = new Dictionary<Type, XmlSerializer>();


        public static string Serialize(object obj)
        {
            Type t = obj.GetType();
            if (!serializers.ContainsKey(t))
                serializers.Add(t, new XmlSerializer(t));
            using (MemoryStream str = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(str))
                    serializers[t].Serializer.Serialize(writer, obj);
                return Encoding.UTF8.GetString(str.ToArray());
            }
        }


        public static T Deserialize<T>(string xml) => (T)Deserialize(typeof(T), xml);
        public static object Deserialize(Type type, string xml)
        {
            if (!serializers.ContainsKey(type))
                serializers.Add(type, new XmlSerializer(type));
            using (StringReader rdr = new StringReader(xml))
                return serializers[type].Serializer.Deserialize(rdr);
        }

        public static void AddSubType(Type parent, Type sub)
        {
            if (!serializers.ContainsKey(parent))
                serializers.Add(parent, new XmlSerializer(parent));

            XmlSerializer serializer = serializers[parent];

            if (serializer.SubTypes.Contains(sub))
                return;

            serializer.UpdateSubTypes(new List<Type>() { sub });
        }

        private class XmlSerializer
        {
            public Type Type { get; set; }
            public System.Xml.Serialization.XmlSerializer Serializer { get; set; }
            public List<Type> SubTypes { get; set; }

            public XmlSerializer(Type type)
            {
                Type = type;
                SubTypes = new List<Type>();
                AddSubTypes(type);
                Serializer = new System.Xml.Serialization.XmlSerializer(type, SubTypes.ToArray());
            }

            public void UpdateSubTypes(List<Type> types)
            {
                SubTypes.AddRange(types);
                Serializer = new System.Xml.Serialization.XmlSerializer(Type, SubTypes.ToArray());
            }

            private void AddSubTypes(Type type)
            {
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo i in properties)
                {
                    if (i.PropertyType.HasElementType)
                    {
                        Type elemType = i.PropertyType.GetElementType();
                        if (!elemType.IsPrimitive && elemType != typeof(string) && !SubTypes.Contains(elemType))
                        {
                            SubTypes.Add(elemType);
                            AddSubTypes(elemType);
                        }
                    }
                    else if (i.PropertyType.IsSubclassOf(typeof(List<>)))
                    {
                        Type genType = i.PropertyType.GetGenericArguments()[0];

                        if (!genType.IsPrimitive && genType != typeof(string) && !SubTypes.Contains(genType))
                        {
                            SubTypes.Add(genType);
                            AddSubTypes(genType);
                        }
                    }
                    else if (!i.PropertyType.IsPrimitive && i.PropertyType != typeof(string) && !SubTypes.Contains(i.PropertyType))
                    {
                        SubTypes.Add(i.PropertyType);
                        AddSubTypes(i.PropertyType);
                    }
                }
            }
        }
    }
}
