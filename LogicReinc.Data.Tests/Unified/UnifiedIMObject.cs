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
        const bool InitialSet = true;

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


            if(InitialSet)
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
        public void Initialization()
        {
            System.Console.WriteLine("Initialized");
        }

        [TestMethod]
        public void IndexedWhere()
        {

            var d1 = UIMTestObject.WhereIndexed("StringProperty", "Testing");
            for(int i = 0; i < 1000000;i ++)
            {
                var data = UIMTestObject.WhereIndexed("IntegerProperty" , i);
            }


        }


        [TestMethod]
        public void ReferenceTest()
        {
            UIMTestObject.ClearDatabase();
            UIMTestObject2.ClearDatabase();
            UIMTestObject parent1 = new UIMTestObject()
            {
                IntegerProperty = 1
            };
            UIMTestObject parent2 = new UIMTestObject()
            {
                IntegerProperty = 2
            };
            parent1.Insert();
            parent2.Insert();

            List<UIMTestObject2> items = new List<UIMTestObject2>();
            for(int i = 0; i < 10; i++)
            {
                UIMTestObject2 child = new UIMTestObject2();
                if (i % 2 == 0)
                {
                    child.IntegerProperty = 1;
                    child.StringProperty = "Was 1";
                }
                else
                {
                    child.IntegerProperty = 2;
                    child.StringProperty = "Was 2";
                }
                child.InitialIndex = i;
                child.Insert();
                items.Add(child);

            }

            Assert.AreEqual(5, parent1.Referenced.Count);
            Assert.AreEqual(5, parent2.Referenced.Count);
            for(int i = 0; i < items.Count; i++)
            {
                List<IUnifiedIMObject> refs = items[i].GetReferences();
                Assert.AreEqual(1, refs.Count);

                if (i % 2 == 0)
                {
                    Assert.IsTrue(parent1.Referenced.Contains(items[i]));
                    Assert.AreEqual(parent1, refs[0]);
                }
                else
                {
                    Assert.IsTrue(parent2.Referenced.Contains(items[i]));
                    Assert.AreEqual(parent2, refs[0]);
                }
            }

            System.Console.WriteLine("Initial References");
            System.Console.WriteLine("Parent1: " + string.Join(" ", parent1.Referenced.Select(x => x.InitialIndex).ToArray()));
            System.Console.WriteLine("Parent2: " + string.Join(" ", parent2.Referenced.Select(x => x.InitialIndex).ToArray()));


            for (int i = 0; i < 10; i++)
            {
                UIMTestObject2 child = items[i];
                if (i % 2 == 0)
                    child.IntegerProperty = 2;
                else
                    child.IntegerProperty = 1;
                child.Update();

            }

            Assert.AreEqual(5, parent1.Referenced.Count);
            Assert.AreEqual(5, parent2.Referenced.Count);
            for (int i = 0; i < items.Count; i++)
            {
                List<IUnifiedIMObject> refs = items[i].GetReferences();
                Assert.AreEqual(1, refs.Count);

                if (i % 2 == 0)
                {
                    Assert.IsTrue(parent2.Referenced.Contains(items[i]));
                    Assert.AreEqual(parent2, refs[0]);
                }
                else
                {
                    Assert.IsTrue(parent1.Referenced.Contains(items[i]));
                    Assert.AreEqual(parent1, refs[0]);
                }
            }

            System.Console.WriteLine("After exchanging the ids (On targets)");
            System.Console.WriteLine("Parent1: " + string.Join(" ", parent1.Referenced.Select(x => x.InitialIndex).ToArray()));
            System.Console.WriteLine("Parent2: " + string.Join(" ", parent2.Referenced.Select(x => x.InitialIndex).ToArray()));

            parent1.IntegerProperty = 2;
            parent1.Update();
            Assert.AreEqual(5, parent1.Referenced.Count);
            for(int i = 0; i < items.Count; i++)
            {
                if(i % 2 == 0)
                    Assert.IsTrue(parent1.Referenced.Contains(items[i]));
            }

            System.Console.WriteLine("After changing the id of parent 1 to parent 2");
            System.Console.WriteLine("Parent1: " + string.Join(" ", parent1.Referenced.Select(x => x.InitialIndex).ToArray()));
            System.Console.WriteLine("Parent2: " + string.Join(" ", parent2.Referenced.Select(x => x.InitialIndex).ToArray()));

            foreach (UIMTestObject2 child in items)
            {
                child.Delete();
                Assert.AreEqual(0, child.GetReferences().Count);
            }

            Assert.AreEqual(0, parent1.Referenced.Count);
            Assert.AreEqual(0, parent2.Referenced.Count);

            System.Console.WriteLine("After deletion of all targets");
            System.Console.WriteLine("Parent1: " + string.Join(" ", parent1.Referenced.Select(x => x.InitialIndex).ToArray()));
            System.Console.WriteLine("Parent2: " + string.Join(" ", parent2.Referenced.Select(x => x.InitialIndex).ToArray()));
        }


        [TestMethod]
        public void TestInsertSpeed()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Random r = new Random();
            for (int i = 0; i < 10000; i++)
                new UIMTestObject2()
                {
                    IntegerProperty = r.Next(1000),
                    StringProperty = UIMTestObject.Database[r.Next(UIMTestObject.Database.Length)].ObjectID
                }.Insert();

            watch.Stop();
            System.Console.WriteLine($"{(double)watch.ElapsedTicks / TimeSpan.TicksPerMillisecond} - {watch.ElapsedMilliseconds}");
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

            
            List<UIMTestObject2> objs = new List<UIMTestObject2>();
            for (int i = 0; i < 10000; i++)
            {
                UIMTestObject2 o = new UIMTestObject2();
                o.IntegerProperty = i;
                o.Insert();
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            UIMTestObject2.ClearDatabase();
            watch.Stop();
            System.Console.WriteLine($"{watch.ElapsedMilliseconds}");
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


            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 10000; i++)
            {

                obj.IntegerProperty = i;
                obj.Update();
            }
            watch.Stop();
            System.Console.WriteLine($"{watch.ElapsedTicks / TimeSpan.TicksPerMillisecond} - {watch.ElapsedMilliseconds}");


            obj.IntegerProperty = 1234;
            if (UIMTestObject.Database.Length > 2)
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
            System.Console.WriteLine(w.ElapsedMilliseconds);
        }

        [UnifiedCollection("LRUIMTestObjects")]
        public class UIMTestObject : UnifiedIMObject<UIMTestObject>
        {
            public int IntegerProperty { get; set; }

            [UnifiedIMIndex]
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
            public int InitialIndex { get; set; }
            public int IntegerProperty { get; set; }
            public string StringProperty { get; set; }
            public double DoubleProperty { get; set; }

            public List<int> PrimitiveList { get; set; } = new List<int>();
            public List<string> StringList { get; set; } = new List<string>();
            public List<SubTestObject> ObjList { get; set; } = new List<SubTestObject>();

            

            public static bool DeleteObject(string id)
            {
                Database.FirstOrDefault(x => x.ObjectID == id)?.Delete();
                return true;
            }

            public static void ClearDatabase()
            {
                Database.ToList().ForEach(x => x.Delete());
            }

            public List<IUnifiedIMObject> GetReferences()
            {
                return ReferenceTo.Select(x=>x.Value).ToList();
            }

            public class SubTestObject
            {
                public int IntegerProperty { get; set; }
                public string StringProperty { get; set; }
            }
        }
    }
}
