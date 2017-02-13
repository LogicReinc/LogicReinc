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
        static Random _random = new Random();
        static string TestID { get; set; }
        static string TestID2 { get; set; }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            TestObject.SQL = new MySql(MySQLHelper.CreateConnectionString("localhost", "kelvin", "testing", "LRTesting"));
            TestObject.Synchronize();
            TestObject rObj = TestObject.CreateRandom();
            rObj.Insert();
            TestID = rObj.GUID;
            TestObject rObj2 = TestObject.CreateRandom();
            rObj2.Insert();
            TestID2 = rObj2.GUID;
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

        const int itterations = 10000;
        [TestMethod]
        public void InsertSpeed()
        {
            for (int i = 0; i < itterations; i++)
                TestObject.CreateRandom().Insert();
        }

        [TestMethod]
        public void Insert()
        {
            TestObject obj = TestObject.CreateRandom();
            obj.Insert();
            obj = TestObject.GetObjectByGUID(obj.GUID);
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void Update()
        {
            TestObject obj = TestObject.GetObjectByGUID(TestID);
            int nrnd = _random.Next();
            obj.IntegerVal = nrnd;
            obj.Update();
            obj = TestObject.GetObjectByGUID(TestID);

            Assert.AreEqual(nrnd, obj.IntegerVal);
        }

        [TestMethod]
        public void Delete()
        {
            TestObject obj = TestObject.GetObjectByGUID(TestID);
            obj.Delete();
            obj = TestObject.GetObjectByGUID(obj.GUID);
            Assert.IsNull(obj);
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


            public static TestObject GetObjectByGUID(string guid)
            {
                return GetObjects("ObjectID2 = @Guid", new Dictionary<string, object>
                {
                    ["Guid"] = guid
                }).FirstOrDefault();
            }


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
