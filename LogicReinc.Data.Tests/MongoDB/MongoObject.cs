using LogicReinc.Data.MongoDB;
using LogicReinc.Data.MongoDB.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Tests.MongoDB
{
    [TestClass]
    public class MongoObjectTests
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            TestObject.Settings = new MongoSettings("localhost", "LRDataTests");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            TestObject.ClearDatabase();
        }

        const int itterations = 10000;
        [TestMethod]
        public void InsertSpeed()
        {
            for (int i = 0; i < itterations; i++)
                Insert();
        }

        [TestMethod]
        public void Insert()
        {
            TestObject obj = new TestObject()
            {
                IntegerProperty = 123,
                StringProperty = "SomeString",
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                ObjList = new List<TestObject.SubTestObject>()
                {
                    new TestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString1",
                        IntegerProperty = 321
                    },
                    new TestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString2",
                        IntegerProperty = 543
                    }
                },
                PrimitiveList = new List<int>()
                {
                    1,2,3
                }
            };

            Assert.IsTrue(obj.Insert(), "Insertion failed");

            Assert.IsNotNull(TestObject.GetObject(obj.ObjectID), "No object found");
        }
        [TestMethod]
        public void Delete()
        {
            TestObject obj = new TestObject()
            {
                IntegerProperty = 123,
                StringProperty = "SomeString",
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                ObjList = new List<TestObject.SubTestObject>()
                {
                    new TestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString1",
                        IntegerProperty = 321
                    },
                    new TestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString2",
                        IntegerProperty = 543
                    }
                },
                PrimitiveList = new List<int>()
                {
                    1,2,3
                }
            };

            Assert.IsTrue(obj.Insert(), "Insertion failed");
            Assert.IsTrue(TestObject.DeleteObject(obj.ObjectID), "Deletion failed");
            Assert.IsNull(TestObject.GetObject(obj.ObjectID), "Object still present");
        }
        [TestMethod]
        public void Update()
        {
            TestObject obj = new TestObject()
            {
                IntegerProperty = 123,
                StringProperty = "SomeString",
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                ObjList = new List<TestObject.SubTestObject>()
                {
                    new TestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString1",
                        IntegerProperty = 321
                    },
                    new TestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString2",
                        IntegerProperty = 543
                    }
                },
                PrimitiveList = new List<int>()
                {
                    1,2,3
                }
            };

            Assert.IsTrue(obj.Insert(), "Insertion failed");

            obj.IntegerProperty = 1234;
            obj.StringProperty = "ABC";
            obj.ObjList[0].IntegerProperty = 12345;
            obj.Update();

            obj = TestObject.GetObject(obj.ObjectID);

            Assert.AreEqual(1234, obj.IntegerProperty);
            Assert.AreEqual("ABC", obj.StringProperty);
            Assert.AreEqual(12345, obj.ObjList[0].IntegerProperty);
        }


        [MongoCollection("TestObjects")]
        public class TestObject : MongoObject<TestObject>
        {
            public int IntegerProperty { get; set; }
            public string StringProperty { get; set; }
            public double DoubleProperty { get; set; }

            public List<int> PrimitiveList { get; set; } = new List<int>();
            public List<string> StringList { get; set; } = new List<string>();
            public List<SubTestObject> ObjList { get; set; } = new List<SubTestObject>();


            public static void ClearDatabase()
            {
                Mongo.DeleteObject(x => true);
            }

            public class SubTestObject
            {
                public int IntegerProperty { get; set; }
                public string StringProperty { get; set; }
            }
        }
    }
}
