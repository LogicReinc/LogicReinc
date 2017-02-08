using LogicReinc.Data.FileIO;
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
    public class UnifiedFileIO
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            TestObject.SetProvider(new FileIOProvider("TestDatabase"));
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            TestObject.ClearDatabase();
        }


        [TestMethod]
        public void CreateSome()
        {
            for (int i = 0; i < 1000; i++)
                TestObject.CreateRandom();
        }



        [UnifiedCollection("TestObject")]
        public class TestObject : UnifiedIMObject<TestObject>
        {
            static Random r = new Random();

            public string TestString { get; set; }
            public int TestInteger { get; set; }
            public DateTime TestDate { get; set; }

            public List<Testsub> Subs { get; set; }

            public class Testsub
            {
                public int SubInteger { get; set; }
                public string SubString { get; set; }
            }


            public static void CreateRandom()
            {
                new TestObject()
                {
                    TestString = "asodgosdalg",
                    TestInteger = r.Next(),
                    Subs = new List<Testsub>()
                    {
                        new Testsub()
                        {
                            SubInteger = r.Next(),
                            SubString = Guid.NewGuid().ToString()
                        },
                        new Testsub()
                        {
                            SubInteger = r.Next(),
                            SubString = Guid.NewGuid().ToString()
                        }
                    },
                    TestDate = DateTime.Now
                }.Insert();
            }

            public static void ClearDatabase()
            {
                Database.ToList().ForEach(x => GetObject(x.ObjectID).Delete());
            }

        }
    }
}
