using LogicReinc.Data.MySQL.Utility;
using LogicReinc.Data.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MySQL
{
    public class MySQL
    {
        private string conString;
        public string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(conString))
                    return conString;
                throw new Exception("ConnectionString not set for instance yet");
            }
            set
            {
                conString = value;
            }
        }

        public string Database { get; private set; }

        public MySQL() { }
        public MySQL(string conString)
        {
            ConnectionString = conString;
            string lowered = conString.ToLower();
            if(lowered.Contains("database"))
            {
                int start = lowered.IndexOf("database=");
                if(start >= 0)
                {
                    start += 9;
                    int end = lowered.IndexOf(";", start);
                    int length = end - start;
                    
                    Database = conString.Substring(start, length);
                }
            }
        }


        private MySqlConnection connection;
        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public int ExecuteQuery(string query)
        {
            return ExecuteQuery(new MySqlCommand(query));
        }

        public int ExecuteQuery(MySqlCommand query)
        {
            using (MySqlConnection con = CreateConnection())
            {
                con.Open();
                query.Connection = con;
                return query.ExecuteNonQuery();
            }
        }

        public bool ExecuteQueriesSafely(List<string> queries) => ExecuteQueriesSafely(queries.Where(x => !string.IsNullOrEmpty(x)).Select(x => new MySqlCommand(x)).ToList());
        public bool ExecuteQueriesSafely(List<MySqlCommand> queries)
        {
            using (MySqlConnection con = CreateConnection())
            {
                con.Open();
                MySqlTransaction transaction = con.BeginTransaction();

                try
                {
                    foreach (MySqlCommand query in queries)
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
            return RetrieveDataTable(new MySqlCommand(query));
        }
        public DataTable RetrieveDataTable(MySqlCommand command)
        {
            DataSet set;
            using (MySqlConnection con = CreateConnection())
            {
                command.Connection = con;

                set = new DataSet();
                set.Clear();

                command.Connection.Open();

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(set);

                command.Connection.Close();
            }
            return set.Tables[0];
        }

        public List<OrderedDictionary> RetrieveData(string query)
        {
            return RetrieveData(new MySqlCommand(query));
        }
        public List<OrderedDictionary> RetrieveData(MySqlCommand command)
        {
            using (MySqlConnection con = CreateConnection())
            {
                command.Connection = con;
                command.Connection.Open();

                OrderedDictionary o = new OrderedDictionary();

                List<OrderedDictionary> data = new List<OrderedDictionary>();
                using (MySqlDataReader reader = command.ExecuteReader())
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

        public List<T> RetrieveObjects<T>(string query)
        {
            return RetrieveObjects<T>(new MySqlCommand(query));
        }
        public List<T> RetrieveObjects<T>(MySqlCommand command)
        {
            List<T> items = new List<T>();

            DataTable table = RetrieveDataTable(command);

            Dictionary<string, ColumnProperty> typeProps = ColumnProperty.GetCollumns<T>(MySQLHelper.Instance);

            DataColumnCollection columns = table.Columns;

            for (int r = 0; r < table.Rows.Count; r++)
            {
                T item = Activator.CreateInstance<T>();
                try
                {
                    foreach (DataColumn col in columns)
                    {
                        if (typeProps.ContainsKey(col.ColumnName))
                            typeProps[col.ColumnName].SetValue(item, Convert.ChangeType(table.Rows[r][col], typeProps[col.ColumnName].Type));
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

        public List<object> RetrieveObjects(Type t, MySqlCommand command)
        {
            List<object> items = new List<object>();

            DataTable table = RetrieveDataTable(command);

            Dictionary<string, ColumnProperty> typeProps = ColumnProperty.GetCollumns(MySQLHelper.Instance, t);

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
