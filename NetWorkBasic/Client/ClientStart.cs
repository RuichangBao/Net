using NetUtilPackage;
using System.Net;
using System.Net.Sockets;
//https://www.qiqiker.com/course/74/task/2469/show

namespace Client
{
    internal class ClientStart
    {
        static void Main(string[] args)
        {
            Console.WriteLine("客户端主线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
            CreateAsyncClient();
            while (true) { }
        }
        ///<summary>创建客户端链接</summary>
        private static void CreateAsyncClient()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 192, 168, 3, 62 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 8080);
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
                SendMessage(socket);
                //接受数据缓存
                byte[] dataRcv = new byte[1024];
                AsyncReceiveData asyncreceivedata = new AsyncReceiveData { socket = socket, data = dataRcv };
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, asyncreceivedata);
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

                object obj = SerializerUtil.DeSerializer(resultData);
                Console.WriteLine("线程id:" + Thread.CurrentThread.ManagedThreadId.ToString() + "服务器数据:" + obj.ToString());
                Array.Clear(dataRcv, 0, dataRcv.Length);
                socket.BeginReceive(dataRcv, 0, 1024, SocketFlags.None, AsyncReceive, asyncReceiveData);
                SendMessage(socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void SendMessage(Socket socket)
        {
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
            LoginMsg loginMsg = new LoginMsg { serverId = 123, account = "456", password = "789" };

            byte[] datas = SerializerUtil.Serializer(loginMsg);
            int dataLength = datas.Length;
            byte[] sendDatas = new byte[dataLength + 4];
            byte[] byteLength = BitConverter.GetBytes(dataLength);
            byteLength.CopyTo(sendDatas, 0);
            datas.CopyTo(sendDatas, 4);
            Console.WriteLine("数据长度：" + dataLength);
            //---------------分段发送 测试网络分包处理 Start-------------------------------
            //已经发送的长度
            int sendLength = 0;
            int[] sendCouts = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 410, 4444, 88888 };
            try
            {
                for (int i = 0; i < sendCouts.Length; i++)
                {
                    do
                    {
                        consoleRead = Console.ReadLine();
                    } while (!consoleRead.Equals("send"));
                    NetworkStream networkStream = new NetworkStream(socket);
                    int needSendNum = sendCouts[i];
                    if (needSendNum > sendDatas.Length - sendLength)
                        needSendNum = sendDatas.Length - sendLength;
                    networkStream.BeginWrite(sendDatas, sendLength, needSendNum, AsyncNetworkStreamSend, networkStream);
                    sendLength += needSendNum;
                    if (sendLength >= sendDatas.Length)
                    {
                        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAA      客户端数据已经发送完毕：");
                        Console.WriteLine(sendLength);
                        Console.WriteLine(sendDatas.Length);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("异步发送数据错误：" + ex.ToString());
            }//---------------分段发送 测试网络分包处理 End-------------------------------
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
    }
}