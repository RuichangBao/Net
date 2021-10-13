using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleServer
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

    class NetManager2
    {
        private const string ip = "127.0.0.1";
        private const int port = 8899;

        private UdpClient udpcRecv = null;
        private UdpClient udpcRecv2 = null;
        private IPEndPoint localIpep = null;

        public NetManager2()
        {
            localIpep = new IPEndPoint(IPAddress.Parse(ip), port); // 本机IP和监听端口号
            udpcRecv = new UdpClient(localIpep);
            udpcRecv2 = new UdpClient();
            Thread thrRecv = new Thread(ReceiveMessage);
            thrRecv.Start();

            Console.WriteLine("NetManager2 监听器已成功启动");
        }

 
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveMessage(object obj)
        {
            while (true)
            {
                try
                {
                    IPEndPoint clienIPEndPoint=null;
                    byte[] bytRecv = udpcRecv.Receive(ref clienIPEndPoint);
                    Console.WriteLine("收到客户端 消息 端口号"+ clienIPEndPoint.Port);
                    string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);

                    SendMessage(bytRecv, clienIPEndPoint);
                    Console.WriteLine(string.Format("{0}[{1}]", clienIPEndPoint, message));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }

        public void SendMessage(byte[] data, IPEndPoint iPEndPoint)
        {
            Console.WriteLine("向客户端发送消息："+ iPEndPoint.Port+"    "+ iPEndPoint.Address);
            udpcRecv.Send(data, data.Length, iPEndPoint);
        }

    }
}