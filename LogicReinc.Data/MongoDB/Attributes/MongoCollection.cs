using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MongoDB.Attributes
{
    public class MongoCollectionAttribute : Attribute
    {
        public string Collection { get; private set; }
        public MongoCollectionAttribute(string collection)
        {
            Collection = collection;
        }

        public static string GetCollection<T>()
        {
            MongoCollectionAttribute attr = (MongoCollectionAttribute)typeof(T)
                .GetCustomAttributes(true).FirstOrDefault();
            if (attr == null)
                return null;

            return attr.Collection;
        }
    }
}
