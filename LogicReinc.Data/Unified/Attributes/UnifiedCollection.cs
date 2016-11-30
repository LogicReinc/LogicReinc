using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified.Attributes
{
    public class UnifiedCollectionAttribute : Attribute
    {

        public string Collection { get; private set; }

        public UnifiedCollectionAttribute(string collection)
        {
            Collection = collection;
        }

        public static string GetCollection<T>()
        {
            UnifiedCollectionAttribute col = (UnifiedCollectionAttribute)typeof(T).GetCustomAttributes(typeof(UnifiedCollectionAttribute), true).FirstOrDefault();
            if (col == null)
                return null;
            return col.Collection;
        }
    }
}
