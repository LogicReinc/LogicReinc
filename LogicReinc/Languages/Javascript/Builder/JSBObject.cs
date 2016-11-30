using LogicReinc.Extensions;
using LogicReinc.SyntaxSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Languages.Javascript.Builder
{
    public class JSBObject : IJSBComponent
    {

        public Dictionary<string, object> Properties { get; private set; }

        public JSBObject()
        {
            Properties = new Dictionary<string, object>();
        }

        public void AddObject(string name, object val)
        {
            Type t = val.GetType();
            if(t.IsObject())
            {
                //List
                if (t.IsList() || t.IsArray())
                    Properties.Add(name, JSBCollection.FromCollection((IEnumerable)val));
                //Object
                else if (t == typeof(JSBObject) || t == typeof(JSBCollection) || t == typeof(JSBFunction))
                    Properties.Add(name, val);
                else
                    Properties.Add(name, FromObject(val));
            }
            else
            {
                AddProperty(name, val);
                /*
                new Switcher()
                    .Case(typeof(String), () => AddProperty(name, (string)val))
                    .Case(typeof(int), () => AddProperty(name, (int)val))
                    .Case(typeof(short), () => AddProperty(name, (short)val))
                    .Case(typeof(long), () => AddProperty(name, (long)val))
                    .Case(typeof(bool), () => AddProperty(name, (bool)val))
                    .Case(typeof(byte), () => AddProperty(name, (byte)val))
                    .Switch(val);*/
            }
        }

        public void AddFunction(string name, JSBuilder builder, params string[] parameters)
        {
            Properties.Add(name, new JSBFunction(null, builder, parameters));
        }

        public void AddFunction(string name, JSBFunction func)
        {
            if (func.Name != null)
                throw new Exception("JSBFunction may not have a name");
            Properties.Add(name, func);
        }

        public void AddProperty(string name, object val)
        {
            Properties.Add(name, val);
        }
        public void AddProperty(string name, string val)
        {
            Properties.Add(name, val);
        }
        public void AddProperty(string name, int val)
        {
            Properties.Add(name, val);
        }
        public void AddProperty(string name, short val)
        {
            Properties.Add(name, val);
        }
        public void AddProperty(string name, long val)
        {
            Properties.Add(name, val);
        }
        public void AddProperty(string name, bool val)
        {
            Properties.Add(name, val);
        }
        public void AddProperty(string name, byte val)
        {
            Properties.Add(name, val);
        }

        public string BuildCode(int indented = 0)
        {
            StringBuilder b = new StringBuilder();

            b.AppendLine("{");

            int i = 0;
            foreach(KeyValuePair<string, object> kv in Properties)
            {
                b.Append(JSBuilder.GetIndented(indented + 1));
                b.Append(kv.Key);
                b.Append(": ");

                b.Append(BuildValue(kv.Value, indented + 1));
                

                if (i < (Properties.Count - 1))
                    b.Append(",\n");
                else
                    b.Append("\n");
                i++;
            }
            b.Append(JSBuilder.GetIndented(indented) + "}");

            return b.ToString();
        }

        public static string BuildValue(object o, int indented = 0)
        {
            Type valType = o.GetType();

            string valueCode = "";

            if (!valType.IsObject())
                new Switcher<string>()
                    .Case(typeof(string), () => string.Format("\"{0}\"", (string)o))
                    .Case(typeof(int), () => ((int)o).ToString())
                    .Case(typeof(short), () => ((short)o).ToString())
                    .Case(typeof(long), () => ((long)o).ToString())
                    .Case(typeof(bool), () => ((bool)o).ToString())
                    .Case(typeof(byte), () => ((byte)o).ToString())
                    .Switch(valType);
            else if (valType == typeof(JSBCollection))
                valueCode = ((JSBCollection)(o)).BuildCode(indented);
            else if (valType == typeof(JSBObject))
                valueCode = ((JSBObject)o).BuildCode(indented);
            else if (valType == typeof(JSBFunction))
                valueCode = ((JSBFunction)o).BuildCode(indented);

            return valueCode;
        }

        public static JSBObject FromObject(object obj)
        {
            JSBObject o = new JSBObject();
            foreach(PropertyInfo p in obj.GetType().GetProperties())
                o.AddObject(p.Name, p.GetValue(obj));
            return o;
        }

        public static object FromAnonymous(object obj)
        {
            Type t = obj.GetType();
            if (t.IsObject())
            {
                //List/Array
                if (t.IsList() || t.IsArray())
                    return JSBCollection.FromCollection((IEnumerable)obj);
                //Object
                else
                    return FromObject(obj);
            }
            else
                return obj;
        }
    }
}
