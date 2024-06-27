﻿using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    internal class ClientStart
    {
        static void Main(string[] args)
        {
            CreateBasicClient();
        }
        private static void CreateBasicClient()
        {
            Console.WriteLine("客户端");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            socket.Connect(endPoint);
            Console.WriteLine("链接服务器成功");

            while (true)
            {
                //接受数据缓存
                byte[] dataRcv = new byte[1024];
                int lenRcv = socket.Receive(dataRcv);
                string msgRcv = Encoding.UTF8.GetString(dataRcv, 0, lenRcv);
                Console.WriteLine("收到来自服务器数据：" + msgRcv);

                string write;
                do
                {
                    write = Console.ReadLine();
                } while (write.Length <= 0);

                if (write.Equals("close"))
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    break;
                }
                socket.Send(Encoding.UTF8.GetBytes(write));
            }
        }
    }
}