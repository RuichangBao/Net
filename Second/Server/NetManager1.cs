using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Server
{

    class NetManager1
    {
        private const string ip = "127.0.0.1";
        private const int port = 8899;

        static Socket server;
        //private int index = 0;
        public NetManager1()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));//绑定端口号和IP
            Thread threadReceive = new Thread(new ThreadStart(Receive));  //Thread1是你新线程的函数
            threadReceive.Start();

        }
        private void Receive()
        {
            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                byte[] buffer = new byte[1024];
                int length = server.ReceiveFrom(buffer, ref point);//接收数据报
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Console.WriteLine(message.Length);
                string content = point.ToString() + message;
                Console.WriteLine(content);
            }
        }
    }

}