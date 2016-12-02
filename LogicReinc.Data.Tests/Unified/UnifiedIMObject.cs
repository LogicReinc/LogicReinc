using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Tests.Unified
{
    [TestClass]
    public class UnifiedIMObjectTests
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            UIMTestObject.ClearDatabase();
        }

        [TestMethod]
        public void Insert()
        {
            UIMTestObject obj = new UIMTestObject()
            {
                IntegerProperty = 123,
                StringProperty = "SomeString",
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                ObjList = new List<UIMTestObject.SubTestObject>()
                {
                    new UIMTestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString1",
                        IntegerProperty = 321
                    },
                    new UIMTestObject.SubTestObject()
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

            Assert.IsNotNull(UIMTestObject.GetObject(obj.ObjectID), "No object found");
        }
        [TestMethod]
        public void Delete()
        {
            UIMTestObject obj = new UIMTestObject()
            {
                IntegerProperty = 123,
                StringProperty = "SomeString",
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                ObjList = new List<UIMTestObject.SubTestObject>()
                {
                    new UIMTestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString1",
                        IntegerProperty = 321
                    },
                    new UIMTestObject.SubTestObject()
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
            Assert.IsTrue(UIMTestObject.DeleteObject(obj.ObjectID), "Deletion failed");
            Assert.IsNull(UIMTestObject.GetObject(obj.ObjectID), "Object still present");
        }
        [TestMethod]
        public void Update()
        {
            UIMTestObject obj = new UIMTestObject()
            {
                IntegerProperty = 123,
                StringProperty = "SomeString",
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                ObjList = new List<UIMTestObject.SubTestObject>()
                {
                    new UIMTestObject.SubTestObject()
                    {
                        StringProperty = "SubSomeString1",
                        IntegerProperty = 321
                    },
                    new UIMTestObject.SubTestObject()
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

            obj = UIMTestObject.GetObject(obj.ObjectID);

            Assert.AreEqual(1234, obj.IntegerProperty);
            Assert.AreEqual("ABC", obj.StringProperty);
            Assert.AreEqual(12345, obj.ObjList[0].IntegerProperty);
        }



        [UnifiedCollection("LRUIMTestObjects")]
        public class UIMTestObject : UnifiedIMObject<UIMTestObject>
        {
            public int IntegerProperty { get; set; }
            public string StringProperty { get; set; }
            public double DoubleProperty { get; set; }

            public List<int> PrimitiveList { get; set; } = new List<int>();
            public List<string> StringList { get; set; } = new List<string>();
            public List<SubTestObject> ObjList { get; set; } = new List<SubTestObject>();


            public static UIMTestObject GetObject(string id)
            {
                return Database.FirstOrDefault(x => x.ObjectID == id);
            }

            public static bool DeleteObject(string id)
            {
                Database.FirstOrDefault(x => x.ObjectID == id)?.Delete();
                return true;
            }

            public static void ClearDatabase()
            {
                Database.ForEach(x => x.Delete());
            }

            public class SubTestObject
            {
                public int IntegerProperty { get; set; }
                public string StringProperty { get; set; }
            }
        }
    }
}
