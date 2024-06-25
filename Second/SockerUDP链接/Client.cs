using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] data = new byte[1024];
            string input, stringData;

            //构建TCP 服务器
            Console.WriteLine("这是一个客户端, host name is {0}", Dns.GetHostName());

            //设置服务IP，设置TCP端口号
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);

            //定义网络类型，数据连接类型和网络协议UDP
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            string welcome = "你好! ";
            data = Encoding.UTF8.GetBytes(welcome);
            server.SendTo(data, data.Length, SocketFlags.None, ip);

            EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            data = new byte[1024];
            int recv = 0;

            try
            {
                recv = server.ReceiveFrom(data, ref sender);
                Console.WriteLine("收到服务器消息{0}: ", sender.ToString());
                Console.WriteLine(Encoding.UTF8.GetString(data, 0, recv));
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            while (true)
            {
                input = Console.ReadLine();
                if (input == "exit")
                    break;

                server.SendTo(Encoding.UTF8.GetBytes(input), sender);
                data = new byte[1024];
                recv = server.ReceiveFrom(data, ref sender);
                stringData = Encoding.UTF8.GetString(data, 0, recv);
                Console.WriteLine("收到服务器消息："+stringData);
            }

            server.Close();
            Console.WriteLine("Stopping Client.");
            Console.ReadKey();
        }
    }
}