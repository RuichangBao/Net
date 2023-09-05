using Google.Protobuf;
using Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class NetManager : Singleton<NetManager>, IInit
    {
        private int serverPort = 2023;
        private CBuffer cRequestBuff;
        private CBuffer cResponseBuff;
        private List<TcpClient> listTcpClient;
        private byte[] data;
        public void Init()
        {
            cRequestBuff = new CBuffer();
            cResponseBuff = new CBuffer();
            listTcpClient = new List<TcpClient>();
            data = new byte[1024];
        }
        public void Start()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, serverPort);
            tcpListener.Start();
            //多线程
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpclient), tcpListener);
        }
        /// <summary>
        /// 客户端链接
        /// </summary>
        private void DoAcceptTcpclient(IAsyncResult state)
        {
            Console.WriteLine("一个客户端链接：");
            /*                   */
            /* 处理多个客户端接入*/
            /*                   */
            TcpListener listener = (TcpListener)state.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(state);
            listTcpClient.Add(client);
            Console.WriteLine("\n收到新客户端:{0}", client.Client.RemoteEndPoint.ToString());
            //开启线程用来持续收来自客户端的数据
            Thread myThread = new Thread(new ParameterizedThreadStart(Instance.PrintReceiveMsg));
            myThread.Start(client);
            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpclient), listener);
        }

        /// <summary>
        /// 解析Request
        /// </summary>
        /// <param name="data"></param>
        private void AnalyzeRequest(TcpClient client,byte[] data, int bytesRead)
        {
            
            byte[] tempData;
            int cacheLength = cRequestBuff.BytesRead;
            if (cacheLength != 0)
            {
                tempData = new byte[bytesRead + cacheLength];
                Array.Copy(cRequestBuff.data, 0, tempData, 0, cacheLength);
                Array.Copy(data, 0, tempData, cacheLength, bytesRead);
            }
            else
            {
                tempData = new byte[bytesRead];
                Array.Copy(data, 0, tempData, 0, bytesRead);
            }

            int intLength = sizeof(int);
            byte[] msgTypeBytes = new byte[intLength];
            byte[] lengthBytes = new byte[intLength];
            int msgType = 0;
            int length = 0;
            int startIndex = 0;
            //剩余数据长度
            int endNum = 0;
            while (true)
            {
                endNum = tempData.Length - startIndex - 2 * intLength;
                //剩余数据不是完整的协议号和长度
                if (endNum < 0)
                {
                    cRequestBuff.Update(tempData, startIndex);
                    break;
                }
                Array.Copy(tempData, startIndex, msgTypeBytes, 0, intLength);
                Array.Copy(tempData, startIndex + intLength, lengthBytes, 0, intLength);
                msgType = BitConverter.ToInt32(msgTypeBytes, 0);
                length = BitConverter.ToInt32(lengthBytes, 0);
                if (endNum < length)
                {
                    cRequestBuff.Update(tempData, startIndex);
                    break;
                }
                byte[] requestData = new byte[length];
                Array.Copy(tempData, startIndex + 2 * intLength, requestData, 0, length);
                IMessage message = this.Deserialization(msgType, requestData, length);
                startIndex = startIndex + 2 * intLength + length;
                cRequestBuff.AddRequest(message);
                if (endNum <= 0)
                {
                    break;
                }
            }
            Console.WriteLine("解析："+ client.Client.RemoteEndPoint.ToString()+"消息！");
        }

        private void SendMessage(TcpClient tcpClient)
        {
            Console.WriteLine("服务器向客户端发送消息");
            TestResponse response = new TestResponse
            {
                Num1 = 1,
                Num2 = 2,
                Str1 = "sdfdsfsd"
            };
            for (int i = 0; i < 100000; i++)
            {
                response.Num1 = i;
                response.Num2 = (long)i;
                cResponseBuff.Update(MsgType.TestResponse, response);
                byte[] data = cResponseBuff.GetBytes();
                int sendLength = cResponseBuff.GetSendLength();
                NetworkStream networkStream = tcpClient.GetStream();
                networkStream.BeginWrite(data, 0, sendLength, HandleDatagramWritten, tcpClient);
            }
        }
        private void HandleDatagramWritten(IAsyncResult ar)
        {
        }

        public void PrintReceiveMsg(object reciveClient)
        {
            /*                   */
            /* 用来打印接收的消息*/
            /*                   */
            TcpClient client = reciveClient as TcpClient;
            if (client == null)
            {
                Console.WriteLine("client error");
                return;
            }
            while (true)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    int num = stream.Read(data, 0, data.Length); //将数据读到result中，并返回字符长度                  
                    if (num != 0)
                    {
                        //todo:这里多线程分包有问题
                        this.AnalyzeRequest(client, data, num);
                    }
                    else
                    {
                        //这里需要注意 当num=0时表明客户端已经断开连接，需要结束循环，不然会死循环一直卡住
                        Console.WriteLine("客户端关闭");
                        listTcpClient.Remove(client);
                        break;
                    }
                }
                catch (Exception e)
                {
                    listTcpClient.Remove(client);
                    Console.WriteLine("error:" + e.ToString());
                    break;
                }

            }

        }

        private IMessage Deserialization(int msgType, byte[] data, int length)
        {
            MsgType msgType1 = (MsgType)msgType;
            IMessage message = null;
            switch (msgType1)
            {
                case MsgType.Zero:

                    break;
                case MsgType.TestRequest:
                    message = new TestRequest();
                    message.MergeFrom(data, 0, length);
                    break;
                case MsgType.TestResponse:
                    break;
                case MsgType.CreateRoomRequest:
                    break;
                case MsgType.CreateRoomResponse:
                    break;
                default:
                    break;
            }
            return message;
        }
    }
}