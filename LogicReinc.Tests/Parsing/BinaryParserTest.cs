using LogicReinc.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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


            TestClass c = new TestClass();
            c.Short = sh;
            c.Int = intt;
            c.Long = lon;
            c.Test.Add(b1);
            c.Test.Add(b2);

            //c.Obj = null;
            c.Obj = new TestClass.SubClass() { Short = sh2 };

            c.Str = str;

            byte[] b = BinaryParser.Serialize(c);

            TestClass val = (TestClass)BinaryParser.Deserialize(b, typeof(TestClass));

            Assert.AreEqual(val.Short, sh);
            Assert.AreEqual(val.Int, intt);
            Assert.AreEqual(val.Long, lon);
            Assert.AreEqual(val.Test[0], b1);
            Assert.AreEqual(val.Test[1], b2);
            Assert.AreEqual(val.Obj?.Short, 8);
            Assert.AreEqual(val.Str, str);
        }



        public class TestClass
        {
            public int Short { get; set; }
            public int Int { get; set; } 
            public long Long { get; set; }

            public string Str { get; set; }

            public List<byte> Test { get; set; } = new List<byte>(); 
            public SubClass Obj { get; set; }

            public class SubClass
            {
                public short Short { get; set; }
            }
        }
    }
}
