using LogicReinc.Data.MSSQL.Utility;
using LogicReinc.Data.SQL.Attributes;
using LogicReinc.Data.SQL.Utility;
using LogicReinc.Expressions;
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


        private ISQLHelper Helper { get; set; }
        public bool IsPrimaryKey { get; private set; }
        public bool HasAttribute { get; private set; }
        public PropertyInfo Info { get; private set; }
        public string Name { get; set; }
        public Type Type { get; set; }

        private string sqlType = "";
        public string SqlType
        {
            get
            {
                if (Info == null)
                    return sqlType;
                if (string.IsNullOrEmpty(sqlType))
                    sqlType = Helper.GetSqlType(Info.PropertyType, Column);
                return sqlType;
            }
        }

        public ColumnAttribute Column { get; private set; }

        private ColumnProperty(ISQLHelper helper, PropertyInfo info, bool primaryKey = false)
        {
            Info = info;
            Helper = helper;
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
            IsPrimaryKey = primaryKey || (HasAttribute && Column.IsPrimaryKey);
        }

        public ColumnProperty(ISQLHelper helper, string name, string type)
        {
            Helper = helper;
            Name = name;
            sqlType = type;
        }

        public void OverrideSqlType(string type)
        {
            sqlType = type;
        }

        public void SetValue(object instance, object val)
        {
            Property.Set(instance, Info.Name, val);
        }

        public object GetValue(object instance)
        {
            return Property.Get(instance, Info.Name);
        }

        public static Dictionary<string, ColumnProperty> GetCollumns<T>(ISQLHelper helper, bool allProps = false, params string[] primaryKeys)
        {
            return GetCollumns(helper, typeof(T), allProps, primaryKeys);
        }
        public static Dictionary<string, ColumnProperty> GetCollumns(ISQLHelper helper, Type type, bool allProps = false, params string[] primaryKeys)
        {
            if (!Cache.ContainsKey(type))
            {
                Dictionary<string, ColumnProperty> columns = new Dictionary<string, ColumnProperty>();
                foreach (PropertyInfo info in type.GetProperties())
                {
                    if (SQLHelper.IsSupportedType(info.PropertyType))
                    {
                        ColumnProperty column = new ColumnProperty(helper, info);
                        if (column.HasAttribute)
                            columns.Add(column.Name, column);
                        else if (allProps)
                            columns.Add(info.Name, new ColumnProperty(helper, info, primaryKeys.Contains(info.Name)));
                    }
                }
                Cache.Add(type, columns);
            }
            return Cache[type];
        }
    }
}
