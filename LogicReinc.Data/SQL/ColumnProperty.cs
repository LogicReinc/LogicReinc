using LogicReinc.Data.MSSQL.Utility;
using LogicReinc.Data.SQL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.SQL
{
    public class ColumnProperty
    {
        private static Dictionary<Type, Dictionary<string, ColumnProperty>> Cache { get; set; } = new Dictionary<Type, Dictionary<string, ColumnProperty>>();

        public bool HasAttribute { get; private set; }
        public PropertyInfo Info { get; private set; }
        public string Name { get; set; }
        public Type Type { get; set; }

        public string SqlType => SqlHelper.GetSqlType(Info.PropertyType);

        public ColumnAttribute Column { get; private set; }

        private ColumnProperty(PropertyInfo info)
        {
            Info = info;
            ColumnAttribute attr = ColumnAttribute.GetAttribute(info);
            if (attr != null)
            {
                Name = attr.Name;
                Type = attr.Type;
                HasAttribute = true;
                Column = attr;
            }
            if (Name == null)
                Name = info.Name;
            if (Type == null)
                Type = info.PropertyType;

        }

        public void SetValue(object instance, object val)
        {
            Info.SetValue(instance, val, null);
        }

        public static Dictionary<string, ColumnProperty> GetCollumns<T>(bool allProps = false)
        {
            return GetCollumns(typeof(T));
        }
        public static Dictionary<string, ColumnProperty> GetCollumns(Type type, bool allProps = false)
        {
            if (!Cache.ContainsKey(type))
            {
                Dictionary<string, ColumnProperty> columns = new Dictionary<string, ColumnProperty>();
                foreach (PropertyInfo info in type.GetProperties())
                {
                    ColumnProperty column = new ColumnProperty(info);
                    if (column.HasAttribute)
                        columns.Add(column.Name, column);
                    else if (allProps)
                        columns.Add(info.Name, new ColumnProperty(info));
                }
                Cache.Add(type, columns);
            }
            return Cache[type];
        }
    }
}
