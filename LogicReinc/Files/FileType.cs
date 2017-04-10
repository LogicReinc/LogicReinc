using LogicReinc.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Files
{
    public enum FileTypeUsage
    {
        Others = 0,
        Data = 1,
        Image = 2,
        Audio = 3,
        Video = 4,
        Compression = 5,
        Document = 6,
    }
    public class FileType
    {
        public static List<FileType> Types = new List<FileType>()
        {
            new FileType()
            {
                Name = "Windows Picture Information File",
                Extensions = "pic",
                MimeType = "image/pict",
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x00
                }
                }
            },
            new FileType()
            {
                Name = "Computer Icon",
                Extensions = "ico",
                MimeType = "image/x-icon",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){ new byte[]
                {
                    0x00, 0x00, 0x01, 0x00
                }
                }
            },
            new FileType()
            {
                Name = "Tar Compressed Zip Lempel-Ziv-Welch algorithm",
                Extensions = "tar.z",
                MimeType = "application/x-tar",
                Usage = FileTypeUsage.Compression,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x1F, 0x9D
                }
                }
            },
            new FileType()
            {
                Name = "Tar Compressed Zip LZH algorithm",
                Extensions = "tar.z",
                MimeType = "application/x-tar",
                Usage = FileTypeUsage.Compression,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x1F, 0xA0
                }
                }
            },
            new FileType()
            {
                Name = "Graphics Interchange Format",
                Extensions = "gif",
                MimeType = "image/gif",
                Usage = FileTypeUsage.Image,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x47, 0x49, 0x46, 0x38, 0x37, 0x61
                },
                    new byte[]
                    {
                    0x47, 0x49, 0x46, 0x38, 0x39, 0x61
                    }
                }
            },
            new FileType()
            {
                Name = "Tagged Image File Format",
                Extensions = "tif",
                MimeType = "image/tiff",
                Usage = FileTypeUsage.Image,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x49, 0x49, 0x2A, 0x00
                },
                    new byte[]
                    {
                        0x4D, 0x4D, 0x00, 0x2A
                    }
                }
            },
            new FileType()
            {
                Name = "JPEG",
                Extensions = "jpg",
                MimeType = "image/jpeg",
                Usage = FileTypeUsage.Image,
                IdentifierOffset = 0,
                IdentifierSkips = new short[]{ 4, 5 },
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0xFF, 0xD8, 0xFF, 0xDB
                },
                    new byte[]
                    {                           //nn  //nn
                        0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x00, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01
                    },
                    new byte[]
                    {                           //nn  //nn
                        0xFF, 0xD8, 0xFF, 0xE1, 0x00, 0x00, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00
                    }
                }
            },
            new FileType()
            {
                Name = "YUV Image",
                Extensions = "yuv",
                MimeType = "image/yuv",
                Usage = FileTypeUsage.Image,
                IdentifierOffset = 0,
                IdentifierSkips = new short[]{ 4, 5, 6, 7 },
                Identifiers = new List<byte[]>(){new byte[]
                {                           //nn  //nn  //nn  //nn
                    0x46, 0x4F, 0x52, 0x4D, 0x00, 0x00, 0x00, 0x00, 0x59, 0x55, 0x56, 0x4E
                }
                }
            },
            new FileType()
            {
                Name = "Audio Interchange File Format",
                Extensions = "aif",
                MimeType = "image/aif",
                Usage = FileTypeUsage.Image,
                IdentifierOffset = 0,

                IdentifierSkips = new short[]{ 4, 5, 6, 7 },
                Identifiers = new List<byte[]>(){new byte[]
                {                           //nn  //nn  //nn  //nn
                    0x46, 0x4F, 0x52, 0x4D, 0x00, 0x00, 0x00, 0x00, 0x41, 0x49, 0x46, 0x46
                }
                }
            },

            new FileType()
            {
                Name = "DOS MZ Executable",
                Extensions = "exe",
                MimeType = "",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x4D, 0x5A
                }
                }
            },
            new FileType()
            {
                Name = "Zip Type",
                Extensions = "zip", //jar, odt, ods, odp, docx, xlsx, pptx, vsdx, apk
                MimeType = "application/zip",
                Usage = FileTypeUsage.Compression,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },
                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },
                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
                }
            },
            new FileType()
            {
                Name = "OpenXML Document",
                Extensions = "docx", //jar, odt, ods, odp, docx, xlsx, pptx, vsdx, apk
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },
                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },
                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
                }
            },
            new FileType()
            {
                Name = "OpenXML Presentation",
                Extensions = "pptx", //jar, odt, ods, odp, docx, xlsx, pptx, vsdx, apk
                MimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },
                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },
                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
                }
            },
            new FileType()
            {
                Name = "OpenXML Spreadsheet",
                Extensions = "xlsx", //jar, odt, ods, odp, docx, xlsx, pptx, vsdx, apk
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },
                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },
                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
                }
            },
            new FileType()
            {
                Name = "OpenDocument Text",
                Extensions = "odt", //jar, odt, ods, odp, docx, xlsx, pptx, vsdx, apk
                MimeType = "application/vnd.oasis.opendocument.text",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },
                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },
                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
                }
            },
            new FileType()
            {
                Name = "Android Package (APK)",
                Extensions = "apk", //jar, odt, ods, odp, docx, xlsx, pptx, vsdx, apk
                MimeType = "application/vnd.android.package-archive",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },
                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },
                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
                }
            },
            new FileType()
            {
                Name = "Java Archive (JAR)",
                Extensions = "jar", //jar, odt, ods, odp, docx, xlsx, pptx, vsdx, apk
                MimeType = "application/java-archive",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x50, 0x4B, 0x03, 0x04
                },
                new byte[]
                {
                    0x50, 0x4B, 0x05, 0x06
                },
                new byte[]
                {
                    0x50, 0x4B, 0x07, 0x08
                }
                }
            },





            new FileType()
            {
                Name = "RAR Archive",
                Extensions = "rar",
                MimeType = "application/rar",
                Usage = FileTypeUsage.Compression,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00
                },
                    new byte[]
                    {
                        0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00
                    }
                }
            },
            new FileType()
            {
                Name = "Portable Network Graphics",
                Extensions = "png",
                MimeType = "image/png",
                Usage = FileTypeUsage.Image,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
                }
                }
            },
            new FileType()
            {
                Name = "PostScript Document",
                Extensions = "ps",
                MimeType = "application/postscript",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x25, 0x21, 0x50, 0x53
                }
                }
            },
            new FileType()
            {
                Name = "PDF Document",
                Extensions = "pdf",
                MimeType = "application/pdf",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x25, 0x50, 0x44, 0x46
                }
                }
            },
            new FileType()
            {
                Name = "Advanced System Format (WMA, WMV, ASF)",
                Extensions = "wmv",
                MimeType = "video/wmv",
                Usage = FileTypeUsage.Video,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C
                }
                }
            },
            new FileType()
            {
                Name = "Ogg Media",
                Extensions = "ogg",
                MimeType = "audio/ogg",
                Usage = FileTypeUsage.Audio,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x4F, 0x67, 0x67, 0x53
                }
                }
            },
            new FileType()
            {
                Name = "Photoshop Document",
                Extensions = "psd",
                MimeType = "application/photoshop",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x38, 0x42, 0x50, 0x53
                }
                }
            },
            new FileType()
            {
                Name = "Waveform Audio",
                Extensions = "wav",
                MimeType = "audio/wave",
                Usage = FileTypeUsage.Audio,
                IdentifierOffset = 0,
                IdentifierSkips = new short[]{ 4, 5, 6, 7 },
                Identifiers = new List<byte[]>(){new byte[]
                {                          //nn  //nn  //nn  //nn      
                   0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45
                }
                }
            },
            new FileType()
            {
                Name = "Audio Video Interleave",
                Extensions = "avi",
                MimeType = "video/avi",
                Usage = FileTypeUsage.Video,
                IdentifierOffset = 0,
                IdentifierSkips = new short[]{ 4, 5, 6, 7 },
                Identifiers = new List<byte[]>(){new byte[]
                {                         //nn  //nn  //nn  //nn
                    0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x41, 0x56, 0x49, 0x20
                }
                }
            },
            new FileType()
            {
                Name = "MP3",
                Extensions = "mp3",
                MimeType = "audio/mp3",
                Usage = FileTypeUsage.Audio,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0xFF, 0xFB
                },
                    new byte[]
                    {
                        0x49, 0x44, 0x33
                    }
                }
            },
            new FileType()
            {
                Name = "Bitmap",
                Extensions = "bmp",
                MimeType = "image/bitmap",
                Usage = FileTypeUsage.Image,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x42, 0x4D
                }
                }
            },
            new FileType()
            {
                Name = "Free Lossless Audio Codec",
                Extensions = "flac",
                MimeType = "audio/flac",
                Usage = FileTypeUsage.Audio,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x66, 0x4C, 0x61, 0x43
                }
                }
            },
            new FileType()
            {
                Name = "MIDI Sound",
                Extensions = "midi",
                MimeType = "audio/midi",
                Usage = FileTypeUsage.Audio,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x4D, 0x54, 0x68, 0x64
                }
                }
            },

            new FileType()
            {
                Name = "Word Document (Compound File Binary)",
                Extensions = "doc",
                MimeType = "application/msword",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1
                }
                }
            },
            new FileType()
            {
                Name = "Excel Document (Compound File Binary)",
                Extensions = "xls",
                MimeType = "application/vnd.ms-excel",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1
                }
                }
            },
            new FileType()
            {
                Name = "Powerpoint Document (Compound File Binary)",
                Extensions = "ppt",
                MimeType = "application/vnd.ms-powerpoint",
                Usage = FileTypeUsage.Document,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1
                }
                }
            },
            new FileType()
            {
                Name = "Google Chrome Extension",
                Extensions = "crx",
                MimeType = "application/x-chrome-extension",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x43, 0x72, 0x32, 0x34
                }
                }
            },
            new FileType()
            {
                Name = "Apple Disk Image",
                Extensions = "dmg",
                MimeType = "application/x-apple-diskimage",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x78, 0x01, 0x73, 0x0D, 0x62, 0x62, 0x60
                }
                }
            },
            new FileType()
            {
                Name = "Nintendo Entertainment System ROM",
                Extensions = ".nes",
                MimeType = "",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x4E, 0x45, 0x53, 0x1A
                }
                }
            },
            new FileType()
            {
                Name = "TAR",
                Extensions = "tar",
                MimeType = "application/x-tar",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x74, 0x6F, 0x78, 0x33
                }
                }
            },
            new FileType()
            {
                Name = "7Zip",
                Extensions = "7z",
                MimeType = "application/x-7z-compressed",
                Usage = FileTypeUsage.Compression,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C
                }
                }
            },
            new FileType()
            {
                Name = "GZIP",
                Extensions = "tar.gz",
                MimeType = "application/gzip",
                Usage = FileTypeUsage.Compression,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x1F, 0x8B
                }
                }
            },
            new FileType()
            {
                Name = "Microsoft Cabinet",
                Extensions = "cab",
                MimeType = "",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x4D, 0x53, 0x43, 0x46
                }
                }
            },
            new FileType()
            {
                Name = "Matroska Media",
                Extensions = "mkv",
                MimeType = "video/x-matroska",
                Usage = FileTypeUsage.Video,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x1A, 0x45, 0xDF, 0xA3
                }
                }
            },
            new FileType()
            {
                Name = "MPEG 4",
                Extensions = "mp4",
                MimeType = "video/mp4",
                Usage = FileTypeUsage.Video,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32
                }
                }
            },
            new FileType()
            {
                Name = "XML",
                Extensions = "xml",
                MimeType = "text/xml",
                Usage = FileTypeUsage.Data,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x3c, 0x3f, 0x78, 0x6d, 0x6c, 0x20
                }
                }
            },
            new FileType()
            {
                Name = "Flash",
                Extensions = "swf",
                MimeType = "application/x-shockwave-flash",
                Usage = FileTypeUsage.Others,
                IdentifierOffset = 0,
                Identifiers = new List<byte[]>(){new byte[]
                {
                    0x43, 0x57, 0x53
                }
                }
            }
        };


        public string Name { get; set; }
        public string Extensions { get; set; }
        public string MimeType { get; set; }
        public FileTypeUsage Usage { get; set; }

        public int IdentifierOffset { get; set; }
        public short[] IdentifierSkips { get; set; }
        public List<byte[]> Identifiers { get; set; }

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

        public static List<FileType> GetFileTypeFromStream(Stream str)
        {
            int max = Types.Max(x => x.Identifiers.Max(y => y.Length));
            byte[] bs = new byte[max];
            int size = str.Read(bs, 0, bs.Length);

            List<FileType> types = new List<FileType>();

            foreach (FileType type in Types)
            {
                bool isMatch = false;
                foreach (byte[] identifier in type.Identifiers)
                {
                    if (identifier.Length > size)
                        continue;

                    bool matching = true;
                    for (short i = 0; i < identifier.Length; i++)
                    {
                        if (type.IdentifierSkips.Contains(i))
                            continue;
                        if(identifier[i] != bs[i])
                        {
                            matching = false;
                            break;
                        }
                    }

                    if (matching)
                    {
                        isMatch = true;
                        break;
                    }
                }

                if (isMatch)
                    types.Add(type);
            }

            return types;
        }
    }
}
