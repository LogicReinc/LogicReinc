using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Utility
{
    public static class Resources
    {
        public static byte[] ReadData(Assembly assembly, string path)
        {
            byte[] data;
            using (Stream str = assembly.GetManifestResourceStream(path))
            {
                data = new byte[str.Length];
                str.Read(data, 0, (int)str.Length);
            }
            return data;
        }
    }
}
