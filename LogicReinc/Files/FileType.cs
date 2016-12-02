using LogicReinc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Files
{
    public static class FileType
    {
        private static Dictionary<byte[], string> magicBytes = new Dictionary<byte[], string>()
        {
            { new byte[]{ 0x42, 0x4D }, "image/bmp"},
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, "image/gif" },
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, "image/gif"},
            { new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png" },
            { new byte[]{ 0xff, 0xd8 }, "image/jpeg" },
        };

        public static string GetImageMime(byte[] bytes)
        {
            string mime = magicBytes.FirstOrDefault(x => bytes.StartsWith(x.Key)).Value;
            return mime;
        }
    }
}
