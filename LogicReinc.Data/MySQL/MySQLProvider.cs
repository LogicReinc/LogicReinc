using LogicReinc.Data.MySQL.Utility;
using LogicReinc.Data.SQL;
using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MySQL
{
    public class MySQLProvider : UnifiedDatabaseProvider
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

        public MySQL SQL { get; set; }

        public MySQLProvider(string connectionString)
        {
            SQL = new MySQL(connectionString);
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
            ColumnCache.Add(typeof(C), ColumnProperty.GetCollumns<C>(MySQLHelper.Instance, true, "ObjectID").Values.ToList());
            ColumnProperty objidcol = GetColumns<C>().FirstOrDefault(X => X.Name == "ObjectID");
            foreach(ColumnProperty prop in GetColumns<C>())
            {
                if (prop.Name == "ObjectID")
                    prop.OverrideSqlType("char(32)");
                else
                    prop.OverrideSqlType(MySQLHelper.Instance.GetSqlType(prop.Type, prop.Column));
            }

            if(!MySQLTable.GetTables(SQL).Contains(collection))
            {
                try
                {
                    MySQLTable.CreateTable(SQL, collection, GetColumns<C>());
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to create table {collection}");
                }
            }
            else
            {
                MySQLTable table = MySQLTable.GetTable(SQL, collection);
                List<ColumnProperty> todo = table.Columns.ToList();
                foreach(ColumnProperty col in GetColumns<C>())
                {
                    ColumnProperty existing = table.Columns.FirstOrDefault(X => X.Name == col.Name);
                    if(existing == null)
                    {
                        System.Console.WriteLine($"SQL missing Column {col.Name}... Adding");
                        try
                        {
                            MySQLTable.AddColumn(SQL, collection, col.Name, col.SqlType);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to add collumn {col.Name}\n{ex.Message}");
                        }
                    }
                    else if(!existing.SqlType.StartsWith(col.SqlType))
                    {
                        System.Console.WriteLine($"SQL incorrect Column Type for {col.Name}... Converting");
                        try
                        {
                            MySQLTable.ConvertColumn(SQL, collection, col.Name, col.SqlType);
                        }
                        catch(Exception ex)
                        {
                            throw new Exception($"Failed to convert collumn {col.Name}\n{ex.Message}");
                        }
                    }
                    if (existing != null)
                        todo.Remove(existing);
                }
                foreach(ColumnProperty prop in todo)
                {
                    System.Console.WriteLine($"Excess collumn {prop.Name}... Removing");
                    try
                    {
                        MySQLTable.RemoveColumn(SQL, collection, prop.Name);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to remove collumn {prop.Name}\n{ex.Message}");
                    }
                }
            }

            return true;
        }



        public bool InsertObject<T>(T obj) where T : UnifiedIMObject<T>
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();

            obj.ObjectID = Guid.NewGuid().ToString("N");

            foreach (ColumnProperty prop in ColumnCache[typeof(T)])
                if (!prop.HasAttribute || !prop.Column.IsAutoNumbering)
                    fields.Add(prop.Name, prop.GetValue(obj));

            MySqlCommand com = MySQLBuilder.Static.InsertBuilder(TableCache[typeof(T)], fields);

            return (SQL.ExecuteQuery(com) > 0);
        }
        public bool DeleteObject<T>(string id) where T : UnifiedIMObject<T>
        {
            return SQL.ExecuteQuery(MySQLBuilder.Static.DeleteBuilder(GetTable<T>(), "ObjectID", id)) > 0;
        }
        public bool UpdateObject<T>(T obj) where T : UnifiedIMObject<T>
        {

            Dictionary<string, object> update = new Dictionary<string, object>();
            foreach (ColumnProperty prop in GetColumns<T>())
                if (prop.Name != "ObjectID")
                    update.Add(prop.Name, prop.GetValue(obj));

            return SQL.ExecuteQuery(MySQLBuilder.Static.UpdateBuilder(GetTable<T>(), $"ObjectID = '{obj.ObjectID}'", update)) > 0;
        }
        public bool UpdateProperties<T>(string id, T obj, string[] properties) where T : UnifiedIMObject<T>
        {
            Dictionary<string, object> update = new Dictionary<string, object>();
            foreach (ColumnProperty prop in GetColumns<T>())
                if (prop.Name != "ObjectID" && properties.Contains(prop.Name))
                    update.Add(prop.Name, prop.GetValue(obj));

            return SQL.ExecuteQuery(MySQLBuilder.Static.UpdateBuilder(GetTable<T>(), $"ObjectID = '{obj.ObjectID}'", update)) > 0;
        }




        public List<T> GetAllObjects<T>() where T : UnifiedIMObject<T>
        {
            return SQL.RetrieveObjects<T>(MySQLBuilder.Static.SelectBuilder(GetTable<T>()));
        }

        public T GetSingleObject<T>(string id) where T : UnifiedIMObject<T>
        {
            return SQL.RetrieveObjects<T>(MySQLBuilder.Static.SelectBuilder(GetTable<T>(), "ObjectID = @ID", new Dictionary<string, object>()
            {
                ["ObjectID"] = id
            })).FirstOrDefault();
        }
    }
}
