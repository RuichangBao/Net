using Client;
using System.Data;
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
                Console.WriteLine("客户端链接线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
                Socket socket = ar.AsyncState as Socket;
                Socket clientSocket = socket.EndAccept(ar);
                //ConnectSuccesMsg connectSuccesMsg = new ConnectSuccesMsg { info = "链接服务器成功AAAAAAAAA" };
                //SendMessage(clientSocket, connectSuccesMsg);
                //异步接收数据缓存
                NetPackage netPackage = new NetPackage();
                AsyncReceiveData asyncReceiveData = new AsyncReceiveData { socket = clientSocket, netPackage = netPackage };
                clientSocket.BeginReceive(netPackage.headBuffer, 0, NetPackage.headLength, SocketFlags.None, AsyncReceiveHead, asyncReceiveData);
                socket.BeginAccept(ClientConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端建立链接错误：" + ex);
            }
        }
        /// <summary>异步接受客户端消息头</summary>
        private static void AsyncReceiveHead(IAsyncResult ar)
        {
            try
            {
                AsyncReceiveData asyncReceiveData = ar.AsyncState as AsyncReceiveData;
                Socket clientSocket = asyncReceiveData.socket;
                NetPackage netPackage = asyncReceiveData.netPackage;
                int length = clientSocket.EndReceive(ar);//本次接收的字节数
                if (length <= 0)
                {
                    Console.WriteLine("客户端已经下线");
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    return;
                }
                netPackage.headIndex += length;
                if (netPackage.headIndex < NetPackage.headLength)
                {
                    clientSocket.BeginReceive(netPackage.headBuffer, netPackage.headIndex, NetPackage.headLength - netPackage.headIndex, SocketFlags.None, AsyncReceiveHead, asyncReceiveData);
                }
                else
                {
                    netPackage.InitBodyBuff();
                    clientSocket.BeginReceive(netPackage.bodyBuffer, 0, netPackage.bodyLength, SocketFlags.None, AsyncReceiveBody, asyncReceiveData);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端非正常下线：" + ex.ToString());
            }
        }
        private static void AsyncReceiveBody(IAsyncResult ar)
        {
            try
            {
                AsyncReceiveData asyncReceiveData = ar.AsyncState as AsyncReceiveData;
                Socket clientSocket = asyncReceiveData.socket;
                NetPackage netPackage = asyncReceiveData.netPackage;
                int length = clientSocket.EndReceive(ar);//本次接收的字节数
                if (length <= 0)
                {
                    Console.WriteLine("客户端已经下线");
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    return;
                }
                netPackage.bodyIndex += length;
                if (netPackage.bodyIndex < netPackage.bodyLength)
                {
                    clientSocket.BeginReceive(netPackage.bodyBuffer, netPackage.bodyIndex, netPackage.bodyLength - netPackage.bodyIndex, SocketFlags.None, AsyncReceiveBody, asyncReceiveData);
                }
                else
                {
                    object obj = SerializerUtil.DeSerializer(netPackage.bodyBuffer);
                    Console.WriteLine("客户端数据接收完成" + obj.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端非正常下线：" + ex.ToString());
            }
        }
        ///<summary>异步发送数据</summary>
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
                Console.WriteLine("异步发送数据错误：" + ex.ToString());
            }
        }

        private static void SendMessage(Socket socket, NetMsg netMsg)
        {

            byte[] datas = SerializerUtil.Serializer(netMsg);
            int dataLength = datas.Length;
            byte[] sendDatas = new byte[dataLength + 4];
            byte[] byteLength = BitConverter.GetBytes(dataLength);
            byteLength.CopyTo(sendDatas, 0);
            datas.CopyTo(sendDatas, 4);

            NetworkStream networkStream = new NetworkStream(socket);
            networkStream.BeginWrite(sendDatas, 0, sendDatas.Length, AsyncNetworkStreamSend, networkStream);
        }

        public class AsyncReceiveData
        {
            public Socket socket;
            public NetPackage netPackage;
        }
    }
}