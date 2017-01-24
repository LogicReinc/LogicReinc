using LogicReinc.Collections;
using LogicReinc.Data.Unified.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogicReinc.Data.Unified
{
    /// <summary>
    /// Unified In-Memory Object
    /// </summary>
    /// <typeparam name="T">Inheritted type</typeparam>
    public class UnifiedIMObject<T> : IUnifiedIMObject where T : UnifiedIMObject<T>
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
                    InitializeDatabase();
                }
                return database;
            }
        }

        internal override Type DataType => typeof(T);

        
        internal override IList DatabaseBase => (IList)Database;
        internal override Dictionary<string, UIMPropertyState> PropertyStates { get; } = new Dictionary<string, UIMPropertyState>();
        internal override List<KeyValuePair<UnifiedIMReference, IUnifiedIMObject>> RefTo { get; } = new List<KeyValuePair<UnifiedIMReference, IUnifiedIMObject>>();

        protected List<KeyValuePair<UnifiedIMReference, IUnifiedIMObject>> ReferenceTo => RefTo;


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public override string ObjectID { get; set; }


        private static void InitializeDatabase()
        {
            database = new TSList<T>(Provider.GetAllObjects<T>());

            database.ForEach((x) =>
            {
                UnifiedSystem.HandleObjectCreation<T>(x);
            });

            loaded = true;
        }

        public virtual bool Load()
        {
            bool b = Provider.LoadCollection<T>();
            if (b)
                InitializeDatabase();

            UnifiedSystem.RegisterType(typeof(T));
            return b;
        }

        public virtual bool Update()
        {
            if (!Loaded)
                Load();

            UnifiedSystem.HandleObjectChange<T>(this);
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

            UnifiedSystem.HandleObjectCreation<T>(this);

            return result;
        }

        public virtual bool Delete()
        {
            if (!Loaded)
                Load();
            bool result = Provider.DeleteObject<T>(ObjectID);
            if (result)
                database.Remove((T)this);
            
            UnifiedSystem.HandleObjectDeletion<T>(this);

            return result;
        }



        public static T GetObject(string id)
        {
            if (UnifiedSystem.UseOmniBase)
            {
                if (UnifiedSystem.OmniBase.ContainsKey(id))
                    return (T)UnifiedSystem.OmniBase[id];
                else
                    return null;
            }
            else
                return Database.FirstOrDefault(x => x.ObjectID == id);
                
        }


        //Utility
        public static void SetProvider(UnifiedDatabaseProvider p, bool loadDatabase = false)
        {
            provider = p;
            Activator.CreateInstance<T>().Load();
        }
    }
}
