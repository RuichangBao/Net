using Google.Protobuf;
using Protocol;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using static Google.Protobuf.Reflection.FieldOptions.Types;

namespace Server
{
    public class NetManager : Singleton<NetManager>, IInit
    {
        private int serverPort = 2023;
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private CBuffer cRequestBuff;
        private CBuffer cResponseBuff;
        public void Init()
        {
            cRequestBuff = new CBuffer();
            cResponseBuff = new CBuffer();
        }
        public void Start()
        {
            tcpListener = new TcpListener(IPAddress.Any, serverPort);
            tcpListener.Start();
            tcpClient = tcpListener.AcceptTcpClient();
            networkStream = tcpClient.GetStream();
            //byte[] buffer = new byte[10240000];
            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    Console.WriteLine("接收到的包长度:" + bytesRead);
                    this.AnalyzeRequest(buffer, bytesRead);
                }
            }
        }
       
        /// <summary>
        /// 解析Request
        /// </summary>
        /// <param name="data"></param>
        private void AnalyzeRequest(byte[] data,int bytesRead)
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
            int length=0;
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
                if (endNum< length)
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
            this.SendMessage();
        }

        private void SendMessage()
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
                networkStream.BeginWrite(data, 0, sendLength, HandleDatagramWritten, tcpClient);
            }
        }
        private void HandleDatagramWritten(IAsyncResult ar)
        {
        }

        private IMessage Deserialization(int msgType,byte[]data,int length)
        {
            MsgType msgType1 = (MsgType)msgType;
            IMessage message=null;
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