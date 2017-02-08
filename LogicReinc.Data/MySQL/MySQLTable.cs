using LogicReinc.Data.MySQL.Utility;
using LogicReinc.Data.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MySQL
{
    public class MySQLTable : SQLTable
    {
        
        public static bool CreateTable(MySQL sql, string table, List<ColumnProperty> properties)
        {
            return sql.ExecuteQuery(MySQLBuilder.Static.TableBuilder(table, properties)) > 0;
        }

        public static bool RemoveColumn(MySQL sql, string table, string column)
        {
            return sql.ExecuteQuery($"ALTER TABLE {table} DROP COLUMN {column}") > 0;
        }
        public static bool AddColumn(MySQL sql, string table, string column, string type)
        {
            return sql.ExecuteQuery($"ALTER TABLE {table} ADD COLUMN {column} {type}") > 0;
        }
        public static bool ConvertColumn(MySQL sql, string table, string column, string type)
        {
            return sql.ExecuteQuery($"ALTER TABLE {table} MODIFY COLUMN {column} {type}") > 0;
        }

        public static List<string> GetTables(MySQL sql)
        {
            return sql.RetrieveDataTable($"select * from information_schema.tables where TABLE_SCHEMA = '{sql.Database}'").Select().Select(x => (string)x["TABLE_NAME"]).ToList();
        }

        public static MySQLTable GetTable(MySQL sql, string table)
        {
            MySQLTable t = new MySQLTable();
            t.Name = table;

            foreach(DataRow row in sql.RetrieveDataTable($"SHOW COLUMNS FROM {table}").Rows)
            {
                t.Columns.Add(new ColumnProperty(MySQLHelper.Instance, (string)row["Field"], (string)row["Type"]));
            }
            return t;
        }
    }
}
