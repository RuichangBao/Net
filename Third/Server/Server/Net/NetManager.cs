using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class NetManager : Singleton<NetManager>, Init
    {
        private int serverPort = 2023;
        TcpListener server;
        TcpClient client;
        NetworkStream stream;
        public void Init()
        {

        }
        public void Start()
        {
            server = new TcpListener(IPAddress.Any, serverPort);
            server.Start();
            client = server.AcceptTcpClient();
            stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    this.AAA(buffer);
                    //Console.WriteLine(server.);

                    this.SendMessage();
                }
            }
        }
        private void AAA(byte[] data)
        {
            Console.WriteLine(data.Length);
            CRuningBuff cRuningBuff = new CRuningBuff(data);
            data = cRuningBuff.data;
            TestRequest message = new TestRequest();
            message.MergeFrom(data, 0, cRuningBuff.length - sizeof(int) * 2);
            Console.WriteLine(message.Num1);
            Console.WriteLine(message.Num2);
            Console.WriteLine(message.Str1);

            Console.WriteLine("向客户端发送消息");
            //MessageExtensions.WriteTo
        }

        private void SendMessage()
        {
            Console.WriteLine("服务器向客户端发送消息");
            TestResponse response = new TestResponse
            {
                Num1 = 1,
                Num2 = 2,
                Str1 = "sdfdsfsd"
            };
            CRuningBuff cRuningBuff = new CRuningBuff(MsgType.TestResponse, response);
            byte[] data = cRuningBuff.GetBytes();
            Console.WriteLine(data.Length);
            stream.BeginWrite(data, 0, data.Length, HandleDatagramWritten, client);
        }
        private void HandleDatagramWritten(IAsyncResult ar)
        {
        }
    }
}