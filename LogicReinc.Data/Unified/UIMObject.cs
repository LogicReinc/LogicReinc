using LogicReinc.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified
{
    /// <summary>
    /// Unified In-Memory Object
    /// </summary>
    /// <typeparam name="T">Inheritted type</typeparam>
    public class UnifiedIMObject<T> where T : UnifiedIMObject<T>
    {
        protected static bool loaded = false;
        public static bool Loaded { get { return loaded; } }

        private static UnifiedDatabaseProvider provider;
        public static UnifiedDatabaseProvider Provider { get { return provider; } }

        public static string DatabaseName { get { return Provider.DatabaseName; } }

        protected static TSList<T> database;
        public static TSList<T> Database
        {
            get
            {
                if (database == null)
                {
                    if (!Loaded)
                        new UnifiedIMObject<T>().Load();
                    database = new TSList<T>(Provider.GetAllObjects<T>());
                    loaded = true;
                }
                return database;
            }
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string ObjectID { get; set; }

        public virtual bool Load()
        {
            bool b = Provider.LoadCollection<T>();
            if (b)
            {
                database = new TSList<T>(Provider.GetAllObjects<T>());
                loaded = true;
            }
            return b;
        }

        public virtual bool Update()
        {
            if (!Loaded)
                Load();
            return Provider.UpdateObject<T>((T)this);
        }
        public virtual bool Update(T obj, bool update, params string[] properties)
        {
            Type t = GetType();
            foreach (string s in properties)
            {
                PropertyInfo type = t.GetProperty(s);
                if (type.SetMethod != null)
                    type.SetValue(this, type.GetValue(obj));
            }
            if (update)
                return Update();
            return true;
        }
        public virtual bool UpdateProperties(params string[] properties)
        {
            if (!Loaded)
                Load();
            return Provider.UpdateProperties<T>(ObjectID, (T)this, properties);
        }

        public virtual bool Insert()
        {
            if (!Loaded)
                Load();
            bool result = Provider.InsertObject<T>((T)this);
            if (result)
                database.Add((T)this);
            return result;
        }

        public virtual bool Delete()
        {
            if (!Loaded)
                Load();
            bool result = Provider.DeleteObject<T>(ObjectID);
            if (result)
                database.Remove((T)this);
            return result;
        }

        //Utility
        public static void SetProvider(UnifiedDatabaseProvider p, bool loadDatabase = false)
        {
            provider = p;
            Activator.CreateInstance<T>().Load();
        }
    }
}
