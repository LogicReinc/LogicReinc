using LogicReinc.Extensions;
using LogicReinc.Files;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Images
{
    public static class Images
    {
        
        public static string ToWebBase64(byte[] bytes)
        {
            return $"data:{FileType.GetImageMime(bytes)};base64,{Convert.ToBase64String(bytes)}";
        }
    }
}
