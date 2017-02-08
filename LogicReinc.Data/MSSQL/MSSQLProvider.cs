using LogicReinc.Data.MSSQL.Utility;
using LogicReinc.Data.MySQL.Utility;
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

        public bool GenerateID => true;

        public MSSQL SQL { get; set; }

        public MSSQLProvider(string connectionString)
        {
            SQL = new MSSQL(connectionString);
        }

        private string GetTable<T>()
        {
            if (!TableCache.ContainsKey(typeof(T)))
                throw new Exception("Table not found");
            return TableCache[typeof(T)];
        }

        private List<ColumnProperty> GetColumns<T>()
        {
            if (!ColumnCache.ContainsKey(typeof(T)))
                throw new Exception("Table not found");
            return ColumnCache[typeof(T)];
        }

        public bool LoadCollection<C>() where C : UnifiedIMObject<C>
        {
            string collection = UnifiedCollectionAttribute.GetCollection<C>();
            if (string.IsNullOrEmpty(collection))
                throw new Exception($"Missing UnifiedCollectionAttribute on type {typeof(C).Name}");
            TableCache.Add(typeof(C), collection);
            ColumnCache.Add(typeof(C), ColumnProperty.GetCollumns<C>(MSSQLHelper.Instance, true, "ObjectID").Values.ToList());
            return true;
        }



        public bool InsertObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();

            obj.ObjectID = Guid.NewGuid().ToString();

            foreach (ColumnProperty prop in ColumnCache[typeof(T)])
                if (!prop.Column.IsAutoNumbering)
                    fields.Add(prop.Name, prop.Info.GetValue(this));

            SqlCommand com = MSSQLBuilder.Static.InsertBuilder(TableCache[typeof(T)], fields);

            return (SQL.ExecuteQuery(com) > 0);
        }
        public bool DeleteObject<T>(string id) where T : UnifiedIMObject<T>
        {
            return SQL.ExecuteQuery(MSSQLBuilder.Static.DeleteBuilder(GetTable<T>(), "ObjectID", id)) > 0;
        }
        public bool UpdateObject<T>(T obj) where T : UnifiedIMObject<T>
        {

            Dictionary<string, object> update = new Dictionary<string, object>();
            foreach (ColumnProperty prop in GetColumns<T>())
                if (prop.Name != "ObjectID")
                    update.Add(prop.Name, prop.GetValue(obj));

            return SQL.ExecuteQuery(MSSQLBuilder.Static.UpdateBuilder(GetTable<T>(), $"ObjectID = '{obj.ObjectID}'", update)) > 0;
        }
        public bool UpdateProperties<T>(string id, T obj, string[] properties) where T : UnifiedIMObject<T>
        {
            Dictionary<string, object> update = new Dictionary<string, object>();
            foreach (ColumnProperty prop in GetColumns<T>())
                if (prop.Name != "ObjectID" && properties.Contains(prop.Name))
                    update.Add(prop.Name, prop.GetValue(obj));

            return SQL.ExecuteQuery(MSSQLBuilder.Static.UpdateBuilder(GetTable<T>(), $"ObjectID = '{obj.ObjectID}'", update)) > 0;
        }




        public List<T> GetAllObjects<T>() where T : UnifiedIMObject<T>
        {
            return SQL.RetrieveObjects<T>(MSSQLBuilder.Static.SelectBuilder(GetTable<T>()));
        }

        public T GetSingleObject<T>(string id) where T : UnifiedIMObject<T>
        {
            return SQL.RetrieveObjects<T>(MSSQLBuilder.Static.SelectBuilder(GetTable<T>(), "ObjectID = @ID", new Dictionary<string, object>()
            {
                ["ObjectID"] = id
            })).FirstOrDefault();
        }        
    }
}
