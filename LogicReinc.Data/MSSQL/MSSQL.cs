using LogicReinc.Data.SQL;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MSSQL
{
    public class MSSQL
    {
        private string conString;

        public string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(conString))
                    return conString;
                throw new Exception("ConnectionString not set for Core yet");
            }
            set
            {
                conString = value;
            }
        }

        public MSSQL() { }
        public MSSQL(string conString)
        {
            ConnectionString = conString;
        }

        private SqlConnection connection;
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
        
        public SqlVersionInfo GetVersionInfo()
        {
            return RetrieveObjects<SqlVersionInfo>("Select	SERVERPROPERTY('productversion') As [Version], SERVERPROPERTY('productlevel') As[Level], SERVERPROPERTY('edition') As[Edition]")?.FirstOrDefault();
        }
        
        public int ExecuteQuery(string query)
        {
            return ExecuteQuery(new SqlCommand(query));
        }
        public int ExecuteQuery(SqlCommand query)
        {
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                query.Connection = con;
                return query.ExecuteNonQuery();
            }
        }

        public bool ExecuteQueriesSafely(List<string> queries) => ExecuteQueriesSafely(queries.Where(x => !string.IsNullOrEmpty(x)).Select(x => new SqlCommand(x)).ToList());
        public bool ExecuteQueriesSafely(List<SqlCommand> queries)
        {
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                
                try
                {
                    foreach (SqlCommand query in queries)
                    {
                        query.Connection = con;
                        query.Transaction = transaction;

                        query.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
                return true;
            }
        }
        
        public DataTable RetrieveDataTable(string query)
        {
            return RetrieveDataTable(new SqlCommand(query));
        }
        public DataTable RetrieveDataTable(SqlCommand command)
        {
            DataSet set;
            using (SqlConnection con = CreateConnection())
            {
                command.Connection = con;

                set = new DataSet();
                set.Clear();

                command.Connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(set);

                command.Connection.Close();
            }
            return set.Tables[0];
        }

        public List<OrderedDictionary> RetrieveData(string query)
        {
            return RetrieveData(new SqlCommand(query));
        }
        public List<OrderedDictionary> RetrieveData(SqlCommand command)
        {
            using (SqlConnection con = CreateConnection())
            {
                command.Connection = con;
                command.Connection.Open();

                OrderedDictionary o = new OrderedDictionary();
                
                List<OrderedDictionary> data = new List<OrderedDictionary>();
                using (SqlDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        OrderedDictionary rowData = new OrderedDictionary();
                        for (int i = 0; i < reader.FieldCount; i++)
                            rowData.Add(reader.GetName(i), reader.GetValue(i));
                        data.Add(rowData);
                    }


                command.Connection.Close();

                return data;
            }
        }

        public List<T> RetrieveObjects<T>(string query) where T : new()
        {
            return RetrieveObjects<T>(new SqlCommand(query));
        }
        public List<T> RetrieveObjects<T>(SqlCommand command) where T : new()
        {
            List<T> items = new List<T>();

            DataTable table = RetrieveDataTable(command);

            Dictionary<string, ColumnProperty> typeProps = ColumnProperty.GetCollumns<T>();

            DataColumnCollection columns = table.Columns;

            for (int r = 0; r < table.Rows.Count; r++)
            {
                T item = new T();
                try
                {
                    foreach (DataColumn col in columns)
                    {
                        if (typeProps.ContainsKey(col.ColumnName))
                            typeProps[col.ColumnName].SetValue(item, table.Rows[r][col]);
                    }
                    items.Add(item);
                }
                catch// (Exception ex)
                {
                    //Skip Property
                }
            }

            return items;
        }

        public List<object> RetrieveObjects(Type t, SqlCommand command)
        {
            List<object> items = new List<object>();

            DataTable table = RetrieveDataTable(command);

            Dictionary<string, ColumnProperty> typeProps = ColumnProperty.GetCollumns(t);

            DataColumnCollection columns = table.Columns;

            for (int r = 0; r < table.Rows.Count; r++)
            {
                object item = Activator.CreateInstance(t);
                try
                {
                    foreach (DataColumn col in columns)
                    {
                        if (col.DataType == typeof(DBNull))
                            continue;
                        if (typeProps.ContainsKey(col.ColumnName))
                            typeProps[col.ColumnName].SetValue(item, table.Rows[r][col]);
                    }
                    items.Add(item);
                }
                catch// (Exception ex)
                {
                    //Skip Property
                }
            }

            return items;
        }

    }
}
