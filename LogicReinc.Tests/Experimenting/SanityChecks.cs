using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Tests.Experimenting
{
    public class SanityChecks
    {
        static byte[] bytes;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Random r = new Random();
            bytes = new byte[1 * 1000 * 1000 * 1000];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)r.Next(255);

        }

        [TestMethod]
        public void ByteArraySpeed()
        {
            int count = 0;
            int h50 = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] > 50)
                {
                    h50++;
                }
                count++;
            }
            Console.WriteLine(count);
            Console.WriteLine(h50);
        }
    }
}
