using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Data;
using System.Linq.Expressions;

namespace Client
{
    internal class ClientStart
    {
        static void Main(string[] args)
        {
            Console.WriteLine("客户端主线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
            //CreateBasicClient();
            CreateAsyncClient();
            while (true) { }
        }
        private static void CreateAsyncClient()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            try
            {
                socket.BeginConnect(endPoint, new AsyncCallback(ServerConnectCB), socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        private static void ServerConnectCB(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                socket.EndConnect(ar);
                Console.WriteLine("连接服务器线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
                //接受数据缓存
                byte[] dataRcv = new byte[1024];
                AsyncReceiveData asyncReceiveData = new AsyncReceiveData { socket = socket, data = dataRcv };
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, new AsyncCallback(AsyncReceive), asyncReceiveData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        ///<summary>异步接受数据</summary>
        private static void AsyncReceive(IAsyncResult ar)
        {
            try
            {
                AsyncReceiveData asyncReceiveData = ar.AsyncState as AsyncReceiveData;
                byte[] dataRcv = asyncReceiveData.data;
                Socket socket = asyncReceiveData.socket;
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

                string serverData = Encoding.UTF8.GetString(dataRcv);
                Console.WriteLine("线程id:" + Thread.CurrentThread.ManagedThreadId.ToString() + " 数据:" + serverData);

                string write;
                do
                {
                    write = Console.ReadLine();
                } while (write.Length <= 0);

                if (write.Equals("close"))
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    return;
                }
                byte[] sendDatas = Encoding.UTF8.GetBytes(write);
                //1:
                //socket.BeginSend(sendDatas, 0, sendDatas.Length, SocketFlags.None, new AsyncCallback(AsyncSend), socket);
                //2:
                NetworkStream networkStream = null;
                try
                {
                    networkStream = new NetworkStream(socket);
                    networkStream.BeginWrite(sendDatas, 0, sendDatas.Length, new AsyncCallback(AsyncNetworkStreamSend), networkStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("异步发送数据：" + ex.ToString());
                }
                Array.Clear(dataRcv, 0, dataRcv.Length);
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, new AsyncCallback(AsyncReceive), asyncReceiveData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void AsyncSend(IAsyncResult ar)
        {
            //Socket socket = ar.AsyncState as Socket;
            //Console.WriteLine("客户端异步发送数据：");
        }
        private static void AsyncNetworkStreamSend(IAsyncResult ar)
        {
            NetworkStream networkStream = ar.AsyncState as NetworkStream;
            try
            {
                networkStream.EndWrite(ar);
                networkStream.Flush();
                networkStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("异步发送数据：" + ex.ToString());
            }
        }
        class AsyncReceiveData
        {
            public Socket socket;
            public byte[] data;
        }
        #region 同步链接
        private static void CreateBasicClient()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            socket.Connect(endPoint);
            Console.WriteLine("链接服务器成功");

            while (true)
            {
                //接受数据缓存
                //byte[] dataRcv = new byte[1024];
                //int lenRcv = socket.Receive(dataRcv);
                //string msgRcv = Encoding.UTF8.GetString(dataRcv, 0, lenRcv);
                //Console.WriteLine("收到来自服务器数据：" + msgRcv);

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
                Console.WriteLine("向服务器发送数据");
                socket.Send(Encoding.UTF8.GetBytes(write));
            }
        }
        #endregion
    }
}