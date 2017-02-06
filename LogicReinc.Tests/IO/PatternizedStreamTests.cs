using LogicReinc.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Tests.IO
{
    [TestClass]
    public class PatternizedStreamTests
    {
        static Random random = new Random();
        static byte[] part1 = new byte[(int)(4096 * 2.5)];
        static byte[] part2 = new byte[(int)(2096 * 0.4)];
        static byte[] part3 = new byte[4096];
        static byte[] sequence = new byte[240];
        static byte[][] parts = new byte[][] { part1, part2, part3 };

        static MemoryStream inputStr;

        private static void FillBytes(byte[] bytes)
        {
            int length = bytes.Length;
            byte cByte = 0;
            for(int i = 0; i < bytes.Length;i++)
            {
                bytes[i] = cByte;

                cByte++;
                if (cByte == 255)
                    cByte = 0;
            }
        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            FillBytes(part1);
            FillBytes(part2);
            FillBytes(part3);
            random.NextBytes(sequence);

            using (MemoryStream str = new MemoryStream())
            {
                str.Write(part1, 0, part1.Length);
                str.Write(sequence, 0, sequence.Length);
                str.Write(part2, 0, part2.Length);
                str.Write(sequence, 0, sequence.Length);
                str.Write(part3, 0, part3.Length);
                str.Write(sequence, 0, sequence.Length);

                inputStr = new MemoryStream(str.ToArray());
            }
        }


        [TestMethod]
        public void Test()
        {
            using (PatternizedStream pStr = new PatternizedStream(inputStr, sequence))
            {
                int part = 0;
                long read = 0;
                byte[] buffer = new byte[4096];
                bool isEnd = false;

                MemoryStream str = new MemoryStream();
                while ((read = pStr.ReadTill(buffer, 0, 4096, out isEnd)) > 0)
                {

                    str.Write(buffer, 0, (int)read);
                   

                    byte[] current = str.ToArray();
                    for(int i = 0; i < current.Length; i++)
                    {
                        Assert.AreEqual(parts[part][i], current[i], "Malformed data");
                    }

                    if (isEnd)
                    {
                        Assert.IsFalse(part > 2, "Found too many parts");
                        Assert.AreEqual(parts[part].Length, str.Length, "Invalid part length");
                        Assert.IsTrue(parts[part].SequenceEqual(str.ToArray()), "Part data incorrect");
                        part++;
                        str = new MemoryStream();
                    }

                }
                Assert.IsFalse(part < 3, "Found too little parts");
            }
        }
    }
}
