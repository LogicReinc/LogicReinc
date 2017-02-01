using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadToEnd(this Stream stream)
        {
            byte[] buffer = new byte[4096];
            using (MemoryStream str = new MemoryStream())
            {
                int read = 0;
                while((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    str.Write(buffer, 0, read);
                return str.ToArray();
            }
        }

        public static int WriteTo(this Stream stream, Stream to, int bufferSize, int index, int length)
        {
            byte[] buffer = new byte[bufferSize];
            int read = 0;
            int leftToRead = 0;

            if (index > 0)
                stream.Seek(index, SeekOrigin.Begin);
            while((read = stream.Read(buffer, 0, (leftToRead > buffer.Length) ? bufferSize : leftToRead)) > 0)
            {
                to.Write(buffer, 0, read);
                leftToRead -= read;
            }
            return length - leftToRead;
        }


        public static byte[] ReadTill(this Stream stream, byte[] sequence)
        {
            byte[] result = null;

            int seekIndex = 0;
            byte seekNext = sequence[0];
            //Build Header
            using (MemoryStream headerstr = new MemoryStream())
            {
                int read;
                byte[] stack = new byte[sequence.Length];
                byte[] sbBuffer = new byte[1];
                while ((read = stream.Read(sbBuffer, 0, 1)) != 0)
                {
                    byte currentByte = sbBuffer[0];
                    if (currentByte == seekNext)
                    {
                        stack[seekIndex] = (byte)currentByte;
                        if (sequence.Length == seekIndex + 1)
                        {
                            //Found splitter
                            break;
                        }
                        seekIndex++;
                        seekNext = sequence[seekIndex];
                    }
                    else
                    {
                        if (seekIndex > 0)
                        {
                            headerstr.Write(stack, 0, seekIndex);
                            stack = new byte[sequence.Length];
                            seekIndex = 0;
                            seekNext = sequence[0];
                        }
                        headerstr.Write(sbBuffer, 0, 1);
                    }
                }
                result = headerstr.ToArray();
            }
            return result;
        }

        public static long ReadTill(this Stream stream, byte[] sequence, byte[] buffer, int offset, int length, out bool isEnd)
        {
            isEnd = false;
            int seekIndex = 0;
            byte seekNext = sequence[0];
            //Build Header
            using (MemoryStream headerstr = new MemoryStream())
            {
                int read;
                byte[] stack = new byte[sequence.Length];
                byte[] sbBuffer = new byte[1];
                while (headerstr.Length + sequence.Length < length && (read = stream.Read(sbBuffer, 0, 1)) != 0)
                {
                    byte currentByte = sbBuffer[0];


                    if(currentByte != seekNext && seekIndex > 0)
                    {
                            headerstr.Write(stack, 0, seekIndex);
                            stack = new byte[sequence.Length];
                            seekIndex = 0;
                            seekNext = sequence[0];
                    }
                    if (currentByte == seekNext)
                    {
                        stack[seekIndex] = (byte)currentByte;
                        if (sequence.Length == seekIndex + 1)
                        {
                            isEnd = true;
                            break;
                        }
                        seekIndex++;
                        seekNext = sequence[seekIndex];
                    }
                    else
                        headerstr.Write(sbBuffer, 0, 1);
                }
                headerstr.ToArray().CopyTo(buffer, offset);
                return headerstr.Length;
            }
            
        }
    }
}
