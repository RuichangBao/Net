using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace Server
{
    internal class Server
    {
        private static int index;
        static void Main(string[] args)
        {
            Console.WriteLine("服务器");
            CreateAsyncServer();

            Console.WriteLine("服务器主线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
            while (true)
            {
            }
        }
        ///<summary>异步创建服务器</summary>
        private static void CreateAsyncServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            try
            {
                socket.Bind(endPoint);
                socket.Listen(100);
                socket.BeginAccept(ClientConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        ///<summary>客户端建立链接</summary>
        private static void ClientConnectCB(IAsyncResult ar)
        {
            try
            {
                index++;
                Console.WriteLine("客户端链接"+Thread.CurrentThread.ManagedThreadId+"   "+ index);
                Socket socket = ar.AsyncState as Socket;
                Socket clientSocket = socket.EndAccept(ar);
                byte[] data = Encoding.UTF8.GetBytes(index.ToString());
                clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, (IAsyncResult ar) => { }, clientSocket);
                ////异步接收数据缓存
                //byte[] data = new byte[1024];
                //AsyncReceiveData asyncReceiveData = new AsyncReceiveData { socket = clientSocket, data = data };
                //clientSocket.BeginReceive(data, 0, 1024, SocketFlags.None, AsyncReceive, asyncReceiveData);
                socket.BeginAccept(ClientConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端建立链接错误：" + ex);
            }
        }
        /// <summary>异步接受客户端消息头</summary>
        private static void AsyncReceive(IAsyncResult ar)
        {
            try
            {
                AsyncReceiveData asyncReceiveData = ar.AsyncState as AsyncReceiveData;
                Socket clientSocket = asyncReceiveData.socket;

                int length = clientSocket.EndReceive(ar);//本次接收的字节数
                if (length <= 0)
                {
                    Console.WriteLine("客户端已经下线");
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    return;
                }
                byte[] data = asyncReceiveData.data;
                byte[] resultData = new byte[length];
                Array.Copy(data, 0, resultData, 0, length);
                string str = Encoding.UTF8.GetString(resultData);
                Console.WriteLine("客户端消息：" + str);
                str = "[" + str;
                byte[] sendData = Encoding.UTF8.GetBytes(str);
                Console.WriteLine("向客户端发送消息：" + str);
                clientSocket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, (IAsyncResult ar) => { }, clientSocket);
                clientSocket.BeginReceive(data, 0, 1024, SocketFlags.None, AsyncReceive, asyncReceiveData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端非正常下线：" + ex.ToString());
            }
        }

        public class AsyncReceiveData
        {
            public Socket socket;
            public byte[] data;
        }
    }
}