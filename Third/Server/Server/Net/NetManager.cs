using Google.Protobuf;
using Microsoft.VisualBasic;
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
    public class NetManager : Singleton<NetManager>, IInit
    {
        class TestClass
        {
            public byte[] data = new byte[2048];
        }
        private int serverPort = 2023;
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private CBuffer cRequestBuff;
        private CBuffer cResponseBuff;
        public void Init()
        {
            cRequestBuff = new CBuffer();
            cResponseBuff = new CBuffer();
        }
        public void Start()
        {
            tcpListener = new TcpListener(IPAddress.Any, serverPort);
            tcpListener.Start();
            tcpClient = tcpListener.AcceptTcpClient();
            networkStream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    this.AnalyzeRequest(buffer);
                    Console.WriteLine(tcpClient);
                    this.SendMessage();
                }
            }
        }
        /// <summary>
        /// 解析Request
        /// </summary>
        /// <param name="data"></param>
        private void AnalyzeRequest(byte[] data)
        {
            cRequestBuff.Update(data);
            data = cRequestBuff.data;
            TestRequest message = new TestRequest();
            message.MergeFrom(data, 0, cRequestBuff.length);
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
            cResponseBuff.Update(MsgType.TestResponse, response);
            byte[] data = cResponseBuff.GetBytes();
            int sendLength = cResponseBuff.GetSendLength();
            networkStream.BeginWrite(data, 0, sendLength, HandleDatagramWritten, tcpClient);
        }
        private void HandleDatagramWritten(IAsyncResult ar)
        {
        }
    }
}