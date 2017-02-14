using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using LogicReinc.Expressions;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MongoDB
{
    public class MongoProvider : UnifiedDatabaseProvider
    {

        private Dictionary<Type, IMongoCol> Collections { get; set; } = new Dictionary<Type, IMongoCol>();

        private Mongo mongo = null;
        public MongoSettings Settings { get; set; }
        public string DatabaseName => Settings.Database;
        public bool GenerateID => false;

        public MongoProvider(MongoSettings settings)
        {
            Settings = settings;
            mongo = new MongoDB.Mongo(settings);
        }



        public bool LoadCollection<T>() where T : UnifiedIMObject<T>
        {
            if (!Collections.ContainsKey(typeof(T)))
            {
                string col = UnifiedCollectionAttribute.GetCollection<T>();
                if (string.IsNullOrEmpty(col))
                    throw new Exception($"Missing UnifiedCollectionAttribute on type {typeof(T).Name}");

                UnifiedIMDerivesAttribute derived = UnifiedIMDerivesAttribute.GetAttribute<T>();
                if(derived != null)
                {
                    foreach (Type type in derived.Derived)
                        Method.CallGeneric(typeof(BsonClassMap).GetMethod("RegisterClassMap", new Type[] { }), null, new Type[] { type });
                }

                Collections.Add(typeof(T), mongo.GetCollection<T>(col));
            }
            return true;
        }

        private MongoCollection<C> GetCollection<C>() where C : UnifiedIMObject<C>
        {
            return (MongoCollection<C>)Collections[typeof(C)];
        }


        public bool InsertObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            GetCollection<T>().InsertObject(obj);
            return true;
        }
        public bool DeleteObject<T>(string id) where T : UnifiedIMObject<T>
        {
            GetCollection<T>().DeleteObject((x) => x.ObjectID == id);
            return true;
        }
        public bool UpdateObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            GetCollection<T>().ReplaceObject(x => x.ObjectID == obj.ObjectID, obj);
            return true;
        }
        public bool UpdateProperties<T>(string id, T obj, string[] properties) where T : UnifiedIMObject<T>
        {
            throw new NotImplementedException();
        }


        public T GetSingleObject<T>(string id) where T : UnifiedIMObject<T>
        {
            return GetCollection<T>().RetrieveObject(x => x.ObjectID == id);
        }
        public List<T> GetAllObjects<T>() where T : UnifiedIMObject<T>
        {
            return GetCollection<T>().RetrieveObjects();
        }
    }
}
