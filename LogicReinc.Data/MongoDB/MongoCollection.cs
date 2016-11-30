using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq.Expressions;
using LogicReinc.Data.MongoDB.Attributes;
using System.Reflection;

namespace LogicReinc.Data.MongoDB
{ 
    public interface IMongoCol
    {

    }

    public class MongoCollection<T> : IMongoCol
    {
        private Mongo client;
        private IMongoCollection<T> collection;

        public IMongoCollection<T> Collection
        {
            get
            {
                if(collection == null)
                {
                    string attr = MongoCollectionAttribute.GetCollection<T>();
                    if (attr == null)
                        throw new Exception("No collection found, either provide it in Mongo<T> constructor or add a MongoCollectionAttribute to target type");
                    collection = client.Database.GetCollection<T>(attr);
                }
                return collection;
            }
        }

        public MongoCollection(Mongo client, string colName)
        {
            this.client = client;
            collection = client.Database.GetCollection<T>(colName);

            List<PropertyInfo> props = typeof(T).GetProperties().Where(x => MongoIndexAttribute.GetIndexType(x) != IndexType.None).ToList();
            foreach(PropertyInfo i in props)
            {
                IndexType t = MongoIndexAttribute.GetIndexType(i);


                ///collection.Indexes.CreateOne(new IndexKeysDefinitionBuilder<T>().Ascending()
            }
        }



        public void InsertObject(T obj)
        {
            Collection.InsertOne(obj);
        }
        public void ReplaceObject(Expression<Func<T, bool>> condition, T obj)
        {
            Collection.ReplaceOne<T>(condition, obj);
        }
        public void UpdateObject(Expression<Func<T,bool>> condition, UpdateDefinition<T> update)
        {
            Collection.UpdateOne<T>(condition, update);
        }
        public void UpdateObjects(Expression<Func<T, bool>> condition, UpdateDefinition<T> update)
        {
            Collection.UpdateMany<T>(condition, update);
        }
        public void DeleteObject(Expression<Func<T, bool>> condition)
        {
            Collection.DeleteOne(condition);
        }

        public T RetrieveObject(Expression<Func<T, bool>> condition)
        {
            return Collection.Find<T>(condition)
                .SingleOrDefault();
        }

        public List<T> RetrieveObjects()
        {
            return Collection.Find<T>(x => true).ToList();
        }
        public List<T> RetrieveObjects(Expression<Func<T, bool>> condition)
        {
            return Collection.Find<T>(condition)
                .ToList();
        }


    }

    public class MongoSettings
    {
        public MongoClientSettings Settings { get; set; }
        public string Database { get; set; }

        public MongoSettings(string server, string db)
        {
            Settings = new MongoClientSettings()
            {
                Server = new MongoServerAddress(server)
            };
            Database = db;
        }

        public MongoSettings(MongoClientSettings settings, string db)
        {
            Settings = settings;
            Database = db;
        }
    }
}
