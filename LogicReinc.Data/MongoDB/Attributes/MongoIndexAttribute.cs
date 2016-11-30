using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MongoDB.Attributes
{
    public enum IndexType
    {
        None = 0,
        Ascending = 1,
        Descending = 2,
        Unique = 3,
        Sparse = 4
    }
    public class MongoIndexAttribute : Attribute
    {
        public IndexType Type { get; set; }

        public static IndexType GetIndexType(PropertyInfo i)
        {
            MongoIndexAttribute attr = i.GetCustomAttribute<MongoIndexAttribute>();
            if (attr != null)
                return attr.Type;
            return IndexType.None;
        }
    }
}
