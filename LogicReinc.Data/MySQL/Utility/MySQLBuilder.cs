using LogicReinc.Data.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MySQL.Utility
{
    public class MySQLBuilder : QueryBuilder
    {
        private static MySQLBuilder _static;
        public static MySQLBuilder Static
        {
            get
            {
                if (_static == null)
                    _static = new MySQLBuilder();
                return _static;
            }
        }


        public virtual string TableBuilder(string tableName, List<ColumnProperty> fields)
        {
            StringBuilder builder = new StringBuilder();


            builder.AppendLine($"CREATE TABLE `{tableName}` (");
            builder.AppendLine(string.Join($", {Environment.NewLine}", fields.Select(cp =>
                $"\t `{cp.Name}` {((cp.HasAttribute && cp.Column.IsAutoGuid) ? "char(32)" : cp.SqlType)}" +
                ((cp.HasAttribute && cp.Column.IsAutoNumbering) ? " AUTO_INCREMENT " : "") +
                ((cp.IsPrimaryKey) ? " PRIMARY KEY " : "")
                ).ToArray()));
            builder.AppendLine(")");
            return builder.ToString();
        }

        public virtual MySqlCommand UpdateBuilder(string table, string where, Dictionary<string, object> sets)
        {
            string col = "";
            MySqlCommand com = new MySqlCommand();
            foreach (KeyValuePair<string, object> set in sets)
            {
                col += $"`{set.Key}` = @{set.Key},";
                com.Parameters.AddWithValue("@" + set.Key, set.Value);
            }

            com.CommandText = $"UPDATE `{table}` SET {col.Trim(',')} WHERE {where}";

            return com;
        }

        public virtual MySqlCommand DeleteBuilder(string table, string pk, object value)
        {
            MySqlCommand com = new MySqlCommand($"DELETE FROM `{table}` WHERE `{pk}` = @{pk};");
            com.Parameters.AddWithValue(pk, value);
            return com;
        }


        public virtual MySqlCommand SelectBuilder(string table)
        {
            MySqlCommand com = new MySqlCommand($"SELECT * FROM `{table}`");
            return com;
        }
        public virtual MySqlCommand SelectBuilder(string table, string[] columns)
        {
            MySqlCommand com = new MySqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"`{x}`"))} FROM `{table}`");
            return com;
        }

        public virtual MySqlCommand SelectBuilder(string table, string between, string[] columns)
        {
            MySqlCommand com = new MySqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"`{x}`"))} FROM `{table}` {between}");
            return com;
        }

        public virtual MySqlCommand SelectBuilder(string table, string where, Dictionary<string, object> values)
        {
            MySqlCommand com = new MySqlCommand($"SELECT * FROM `{table}` WHERE {where}");
            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }

        public virtual MySqlCommand SelectBuilder(string table, string between, string where, Dictionary<string, object> values)
        {
            MySqlCommand com = new MySqlCommand($"SELECT * FROM `{table}` {between} WHERE {where}");
            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }


        public virtual MySqlCommand SelectBuilder(string table, string[] columns, string where, Dictionary<string, object> values)
        {
            MySqlCommand com = new MySqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"`{x}`"))} FROM `{table}` WHERE {where}");
            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }

        public virtual MySqlCommand SelectBuilder(string table, string between, string[] columns, string where, Dictionary<string, object> values)
        {
            MySqlCommand com = new MySqlCommand($"SELECT {string.Join(", ", columns.Select(x => $"`{x}`"))} FROM `{table}` {between} WHERE {where}");

            if (values != null)
                foreach (KeyValuePair<string, object> val in values)
                    com.Parameters.AddWithValue(val.Key, val.Value);
            return com;
        }

        public virtual MySqlCommand InsertBuilder(string table, Dictionary<string, object> values)
        {
            MySqlCommand com = new MySqlCommand();
            string keys = "";
            string vals = "";

            foreach (KeyValuePair<string, object> obj in values)
            {
                if (obj.Value != null)
                {
                    keys += "`" + obj.Key + "`,";
                    vals += "@" + obj.Key + ",";
                    com.Parameters.AddWithValue(obj.Key, obj.Value);
                }
            }
            com.CommandText = $"INSERT INTO `{table}` ({keys.Trim(',')}) VALUES ({vals.Trim(',')});";
            return com;
        }

    }
}
