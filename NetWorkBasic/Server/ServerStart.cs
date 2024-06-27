using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class SKTParams
    {
        public Socket skt;
        public string info;
    }
    internal class ServerStart
    {
        private static uint index;
        static void Main(string[] args)
        {
            Console.WriteLine("服务器");
            //CreateBasicServer();
            CreateAsyncServer();
        }
        private static void CreateBasicServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            socket.Bind(endPoint);
            socket.Listen(100);
            Console.WriteLine("服务器已经启动");

            while (true)
            {
                Socket clientSocket = socket.Accept();
                Thread thread = new Thread(NewClientLink);
                thread.Start(clientSocket);
                index++;
                Console.WriteLine($"客户端{index}链接成功");
            }
        }


        private static void CreateAsyncServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            socket.Bind(endPoint);
            socket.Listen(100);
            socket.BeginAccept(new AsyncCallback(ClientConnectCB), new SKTParams { skt = socket, info = "test" });
            while (true)
            {

            }
        }

        private static void ClientConnectCB(IAsyncResult ar)
        {
            index++;
            SKTParams sktParams = ar.AsyncState as SKTParams;
            Socket socket = sktParams.skt;
            Socket clientSocket = socket.EndAccept(ar);
            socket.BeginAccept(new AsyncCallback(ClientConnectCB), new SKTParams { skt = socket, info = "test" });
            clientSocket.Send(Encoding.UTF8.GetBytes("客户端链接成功"));
            //接收数据缓存
            while (true)
            {
                try
                {
                    byte[] dataRcv = new byte[1024];
                    int lenRcv = clientSocket.Receive(dataRcv);
                    if (lenRcv <= 0)
                    {
                        Console.WriteLine("客户端已经断开链接");
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        return;
                    }
                    string msgRcv = Encoding.UTF8.GetString(dataRcv, 0, lenRcv);
                    Console.WriteLine($"客户端{clientSocket.LocalEndPoint}index:{index}数据：" + msgRcv);
                    clientSocket.Send(Encoding.UTF8.GetBytes("服务器向客户端发送数据:" + msgRcv));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("服务器主动关闭客户端");
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    return;
                }

            }
        }

        private static void NewClientLink(object? obj)
        {
            Socket clientSocket = obj as Socket;
            clientSocket.Send(Encoding.UTF8.GetBytes("客户端链接成功"));

            //接收数据缓存
            while (true)
            {
                try
                {
                    byte[] dataRcv = new byte[1024];
                    int lenRcv = clientSocket.Receive(dataRcv);
                    if (lenRcv <= 0)
                    {
                        Console.WriteLine("客户端已经断开链接");
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        return;
                    }
                    string msgRcv = Encoding.UTF8.GetString(dataRcv, 0, lenRcv);
                    Console.WriteLine($"客户端{clientSocket.LocalEndPoint}index:{index}数据：" + msgRcv);
                    clientSocket.Send(Encoding.UTF8.GetBytes("服务器向客户端发送数据:" + msgRcv));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("服务器主动关闭客户端");
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    return;
                }

            }
        }

        //private static void ClientConnectCB(Socket ar)
        //{

        //}
    }
}