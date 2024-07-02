using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    internal class Client
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                Thread thread = new Thread(CreateAsyncClient);
                thread.Start();
            }
           
            //}
            while (true) { }
        }
        ///<summary>创建客户端链接</summary>
        private static void CreateAsyncClient()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                //Console.WriteLine("连接服务器线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());

                //接受数据缓存
                byte[] dataRcv = new byte[1024];
                AsyncReceiveData asyncReceiveData = new AsyncReceiveData { socket = socket, data = dataRcv };
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, asyncReceiveData);
                //byte[] sendData = Encoding.UTF8.GetBytes("AA");
                //string str = Encoding.UTF8.GetString(sendData);
                //Console.WriteLine("AAAAAAAAA" + str);
                //socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, AsyncSend, socket);
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
                AsyncReceiveData asyncReceiveData = ar.AsyncState as AsyncReceiveData;
                Socket socket = asyncReceiveData.socket;
                byte[] dataRcv = asyncReceiveData.data;
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
                Console.WriteLine(obj.ToString());
                Array.Clear(dataRcv, 0, dataRcv.Length);
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, asyncReceiveData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    public class AsyncReceiveData
    {
        public Socket socket;
        public byte[] data;
    }
}