using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MongoDB
{
    public class Mongo
    {
        public MongoClient Client { get; set; }
        public IMongoDatabase Database { get; set; }

        public Dictionary<string, IMongoCol> Collections { get; set; } = new Dictionary<string, IMongoCol>();

        public Mongo(MongoSettings config)
        {
            Client = new MongoClient(config.Settings);
            Database = Client.GetDatabase(config.Database);
        }

        public MongoCollection<T> GetCollection<T>(string col)
        {
            if (!Collections.ContainsKey(col))
                Collections.Add(col, new MongoCollection<T>(this, col));
            return (MongoCollection<T>)Collections[col];
        }
        
    }
}
