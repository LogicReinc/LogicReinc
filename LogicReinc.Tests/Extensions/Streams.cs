using LogicReinc.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Tests.Extensions
{
    [TestClass]
    public class Streams
    {

        [TestMethod]
        public void TestReadTill()
        {
            byte[] bytes = new byte[]
            {
                1, 2, 3, 4, 5, 6, 100, 110, 7, 8, 9, 10, 11, 12, 13, 14, 100, 110, 120, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25
            };

            using (MemoryStream str = new MemoryStream(bytes))
            using (MemoryStream outStr = new MemoryStream())
            {
                long read = 0;
                byte[] buffer = new byte[4096];
                bool isEnd = false;
                while ((read = str.ReadTill(new byte[] { 100,110, 120 }, buffer, 0, buffer.Length, out isEnd)) > 0)
                {
                    outStr.Write(buffer, 0, (int)read);
                }


                System.Console.WriteLine(string.Join(", ", bytes));
                System.Console.WriteLine(string.Join(", ", outStr.ToArray()));
            }
        }

    }
}
