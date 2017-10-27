using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LogicReinc.Extensions;
using LogicReinc.Expressions;
using System.Collections;

namespace LogicReinc.Parsing
{
    public static class BinaryParser
    {
        public static object Deserialize(byte[] data, Type t)
        {
            using(MemoryStream obj = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(obj))
                return Deserialize(reader, t);
        }

        public static object Deserialize(MemoryStream obj, Type t)
        {
            using (BinaryReader reader = new BinaryReader(obj))
                return Deserialize(reader, t);
        }

        public static object Deserialize(BinaryReader rea, Type t)
        {
            if (t.IsPrimitive)
                return DeserializePrimitive(rea, t);
            else if (t.IsList())
                return DeserializeList(rea, t);
            else if (t.IsArray)
                return DeserializeArray(rea, t);
            else
            {

                object o = Activator.CreateInstance(t);
                foreach (PropertyInfo p in t.GetProperties().OrderBy(x=>x.Name))
                    DeserializeProperty(rea, p, o);

                return o;
            }
        }

        public static T Deserialize<T>(MemoryStream obj)
        {
            Type t = typeof(T);
            return (T)Deserialize(obj, t);
        }


        public static void DeserializeProperty(BinaryReader rea, PropertyInfo info, object o)
        {
            Type t = info.PropertyType;

            if (t.IsPrimitive)
                Property.Set(o, info.Name, DeserializePrimitive(rea, info.PropertyType));
            else if (t.IsList())
                Property.Set(o, info.Name, DeserializeList(rea, info.PropertyType));
            else if (t.IsArray)
                Property.Set(o, info.Name, DeserializeArray(rea, info.PropertyType));
            else
                Property.Set(o, info.Name, DeserializeObject(rea, info.PropertyType));
        }


        public static object DeserializeList(BinaryReader reader, Type type)
        {
            IList list = (IList)Activator.CreateInstance(type);

            Type t = list.GetType().GenericTypeArguments.FirstOrDefault();

            int read = reader.ReadInt32();

            for (int i = 0; i < read; i++)
                list.Add(Deserialize(reader, t));

            return list;
        }

        public static object DeserializeArray(BinaryReader reader, Type type)
        {
            Array list = (Array)Activator.CreateInstance(type);

            int read = reader.ReadInt32();

            for (int i = 0; i < read; i++)
                list.SetValue(Deserialize(reader, type), i);

            return list;
        }


        public static object DeserializeObject(BinaryReader rea, Type t)
        {
            bool b = rea.ReadByte() == (byte)1;
            if (!b)
                return null;
            if (t == typeof(string))
                return DeserializeString(rea);
            return Deserialize(rea, t);
        }

        public static object DeserializeString(BinaryReader rea)
        {
            int read = rea.ReadInt32();

            byte[] data = new byte[read];
            rea.Read(data, 0, read);

            return Encoding.UTF8.GetString(data);
        }


        public static object DeserializePrimitive(BinaryReader reader, Type typ)
        {
            if (typ == typeof(byte))
                return reader.ReadByte();
            else if (typ == typeof(char))
                return reader.ReadChar();
            else if (typ == typeof(short))
                return reader.ReadInt16();
            else if (typ == typeof(ushort))
                return reader.ReadUInt16();
            else if (typ == typeof(int))
                return reader.ReadInt32();
            else if (typ == typeof(uint))
                return reader.ReadUInt32();
            else if (typ == typeof(long))
                return reader.ReadInt64();
            else if (typ == typeof(ulong))
                return reader.ReadUInt64();
            else if (typ == typeof(decimal))
                return reader.ReadDecimal();
            else if (typ == typeof(float))
                return reader.ReadSingle();
            else if (typ == typeof(double))
                return reader.ReadDouble();
            return Activator.CreateInstance(typ);
        }


        public static byte[] Serialize(short packageType, object obj)
        {
            using (MemoryStream str = new MemoryStream())
            using(BinaryWriter wri = new BinaryWriter(str))
            {
                wri.Write(packageType);
                wri.Write(Serialize(obj));
                
                return str.ToArray();
            }
        }
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream str = new MemoryStream())
            using (BinaryWriter wri = new BinaryWriter(str))
            {
                Type t = obj.GetType();

                if (t.IsPrimitive)
                    SerializePrimitive(wri, obj);
                else if (t.IsList())
                    SerializeList(wri, obj);
                else if (t.IsArray)
                    SerializeArray(wri, obj);
                else
                    foreach (PropertyInfo p in t.GetProperties().OrderBy(x => x.Name))
                        SerializeProperty(wri, p.PropertyType, Property.Get(obj, p.Name));

                return str.ToArray();
            }
        }


        public static void SerializeString(BinaryWriter wri, string val)
        {
            byte[] data = Encoding.UTF8.GetBytes(val);

            wri.Write(data.Length);

            wri.Write(data);
        }

        public static void SerializeProperty(BinaryWriter wri, Type t, object obj)
        {
            if (t.IsPrimitive)
                SerializePrimitive(wri, obj);
            else if (t.IsList())
                SerializeList(wri, obj);
            else if (t.IsArray)
                SerializeArray(wri, obj);
            else
                SerializeObject(wri, obj);
        }

        public static void SerializeList(BinaryWriter writer, object obj)
        {
            IList list = (IList)obj;

            writer.Write(list.Count);
            
            for(int i = 0; i < list.Count; i++)
                writer.Write(Serialize(list[i]));
        }

        public static void SerializeArray(BinaryWriter writer, object obj)
        {
            Array list = (Array)obj;

            writer.Write(list.Length);

            for (int i = 0; i < list.Length; i++)
                writer.Write(Serialize(list.GetValue(i)));
        }

        public static void SerializeObject(BinaryWriter writer, object obj)
        {
            if (obj == null)
            {
                writer.Write((byte)0);
                return;
            }
            else
                writer.Write((byte)1);

            if (obj.GetType() == typeof(string))
                SerializeString(writer, (string)obj);
            else
                writer.Write(Serialize(obj));
        }

        public static void SerializePrimitive(BinaryWriter writer, object obj)
        {
            Type typ = obj.GetType();

            if (typ == typeof(byte))
                writer.Write((byte)obj);
            else if (typ == typeof(char))
                writer.Write((char)obj);
            else if (typ == typeof(short))
                writer.Write((short)obj);
            else if (typ == typeof(ushort))
                writer.Write((ushort)obj);
            else if (typ == typeof(int))
                writer.Write((int)obj);
            else if (typ == typeof(uint))
                writer.Write((uint)obj);
            else if (typ == typeof(long))
                writer.Write((long)obj);
            else if (typ == typeof(ulong))
                writer.Write((ulong)obj);
            else if (typ == typeof(decimal))
                writer.Write((decimal)obj);
            else if (typ == typeof(float))
                writer.Write((float)obj);
            else if (typ == typeof(double))
                writer.Write((double)obj);
        }
    }
}
