using LogicReinc.Data.MongoDB;
using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using LogicReinc.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Property.BuildILPropertyGetters(new PropertyReference(typeof(UIMTestObject), "StringProperty"),
                new PropertyReference(typeof(UIMTestObject), "StringProperty"),
                new PropertyReference(typeof(UIMTestObject), "Referenced"));
            UnifiedSystem.AllowReferences = true;
            UnifiedSystem.AllowIndexes = true;
            UnifiedSystem.CreateReferenceIndexes = true;
            MongoProvider provider = new MongoProvider(new MongoSettings("127.0.0.1", "LogicReincDataTests"));
            UIMTestObject.SetProvider(provider);
            UIMTestObject2.SetProvider(provider);



            for (int i = 0; i < 10000; i++)
            {
                new UIMTestObject()
                {
                    IntegerProperty = i,
                    StringProperty = (i % 2 == 0) ? "SomeString" : "Testing",
                }.Insert();
            }
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            UIMTestObject.ClearDatabase();
            UIMTestObject2.ClearDatabase();
        }



        [TestMethod]
        public void TestInsertSpeed()
        {
            Random r = new Random();
            for (int i = 0; i < 10000; i++)
                new UIMTestObject2()
                {
                    IntegerProperty = r.Next(1000),
                    StringProperty = UIMTestObject.Database[r.Next(UIMTestObject.Database.Length)].ObjectID
                }.Insert();

        }

        [TestMethod]
        public void Insert()
        {
            UIMTestObject2 obj = new UIMTestObject2()
            {
                IntegerProperty = 123,
                StringProperty = "SomeString",
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                /*
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
                },*/
                PrimitiveList = new List<int>()
                {
                    1,2,3
                }
            };

            Assert.IsTrue(obj.Insert(), "Insertion failed");

            Assert.IsNotNull(UIMTestObject2.GetObject(obj.ObjectID), "No object found");
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
            UIMTestObject2 obj = new UIMTestObject2()
            {
                IntegerProperty = 123,
                StringProperty = UIMTestObject.Database[0].ObjectID,
                StringList = new List<string>()
                {
                    "Test1",
                    "Test2"
                },
                DoubleProperty = 1.234,
                /*
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
                },*/
                PrimitiveList = new List<int>()
                {
                    1,2,3
                }
            };

            Assert.IsTrue(obj.Insert(), "Insertion failed");

            obj.IntegerProperty = 1234;
            obj.StringProperty = UIMTestObject.Database[2].ObjectID;
            //obj.ObjList[0].IntegerProperty = 12345;
            obj.Update();

            obj = UIMTestObject2.GetObject(obj.ObjectID);

            Assert.AreEqual(1234, obj.IntegerProperty);
            //Assert.AreEqual("ABC", obj.StringProperty);
            //Assert.AreEqual(12345, obj.ObjList[0].IntegerProperty);
        }


        [TestMethod]
        public void TestReferencing()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            List<UIMTestObject> uims = new List<UIMTestObject>();
            for (int a = 0; a < 10; a++)
            {
                UIMTestObject uim = new UIMTestObject()
                {
                    StringProperty = "Testing"
                };
                uim.Insert();
                uims.Add(uim);
            }

            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds);
        }

        [UnifiedCollection("LRUIMTestObjects")]
        public class UIMTestObject : UnifiedIMObject<UIMTestObject>
        {
            public int IntegerProperty { get; set; }

            public string StringProperty { get; set; }
            public double DoubleProperty { get; set; }


            [BsonIgnore]
            [UnifiedIMReference("IntegerProperty", typeof(UIMTestObject2), "IntegerProperty")]
            public List<UIMTestObject2> Referenced { get; set; } = new List<UIMTestObject2>();


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
                Database.ToList().ForEach(x => x.Delete());
            }

            public class SubTestObject
            {
                public int IntegerProperty { get; set; }
                public string StringProperty { get; set; }
            }
        }

        [UnifiedCollection("LRUIMTestObjects2")]
        public class UIMTestObject2 : UnifiedIMObject<UIMTestObject2>
        {
            public int IntegerProperty { get; set; }
            public string StringProperty { get; set; }
            public double DoubleProperty { get; set; }

            public List<int> PrimitiveList { get; set; } = new List<int>();
            public List<string> StringList { get; set; } = new List<string>();
            public List<SubTestObject> ObjList { get; set; } = new List<SubTestObject>();


            public static UIMTestObject2 GetObject(string id)
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
                Database.ToList().ForEach(x => x.Delete());
            }

            public class SubTestObject
            {
                public int IntegerProperty { get; set; }
                public string StringProperty { get; set; }
            }
        }
    }
}
