using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.SQL.Attributes
{
    public class DBObjectDescriptorAttribute : Attribute
    {
        public string Table { get; private set; }
        public string PrimaryKey { get; set; }
        public JoinAttribute[] Joins { get; private set; } = new JoinAttribute[0];

        public DBObjectDescriptorAttribute(string table, string[] columns = null, string[] joins = null)
        {
            this.Table = table;
        }

        public static DBObjectDescriptorAttribute GetAttribute(Type info)
        {
            object[] attrs = info.GetCustomAttributes(typeof(DBObjectDescriptorAttribute), false);

            if (attrs.Length == 0)
                return null;
            return (DBObjectDescriptorAttribute)attrs[0];
        }
    }
}
