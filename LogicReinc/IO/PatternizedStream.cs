using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.IO
{
    public class PatternizedStream : IDisposable
    {
        public Stream Stream { get; private set; }

        public bool IsEnd { get; private set; }

        private MemoryStream CurrentBuffer { get; set; }
        private MemoryStream OutputBuffer { get; set; }

        private int _seekIndex = 0;
        private byte _seekNext;
        private byte[] _stack;
        private byte[] _sbBuffer = new byte[1];
        private byte[] _sequence;
        private int _bufferSize;

        public PatternizedStream(Stream stream, byte[] sequence, int bufferSize = 4096)
        {
            Stream = stream;
            _seekNext = sequence[0];
            _stack = new byte[sequence.Length];
            this._sequence = sequence;
            this._bufferSize = bufferSize;
        }

        public bool UpdateBuffer()
        {
            if (CurrentBuffer != null)
                CurrentBuffer.Dispose();

            byte[] buffer = new byte[_bufferSize];
            int read = Stream.Read(buffer, 0, _bufferSize);

            if (read > 0)
            {
                CurrentBuffer = new MemoryStream(buffer, 0, read);
            }
            else
                IsEnd = true;

            return !IsEnd;
        }

        public byte[] ReadTill(byte[] splitter)
        {
            byte[] result = null;

            int seekIndex = 0;
            byte seekNext = splitter[0];
            //Build Header
            using (MemoryStream outStr = new MemoryStream())
            {
                int read;
                byte[] stack = new byte[splitter.Length];
                byte[] sbBuffer = new byte[1];
                while (true)
                {
                    if (CurrentBuffer == null && !UpdateBuffer())
                    {
                        return outStr.ToArray();
                    }
                    if ((read = CurrentBuffer.Read(sbBuffer, 0, 1)) == 0)
                    {
                        if (!UpdateBuffer())
                        {
                            return outStr.ToArray();
                        }
                        else if ((read = CurrentBuffer.Read(sbBuffer, 0, 1)) == 0)
                        {
                            throw new Exception("Stream ended while it shouldn't");
                        }
                    }


                    byte currentByte = sbBuffer[0];
                    if (currentByte == seekNext)
                    {
                        stack[seekIndex] = (byte)currentByte;
                        if (splitter.Length == seekIndex + 1)
                        {
                            //Found splitter
                            break;
                        }
                        seekIndex++;
                        seekNext = splitter[seekIndex];
                    }
                    else
                    {
                        if (seekIndex > 0)
                        {
                            outStr.Write(stack, 0, seekIndex);
                            stack = new byte[splitter.Length];
                            seekIndex = 0;
                            seekNext = splitter[0];
                        }
                        outStr.Write(sbBuffer, 0, 1);
                    }
                }
                result = outStr.ToArray();
            }
            return result;
        }

        public long ReadTill(byte[] buffer, int offset, int length, out bool isEnd)
        {
            isEnd = false;
            using (MemoryStream outStr = new MemoryStream())
            {
                int read;
                while (outStr.Length + _sequence.Length < length)
                {
                    if (CurrentBuffer == null && !UpdateBuffer())
                    {
                        isEnd = true;
                        break;
                    }
                    if ((read = CurrentBuffer.Read(_sbBuffer, 0, 1)) == 0)
                    {
                        if(!UpdateBuffer())
                        {
                            isEnd = true;
                            break;
                        }
                        else if((read = CurrentBuffer.Read(_sbBuffer, 0, 1)) == 0)
                        {
                            throw new Exception("Stream ended while it shouldn't");
                        }
                    }

                    byte currentByte = _sbBuffer[0];

                    if (currentByte != _seekNext && _seekIndex > 0)
                    {
                        outStr.Write(_stack, 0, _seekIndex);
                        _stack = new byte[_sequence.Length];
                        _seekIndex = 0;
                        _seekNext = _sequence[0];
                    }
                    if (currentByte == _seekNext)
                    {
                        _stack[_seekIndex] = (byte)currentByte;
                        if (_sequence.Length == _seekIndex + 1)
                        {
                            isEnd = true;
                            _seekIndex = 0;
                            _seekNext = _sequence[0];
                            _stack = new byte[_sequence.Length];
                            break;
                        }
                        _seekIndex++;
                        _seekNext = _sequence[_seekIndex];
                    }
                    else
                        outStr.Write(_sbBuffer, 0, 1);
                }
                outStr.ToArray().CopyTo(buffer, offset);
                return outStr.Length;
            }
        }




        public void Dispose()
        {
            if (CurrentBuffer != null)
                CurrentBuffer.Close();
            if (OutputBuffer != null)
                OutputBuffer.Close();
        }
    }
}
