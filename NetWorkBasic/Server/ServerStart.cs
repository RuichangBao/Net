using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    internal class ServerStart
    {

        static void Main(string[] args)
        {
            Console.WriteLine("服务器");
            CreateAsyncServer();
            Console.WriteLine("服务器主线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
            while (true)
            {

            }
        }

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
        private static void ClientConnectCB(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("客户端链接线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
                Socket socket = ar.AsyncState as Socket;
                Socket clientSocket = socket.EndAccept(ar); 
                byte[] sendDatas = Encoding.UTF8.GetBytes("客户端链接成功");
                //clientSocket.Send(sendDatas);
                clientSocket.BeginSend(sendDatas, 0, sendDatas.Length, SocketFlags.None, AsyncSend, clientSocket);
                //异步接收数据缓存
                byte[] dataRcv = new byte[1024];
                AsyncReceiveData asyncReceiveData = new AsyncReceiveData { socket = clientSocket, data = dataRcv };
                clientSocket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, asyncReceiveData);
                socket.BeginAccept(ClientConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void AsyncReceive(IAsyncResult ar)
        {
            try
            {
                AsyncReceiveData asyncReceiveData = ar.AsyncState as AsyncReceiveData;
                Socket clientSocket = asyncReceiveData.socket;
                byte[] dataRcv = asyncReceiveData.data;
                int length = clientSocket.EndReceive(ar);//接受的字节数
                if (length <= 0)
                {
                    Console.WriteLine("客户端已经下线");
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    return;
                }
                byte[] resultData = new byte[length];
                Array.Copy(dataRcv, 0, resultData, 0, length);
                string clientData = Encoding.UTF8.GetString(resultData);
                Console.WriteLine("接收消息，线程id:" + Thread.CurrentThread.ManagedThreadId.ToString() + "   内容：" + clientData);
                byte[] sendDatas = Encoding.UTF8.GetBytes("服务器消息：" + clientData);
                //clientSocket.Send(sendDatas);
                //1:
                //clientSocket.BeginSend(sendDatas, 0, sendDatas.Length, SocketFlags.None, AsyncSend, clientSocket);
                //2:
                NetworkStream networkStream = null;
                try
                {
                    networkStream = new NetworkStream(clientSocket);
                    networkStream.BeginWrite(sendDatas, 0, sendDatas.Length, AsyncNetworkStreamSend, networkStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("异步发送数据：" + ex.ToString());
                }
                clientSocket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, asyncReceiveData);
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
        public class AsyncReceiveData
        {
            public Socket socket;
            public byte[] data;
        }
    }
}