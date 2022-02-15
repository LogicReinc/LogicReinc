using LogicReinc.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static LogicReinc.Tests.Parsing.BinaryParserTest.TestClass;

namespace LogicReinc.Tests.Parsing
{
    [TestClass]
    public class BinaryParserTest
    {
        [TestMethod]
        public void TestSerialize()
        {
            short sh = 5;
            int intt = 6;
            long lon = 7;
            byte b1 = 1;
            byte b2 = 2;
            short sh2 = 8;
            string str = "Test";
            bool bl = true;


            TestClass c = new TestClass();
            c.Short = sh;
            c.Int = intt;
            c.Long = lon;
            c.Bool = true;
            c.Test.Add(b1);
            c.Test.Add(b2);

            //c.Obj = null;
            c.Obj = new TestClass.SubClass() { Short = sh2 };

            c.Str = str;

            for (int i = 0; i < 10000; i++)
            {
                byte[] b = BinaryParser.Serialize(c);

                TestClass val = (TestClass)BinaryParser.Deserialize(b, typeof(TestClass));

                Assert.AreEqual(val.Short, sh, "class error");
                Assert.AreEqual(val.Int, intt, "class error");
                Assert.AreEqual(val.Long, lon, "class error");
                Assert.AreEqual(val.Test[0], b1, "class error");
                Assert.AreEqual(val.Test[1], b2, "class error");
                Assert.AreEqual(val.Obj?.Short, (short)8, "class error");
                Assert.AreEqual(val.Str, str, "class error");
                Assert.AreEqual(val.Bool, bl, "class error");
            }


            /*
            TestStruct c2 = new TestStruct();
            c2.Short = sh;
            c2.Int = intt;
            c2.Long = lon;
            c2.Bool = true;

            //c.Obj = null;
            c2.Obj = new TestClass.SubClass() { Short = sh2 };

            c2.Str = str;

            byte[] b3 = BinaryParser.Serialize(c2);

            TestStruct val2 = (TestStruct)BinaryParser.Deserialize(b3, typeof(TestStruct));

            Assert.AreEqual(val2.Short, sh, "struct error");
            Assert.AreEqual(val2.Int, intt, "struct error");
            Assert.AreEqual(val2.Long, lon, "struct error");
            Assert.AreEqual(val2.Obj?.Short, (short)8, "struct error");
            Assert.AreEqual(val2.Str, str, "struct error");
            Assert.AreEqual(val2.Bool, bl, "struct error");*/
        }



        public class TestClass
        {
            public int Short { get; set; }
            public int Int { get; set; } 
            public long Long { get; set; }

            public string Str { get; set; }
            public bool Bool { get; set; }

            public List<byte> Test { get; set; } = new List<byte>(); 
            public SubClass Obj { get; set; }

            public class SubClass
            {
                public short Short { get; set; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TestStruct
        {
            public int Short { get; set; }
            public int Int { get; set; }
            public long Long { get; set; }

            public string Str { get; set; }
            public bool Bool { get; set; }
            
            public SubClass Obj { get; set; }
        }
    }
}
