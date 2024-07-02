using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    internal class Client
    {
        private static Socket socket;
        static void Main(string[] args)
        {
            Console.WriteLine("客户端主线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
            CreateAsyncClient();
            while (true) { }
        }
        ///<summary>创建客户端链接</summary>
        private static void CreateAsyncClient()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            try
            {
                socket.BeginConnect(endPoint, ServerConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        ///<summary>异步链接到服务器</summary>
        private static void ServerConnectCB(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                socket.EndConnect(ar);
                Console.WriteLine("连接服务器线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());

                //接受数据缓存
                byte[] dataRcv = new byte[1024];
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, dataRcv);
                Console.WriteLine("可以发送消息");
                byte[] sendData = Encoding.UTF8.GetBytes("AA");
                string str = Encoding.UTF8.GetString(sendData);
                Console.WriteLine("AAAAAAAAA" + str);
                socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, AsyncSend, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void AsyncSend(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            string consoleRead;
            do
            {
                consoleRead = Console.ReadLine();
            } while (consoleRead.Length <= 0);
            if (consoleRead.Equals("close"))
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return;
            }
            byte[] data = Encoding.UTF8.GetBytes(consoleRead);
            Console.WriteLine("客户端向服务器发送数据" + consoleRead);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, AsyncSend, socket);
        }
        ///<summary>异步接受数据</summary>
        private static void AsyncReceive(IAsyncResult ar)
        {
            try
            {
                byte[] dataRcv = ar.AsyncState as byte[];
                int length = socket.EndReceive(ar);
                if (length <= 0)
                {
                    Console.WriteLine("服务器已经关闭");
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    return;
                }
                byte[] resultData = new byte[length];
                Array.Copy(dataRcv, 0, resultData, 0, length);

                string obj = Encoding.UTF8.GetString(resultData);
                Console.WriteLine("服务器消息:" + obj.ToString());
                Array.Clear(dataRcv, 0, dataRcv.Length);
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, dataRcv);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}