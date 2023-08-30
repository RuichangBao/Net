using Google.Protobuf;
using Protocol;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class NetManager : Singleton<NetManager>, IInit
    {
        //private int clientPort = 2022;//客户端端口号
        private int serverPort = 2023;//服务器端口号;
        //private string clientIP = "127.0.0.1";
        private string serverIP = "127.0.0.1";
        private UdpClient udpClient;
        private CBuffer cRequestBuff;
        private CBuffer cResponseBuff;
        IPEndPoint iPEndPoint;
        public void Init()
        {
            cRequestBuff = new CBuffer();
            cResponseBuff = new CBuffer();
        }
        public void Start()
        {
            IPAddress iPAddress = IPAddress.Parse(serverIP);
            iPEndPoint = new IPEndPoint(iPAddress, serverPort);
            udpClient = new UdpClient(iPEndPoint);
            Thread thread = new Thread(Receive);
            thread.Start();
        }

        private void Receive()
        {
            while (true)
            {
                byte[] data = udpClient.Receive(ref iPEndPoint);                
                Console.WriteLine(iPEndPoint);
                Console.WriteLine("收到来自客户端的消息:"+ iPEndPoint);
                AnalyzeRequest(data);
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
            SendMessage();
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
            udpClient.Send(data, data.Length, iPEndPoint);
        }
    }
}