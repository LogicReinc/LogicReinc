using LogicReinc.Data.MySQL;
using LogicReinc.Data.MySQL.Utility;
using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Tests.MySQL
{
    [TestClass]
    public class MySQLProviderTests
    {

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            MySQLProvider provider = new MySQLProvider(MySQLHelper.CreateConnectionString("localhost", "kelvin", "testing", "LRTesting"));
            TestObject.SetProvider(provider, true);

            TestObject.NewRandom().Insert();
            TestObject.NewRandom().Insert();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            foreach (TestObject obj in TestObject.Database.ToList())
                obj.Delete();
        }

        [TestMethod]
        public void Insert()
        {
            TestObject obj = TestObject.NewRandom();
            obj.Insert();
            Assert.IsNotNull(TestObject.GetObject(obj.ObjectID));
        }

        [TestMethod]
        public void Delete()
        {
            TestObject obj = TestObject.Database.FirstOrDefault();
            obj.Delete();
            Assert.IsNull(TestObject.GetObject(obj.ObjectID));
        }

        [TestMethod]
        public void Update()
        {
            TestObject obj = TestObject.Database.FirstOrDefault();

            string old = obj.String1;
            string newVal = Guid.NewGuid().ToString();
            obj.String1 = newVal;

            obj.Update();

            Assert.AreEqual(newVal, TestObject.GetObject(obj.ObjectID).String1);
        }

        [UnifiedCollection("TestObjects")]
        public class TestObject : UnifiedIMObject<TestObject>
        {
            static Random r = new Random();

            public string String1 { get; set; }
            public int Int1 { get; set; }
            public short Short1 { get; set; }
            public long Long1 { get; set; }
            public double Double1 { get; set; }
            public decimal Decimal1 { get; set; }
            public DateTime Date1 { get; set; }
            public bool Bool1 { get; set; }

            public static TestObject NewRandom()
            {
                TestObject obj = new TestObject();
                obj.String1 = Guid.NewGuid().ToString();
                obj.Int1 = r.Next();
                obj.Short1 = (short)r.Next(100000);
                obj.Long1 = r.Next();
                obj.Double1 = r.NextDouble();
                obj.Date1 = DateTime.Now;
                obj.Decimal1 = (decimal)r.NextDouble();
                obj.Bool1 = r.Next(2) > 0;
                return obj;
            }
        }
    }
}
