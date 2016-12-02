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
    }
}
