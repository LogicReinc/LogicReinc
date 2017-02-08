using LogicReinc.Data.MySQL;
using LogicReinc.Data.MySQL.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySQLs = LogicReinc.Data.MySQL.MySQL;

namespace LogicReinc.Data.Tests.MySQL
{
    [TestClass]
    public class MySQLTests
    {
        static MySQLs sql;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            sql = new MySQLs(MySQLHelper.CreateConnectionString("localhost", "kelvin", "testing", "LRTesting"));
        }


        [TestMethod]
        public void GetTables()
        {
            List<string> tables = MySQLTable.GetTables(sql);
        }

        [TestMethod]
        public void GetTable()
        {
            MySQLTable table = MySQLTable.GetTable(sql, "TestObjects");
        }
        
    }
}
