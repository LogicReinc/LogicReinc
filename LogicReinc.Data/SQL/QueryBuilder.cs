using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.SQL
{
    public class QueryBuilder
    {
        public virtual string TableBuilder(string tableName, List<ColumnProperty> fields)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"CREATE TABLE [{tableName}] (");
            builder.AppendLine(string.Join($", {Environment.NewLine}", fields.Select(cp =>
                $"\t [{cp.Name}] {cp.SqlType}" +
                ((cp.Column.IsAutoNumbering) ? " Identity(1,1) " : "") +
                ((cp.Column.IsPrimaryKey) ? " PRIMARY KEY " : "")
                ).ToArray()));
            builder.AppendLine(")");
            return builder.ToString();
        }

        public virtual SqlCommand UpdateBuilder(string table, string where, Dictionary<string, object> sets)
        {
            string col = "";
            SqlCommand com = new SqlCommand();
            foreach (KeyValuePair<string, object> set in sets)
            {
                col += $"[{set.Key}] = @{set.Key},";
                com.Parameters.AddWithValue("@" + set.Key, set.Value);
            }

            com.CommandText = $"UPDATE [{table}] SET {col.Trim(',')} WHERE {where}";

            return com;
        }

        public virtual SqlCommand DeleteBuilder(string table, string pk, object value)
        {
            SqlCommand com = new SqlCommand($"DELETE FROM [{table}] WHERE [{pk}] = @{pk};");
            com.Parameters.AddWithValue(pk, value);
            return com;
        }


        public virtual SqlCommand SelectBuilder(string table)
        {
            SqlCommand com = new SqlCommand($"SELECT * FROM [{table}]");
            return com;
        }
        public virtual SqlCommand SelectBuilder(string table, string[] columns)
        {
            SqlCommand com = new SqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"[{x}]"))} FROM [{table}]");
            return com;
        }

        public virtual SqlCommand SelectBuilder(string table, string between, string[] columns)
        {
            SqlCommand com = new SqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"[{x}]"))} FROM [{table}] {between}");
            return com;
        }

        public virtual SqlCommand SelectBuilder(string table, string where, Dictionary<string, object> values)
        {
            SqlCommand com = new SqlCommand($"SELECT * FROM [{table}] WHERE {where}");
            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }

        public virtual SqlCommand SelectBuilder(string table, string between, string where, Dictionary<string, object> values)
        {
            SqlCommand com = new SqlCommand($"SELECT * FROM [{table}] {between} WHERE {where}");
            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }


        public virtual SqlCommand SelectBuilder(string table, string[] columns, string where, Dictionary<string, object> values)
        {
            SqlCommand com = new SqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"[{x}]"))} FROM [{table}] WHERE {where}");
            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }

        public virtual SqlCommand SelectBuilder(string table, string between, string[] columns, string where, Dictionary<string, object> values)
        {
            SqlCommand com = new SqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"[{x}]"))} FROM [{table}] {between} WHERE {where}");

            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }

        public virtual SqlCommand InsertBuilder(string table, Dictionary<string, object> values)
        {
            SqlCommand com = new SqlCommand();
            string keys = "";
            string vals = "";

            foreach (KeyValuePair<string, object> obj in values)
            {
                if (obj.Value != null)
                {
                    keys += obj.Key + ",";
                    vals += "@" + obj.Key + ",";
                    com.Parameters.AddWithValue(obj.Key, obj.Value);
                }
            }
            com.CommandText = $"INSERT INTO [{table}] ({keys.Trim(',')}) VALUES ({vals.Trim(',')});";
            return com;
        }
    }

}
