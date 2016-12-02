using LogicReinc.Data.MongoDB.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MongoDB
{
    public class MongoObject<T> where T : MongoObject<T>
    {
        public static MongoSettings Settings { get; set; }
        public static Mongo Client { get; set; }
        static MongoCollection<T> mongo;
        public static MongoCollection<T> Mongo
        {
            get
            {
                if(Client == null)
                {
                    if (Settings == null)
                        throw new Exception($"MongoObject {typeof(T).Name} has not yet have a Mongo object assigned to MongoObject<T>.Client or MongoSettings to MongoObject<T>.Settings");
                    Client = new MongoDB.Mongo(Settings);
                }
                if (mongo == null)
                {
                    string col = MongoCollectionAttribute.GetCollection<T>();
                    if (string.IsNullOrEmpty(col))
                        throw new Exception($"No MongoCollectionAttribute detected on Type {typeof(T).Name}");
                    mongo = new MongoCollection<T>(Client, col);
                }
                return mongo;
            }
            set
            {
                mongo = value;
            }
        }


        //Properties
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectID { get; set; }
       
        //Manipulation
        public bool Insert()
        {
            Mongo.InsertObject((T)this);
            return true;
        }
        public bool Update()
        {
            Mongo.ReplaceObject(x => x.ObjectID == ObjectID, (T)this);
            return true;
        }
        public bool Delete()
        {
            DeleteObject(ObjectID);
            return true;
        }


        //Single
        public static T GetObject(string id)
        {
            return Mongo.RetrieveObject(x => x.ObjectID == id);
        }

        //Multiple
        public static List<T> GetObjects()
        {
            return Mongo.RetrieveObjects();
        }
        protected static List<T> GetObjects(Expression<Func<T, bool>> condition)
        {
            return Mongo.RetrieveObjects(condition);
        }

        //
        public static bool DeleteObject(string id)
        {
            Mongo.DeleteObject(x => x.ObjectID == id);
            return true;
        }
    }
}
