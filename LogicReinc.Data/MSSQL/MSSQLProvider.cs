using LogicReinc.Data.MSSQL.Utility;
using LogicReinc.Data.SQL;
using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MSSQL
{
    public class MSSQLProvider : UnifiedDatabaseProvider
    {
        private Dictionary<Type, List<ColumnProperty>> ColumnCache { get; set; } = new Dictionary<Type, List<ColumnProperty>>();
        private Dictionary<Type, string> TableCache { get; set; } = new Dictionary<Type, string>();

        public string DatabaseName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MSSQL SQL { get; set; }

        public MSSQLProvider()
        {

        }



        public bool LoadCollection<C>() where C : UnifiedIMObject<C>
        {
            string collection = UnifiedCollectionAttribute.GetCollection<C>();
            if (string.IsNullOrEmpty(collection))
                throw new Exception($"Missing UnifiedCollectionAttribute on type {typeof(C).Name}");
            TableCache.Add(typeof(C), collection);
            ColumnCache.Add(typeof(C), ColumnProperty.GetCollumns<C>(true).Values.ToList());
            return true;
        }



        public bool InsertObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();

            foreach (ColumnProperty prop in ColumnCache[typeof(T)])
                if (!prop.Column.IsAutoNumbering)
                    fields.Add(prop.Name, prop.Info.GetValue(this));

            SqlCommand com = MSSQLBuilder.Static.InsertBuilder(TableCache[typeof(T)], fields);

            return (SQL.ExecuteQuery(com) > 0);
        }
        public bool DeleteObject<T>(string id) where T : UnifiedIMObject<T>
        {
            throw new NotImplementedException();
        }
        public bool UpdateObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            throw new NotImplementedException();
        }
        public bool UpdateProperties<T>(string id, T obj, string[] properties) where T : UnifiedIMObject<T>
        {
            throw new NotImplementedException();
        }




        public List<T> GetAllObjects<T>() where T : UnifiedIMObject<T>
        {
            throw new NotImplementedException();
        }

        public T GetSingleObject<T>(string id) where T : UnifiedIMObject<T>
        {
            throw new NotImplementedException();
        }        
    }
}
