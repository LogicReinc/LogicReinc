using LogicReinc.Data.MySQL;
using LogicReinc.Data.MySQL.Utility;
using LogicReinc.Data.SQL.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql = LogicReinc.Data.MySQL.MySQL;

namespace LogicReinc.Data.Tests.MySQL
{
    [TestClass]
    public class MySqlObjectTests
    {

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            TestObject.SQL = new MySql(MySQLHelper.CreateConnectionString("localhost", "kelvin", "testing", "LRTesting"));
            TestObject.Synchronize();
        }
        
        [ClassCleanup]
        public static void Cleanup()
        {
            List<TestObject> objs = TestObject.GetObjects();
            foreach(TestObject obj in objs)
            {
                obj.Delete();
            }
        }


        [TestMethod]
        public void Insert()
        {
            TestObject obj = TestObject.CreateRandom();
            obj.Insert();
        }

        [TestMethod]
        public void Update()
        {

        }

        [TestMethod]
        public void Delete()
        {

        }

        [DBObjectDescriptor("TestObjects2")]
        public class TestObject : MySQLObject<TestObject>
        {
            static Random r = new Random();

            [Column("ObjectID", true, true)]
            public long ID { get; set; }

            [Column("ObjectID2", false, false, true)]
            public string GUID { get; set; }

            public string StringVal { get; set; }
            public int IntegerVal { get; set; }
            public double DoubleVal { get; set; }

            public static TestObject CreateRandom()
            {
                return new TestObject()
                {
                    StringVal = Guid.NewGuid().ToString(),
                    IntegerVal = r.Next(),
                    DoubleVal = r.NextDouble()
                };
            }
        }
    }
}
