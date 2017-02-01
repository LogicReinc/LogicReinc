using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Extensions
{
    public static class ByteArrayExtensions
    {
        public static bool StartsWith(this byte[] bytes, byte[] with)
        {
            if (with.Length > bytes.Length)
                return false;

            for (int i = 0; i < with.Length; i++)
                if (bytes[i] != with[i])
                    return false;
            return true;
        }

        public static int FindSequence(this byte[] bytes, byte[] sequence, int start = 0)
        {
            int end = bytes.Length - sequence.Length;
            byte firstByte = sequence[0];

            while (start < end)
            {
                if (bytes[start] == firstByte)
                {
                    for (int offset = 1; offset < sequence.Length; ++offset)
                    {
                        if (bytes[start + offset] != sequence[offset])
                        {
                            break;
                        }
                        else if (offset == sequence.Length - 1)
                        {
                            return start;
                        }
                    }
                }
                ++start;
            }
            return -1;
        }
    }
}
