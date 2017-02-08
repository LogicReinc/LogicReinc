using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.FileIO
{
    public class FileIOProvider : UnifiedDatabaseProvider
    {
        public string DirectoryPath { get; private set; }

        public bool GenerateID => true;

        public string DatabaseName
        {
            get
            {
                return DirectoryPath;
            }
        }

        public FileIOProvider(string directoryName)
        {
            Directory.CreateDirectory(directoryName);
            DirectoryPath = directoryName;
            
        }

        private string BuildID()
        {
            return Guid.NewGuid().ToString() + DateTime.Now.Second.ToString();
        }

        private string GetCollectionPath(string collection)
        {
            return DirectoryPath + "/" + collection + "/";
        }

        private string GetObjectPath(string collection, string id)
        {
            return DirectoryPath + "/" + collection + "/" + id;
        }

        private void EnsureExistence(string collection, string id)
        {
            if (!File.Exists(GetObjectPath(collection, id)))
                throw new ArgumentException("Given Collection/ID does not exist");
        }

        public bool LoadCollection<C>() where C : UnifiedIMObject<C>
        {
            string col = UnifiedCollectionAttribute.GetCollection<C>();
            col = GetCollectionPath(col);

            Directory.CreateDirectory(col);
            return true;
        }


        public List<T> GetAllObjects<T>() where T : UnifiedIMObject<T>
        {
            string col = UnifiedCollectionAttribute.GetCollection<T>();
            List<T> objs = new List<T>();
            foreach (FileInfo f in new DirectoryInfo(GetCollectionPath(col)).GetFiles())
            {
                T obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(f.FullName));
                if(obj != null)
                    objs.Add(obj);
            }
            
            return objs;
        }

        public T GetSingleObject<T>(string id) where T : UnifiedIMObject<T>
        {
            string col = UnifiedCollectionAttribute.GetCollection<T>();
            EnsureExistence(col, id);
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(GetObjectPath(col, id)));
        }

        public bool InsertObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            string col = UnifiedCollectionAttribute.GetCollection<T>();
            string id = BuildID();
            obj.ObjectID = id;
            File.WriteAllText(GetObjectPath(col, id), JsonConvert.SerializeObject(obj));
            return true;
        }
        
        public bool UpdateObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            string col = UnifiedCollectionAttribute.GetCollection<T>();
            EnsureExistence(col, obj.ObjectID);
            File.WriteAllText(GetObjectPath(col, obj.ObjectID), JsonConvert.SerializeObject(obj));
            return true;
        }

        public bool UpdateProperties<T>(string id, T obj, string[] properties) where T : UnifiedIMObject<T>
        {
            string col = UnifiedCollectionAttribute.GetCollection<T>();
            EnsureExistence(col, id);
            File.WriteAllText(GetObjectPath(col, obj.ObjectID), JsonConvert.SerializeObject(obj));
            return true;
        }

        public bool DeleteObject<T>(string id) where T : UnifiedIMObject<T>
        {
            string col = UnifiedCollectionAttribute.GetCollection<T>();
            EnsureExistence(col, id);
            File.Delete(GetObjectPath(col, id));
            return true;
        }

       
    }
}
