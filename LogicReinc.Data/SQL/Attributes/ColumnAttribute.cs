using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.SQL.Attributes
{
    public class ColumnAttribute : Attribute
    {
        public bool IsPrimaryKey { get; private set; }
        public bool IsAutoNumbering { get; private set; }
        public bool IsAutoGuid { get; private set; }
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public ColumnAttribute(string name, Type type = null)
        {
            Name = name;
            Type = type;
        }

        public ColumnAttribute(string name, bool isPK = false, bool isAutoNumbering = false, bool isAutoGuid = false, Type type = null)
        {
            IsPrimaryKey = isPK;
            IsAutoNumbering = isAutoNumbering;
            IsAutoGuid = isAutoGuid;
            Name = name;
            Type = type;
        }

        public static ColumnAttribute GetAttribute(PropertyInfo info)
        {
            object[] attrs = info.GetCustomAttributes(typeof(ColumnAttribute), false);

            if (attrs.Length == 0)
                return null;
            return (ColumnAttribute)attrs[0];
        }
    }
}
