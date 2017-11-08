using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Network
{
    public class TcpHttpClient
    {
        public TcpClient Client { get; private set; }

        public string Method { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public List<Header> Headers { get; private set; } = new List<Header>()
        {
            new Header("Connection", "keep-alive")
        };

        public NetworkStream Stream { get; private set; }

        public TcpHttpClient()
        {
        }

        public void AddHeader(string key, string value)
        {
            Headers.Add(new Header(key, value));
        }


        public string GetRequestString()
        {
            return $"{Method} {Path} HTTP/1.1";
        }

        public string BuildHeader()
        {
            return GetRequestString() + "\r\n" + Header.Format(Headers) + "\r\n";
        }

        public void Connect()
        {
            Client = new TcpClient();

            byte[] header = Encoding.UTF8.GetBytes(BuildHeader());

            Client.Connect(Host, Port);

            Stream = Client.GetStream();

            Stream.Write(header, 0, header.Length);
        }

        public NetworkStream GetRequestStream()
        {
            return Stream;
        }

        public NetworkStream GetResponseStream()
        {
            Stream.Flush();
            return Stream;
        }


        public class Header
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public Header(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string GetText()
            {
                return $"{Key}: {Value}";
            }

            public static string Format(List<Header> headers)
            {
                StringBuilder builder = new StringBuilder();
                foreach(Header header in headers)
                {
                    builder.AppendLine(header.GetText());
                }
                return builder.ToString();
            }
        }
    }
}
