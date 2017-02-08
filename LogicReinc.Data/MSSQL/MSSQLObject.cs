using LogicReinc.Data.MSSQL.Utility;
using LogicReinc.Data.SQL;
using LogicReinc.Data.SQL.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MSSQL
{
    public class MSSQLObject<T> where T : new()
    {
        private static MSSQL sql = null;
        public static MSSQL SQL
        {
            get
            {
                if (sql == null)
                    throw new Exception($"SQL Object {typeof(T).Name} has not yet have a MSSQL object assigned to SqlObject<T>.SQL");
                return sql;
            }
            set
            {
                sql = value;
            }
        }
   
        //MetaData
        public static DBObjectDescriptorAttribute Descriptor { get; } = DBObjectDescriptorAttribute.GetAttribute(typeof(T));
        public static List<ColumnProperty> Columns { get; } = ColumnProperty.GetCollumns<T>(MSSQLHelper.Instance, true).Values.ToList();


        protected static string Join => string.Join(" ", Descriptor.Joins.Select(x => x.Join));
        protected static ColumnProperty PrimaryKey => Columns.FirstOrDefault(x => x.Column.IsPrimaryKey);
        protected static string[] ColumnNames => Columns.Select(x => x.Column.Name).ToArray();


        //Manipulation
        public bool Insert()
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();

            
            foreach (ColumnProperty prop in Columns)
            {
                object val = prop.GetValue(this);
                if (prop.HasAttribute && prop.Column.IsAutoGuid && val == null)
                    prop.SetValue(this, Guid.NewGuid().ToString("N"));
                if (!prop.HasAttribute || !prop.Column.IsAutoNumbering)
                    fields.Add(prop.Name, val);
            }

            SqlCommand com = MSSQLBuilder.Static.InsertBuilder(Descriptor.Table, fields);
            
            return (SQL.ExecuteQuery(com) > 0);
        }
        public bool Update()
        {
            string where = "";

            SqlCommand com = new SqlCommand();
            KeyValuePair<string, object>? pk = null;

            Dictionary<string, object> objs = new Dictionary<string, object>();

            foreach (ColumnProperty prop in Columns)
                if (prop.Column.IsPrimaryKey)
                {
                    where = "[" + prop.Name + "] = @" + prop.Name;
                    pk = new KeyValuePair<string, object>(prop.Name, prop.Info.GetValue(this));
                }
                else                 
                    objs.Add(prop.Name, prop.Info.GetValue(this));


            com = MSSQLBuilder.Static.UpdateBuilder(Descriptor.Table, where, objs);

            if (!pk.HasValue)
                throw new Exception("No Primary Key");
            else
                com.Parameters.AddWithValue(pk.Value.Key, pk.Value.Value);
            
            return (SQL.ExecuteQuery(com) > 0);
        }
        public bool Delete()
        {
            ColumnProperty primaryKey = PrimaryKey;

            if (primaryKey == null)
                throw new Exception("DeleteObject requires you to define a primary key");

            object pVal = primaryKey.Info.GetValue(this);
            if (pVal == null)
                throw new Exception("Primary key cannot be null");

            return SQL.ExecuteQuery(
                MSSQLBuilder.Static.DeleteBuilder(Descriptor.Table, primaryKey.Name, primaryKey.Info.GetValue(this))) > 0;
        }


        //Single
        public static T GetObject(object primaryKey)
        {
            return SQL.RetrieveObjects<T>(
                MSSQLBuilder.Static.SelectBuilder(Descriptor.Table, Join, ColumnNames,
                $"[{PrimaryKey.Name}] = @pk", new Dictionary<string, object>()
                {
                    { "pk", primaryKey }
                })).FirstOrDefault();
        }
        
        //Multiple
        public static List<T> GetObjects()
        {
            return SQL.RetrieveObjects<T>(
                MSSQLBuilder.Static.SelectBuilder(Descriptor.Table, Join, ColumnNames));
        }
        protected static List<T> GetObjects(string where, Dictionary<string, object> values = null)
        {
            return SQL.RetrieveObjects<T>(
                MSSQLBuilder.Static.SelectBuilder(Descriptor.Table, Join, ColumnNames, where, values));
        }

        //
        public static bool DeleteObject(object primaryKey)
        {
            ColumnProperty pk = PrimaryKey;

            if (pk == null)
                throw new Exception("DeleteObject requires you to define a primary key");

            return SQL.ExecuteQuery(MSSQLBuilder.Static.DeleteBuilder(Descriptor.Table, pk.Name, primaryKey)) > 0;
        }

        public static string BuildTable()
        {
            return MSSQLBuilder.Static.TableBuilder(Descriptor.Table, Columns);
        }
        public static string InitTable()
        {
            try
            {
                return SQL.ExecuteQuery(
                    MSSQLBuilder.Static.TableBuilder(Descriptor.Table, Columns)).ToString();
            }
            catch (Exception Exception) { return Exception.Message; }
        }
    }
}
