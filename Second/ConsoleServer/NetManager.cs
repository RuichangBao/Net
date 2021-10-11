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

    class Test
    {
        /// <summary>
        /// 用于UDP发送的网络服务类
        /// </summary>
        static UdpClient udpcRecv = null;
        static IPEndPoint localIpep = null;

        /// <summary>
        /// 线程：不断监听UDP报文
        /// </summary>
        static Thread thrRecv;

        public Test()
        {
            localIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8899); // 本机IP和监听端口号
            udpcRecv = new UdpClient(localIpep);
            thrRecv = new Thread(ReceiveMessage);
            thrRecv.Start();

            Console.WriteLine("UDP监听器已成功启动");
        }

 
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private static void ReceiveMessage(object obj)
        {
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref localIpep);
                    string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);
                    Console.WriteLine(string.Format("{0}[{1}]", localIpep, message));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }

    }



    class NetManager
    {
        static Socket server;

        public NetManager()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8899));//绑定端口号和IP
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