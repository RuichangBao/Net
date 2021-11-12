using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class NetManager2
    {
        private const string ip = "127.0.0.1";
        private int clientPort = 8898;
        private int serverPort = 8899;


        private UdpClient udpcRecv = null;
        private IPEndPoint localIpep = null;

        public NetManager2()
        {
            localIpep = new IPEndPoint(IPAddress.Parse(ip), serverPort); // 本机IP和监听端口号
            udpcRecv = new UdpClient(localIpep);
            Thread thrRecv = new Thread(ReceiveMessage);
            thrRecv.Start();

            Console.WriteLine("NetManager2 监听器已成功启动");
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    IPEndPoint clienIPEndPoint = null;
                    byte[] data = udpcRecv.Receive(ref clienIPEndPoint);

                    Req req = MySerializerUtil.Deserialize<Req>(data);
                    if (req != null)
                    {
                        Parsing(req.msgType, data);
                    }
                    IPEndPoint serverIPEndPoint = new IPEndPoint(clienIPEndPoint.Address, clientPort);
                    SendMessage(data, serverIPEndPoint);
                    //Console.WriteLine(string.Format("{0}[{1}]", serverIPEndPoint, message));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }

        private byte[] Parsing(MsgType msgType, byte[] data)
        {
            switch (msgType)
            {
                case MsgType.TEST:
                    break;
                case MsgType.CreateRoom:
                    CreateRoomReq req = MySerializerUtil.Deserialize<CreateRoomReq>(data);
                    break;
                default:
                    return data;
            }
            return null;
        }

        public void SendMessage(byte[] data, IPEndPoint iPEndPoint)
        {
            Console.WriteLine("向客户端发送消息：" + iPEndPoint.Port + "    " + iPEndPoint.Address);
            udpcRecv.Send(data, data.Length, iPEndPoint);
        }
    }
}