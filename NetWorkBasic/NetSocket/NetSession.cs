using System.Net.Sockets;

namespace NetTools
{
    public class NetSession
    {
        private Socket socket;

        public void StartRecData(Socket socket)
        {
            this.socket = socket;
            NetPackage netPackage = new NetPackage();
            try
            {
                socket.BeginReceive(netPackage.headBuffer, 0, NetPackage.headLength, SocketFlags.None, AsyncReceiveHead, netPackage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("建立链接错误，" + ex.ToString());
            }
        }

        /// <summary>异步接收客户端消息头</summary>
        private void AsyncReceiveHead(IAsyncResult ar)
        {
            try
            {
                NetPackage netPackage = ar.AsyncState as NetPackage;
                int length = socket.EndReceive(ar);//本次接收的字节数
                if (length <= 0)
                {
                    Console.WriteLine("客户端已经下线");
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    return;
                }
                netPackage.headIndex += length;
                if (netPackage.headIndex < NetPackage.headLength)
                {
                    socket.BeginReceive(netPackage.headBuffer, netPackage.headIndex, NetPackage.headLength - netPackage.headIndex, SocketFlags.None, AsyncReceiveHead, netPackage);
                }
                else
                {
                    netPackage.InitBodyBuff();
                    socket.BeginReceive(netPackage.bodyBuffer, 0, netPackage.bodyLength, SocketFlags.None, AsyncReceiveBody, netPackage);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端非正常下线：" + ex.ToString());
            }
        }
        private void AsyncReceiveBody(IAsyncResult ar)
        {
            try
            {
                NetPackage netPackage = ar.AsyncState as NetPackage;
                int length = socket.EndReceive(ar);//本次接收的字节数
                if (length <= 0)
                {
                    Console.WriteLine("客户端已经下线");
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    return;
                }
                netPackage.bodyIndex += length;
                if (netPackage.bodyIndex < netPackage.bodyLength)
                {
                    socket.BeginReceive(netPackage.bodyBuffer, netPackage.bodyIndex, netPackage.bodyLength - netPackage.bodyIndex, SocketFlags.None, AsyncReceiveBody, netPackage);
                }
                else
                {
                    NetMsg netMsg = SerializerUtil.DeSerializer(netPackage.bodyBuffer);
                    Console.WriteLine("客户端数据接收完成" + netMsg.ToString());
                    HanldNetMsg(netMsg);
                    netPackage.Reset();
                    socket.BeginReceive(netPackage.headBuffer, 0, NetPackage.headLength, SocketFlags.None, AsyncReceiveHead, netPackage);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端非正常下线：" + ex.ToString());
            }
        }
        private void HanldNetMsg(NetMsg netMsg)
        {
            Console.WriteLine(netMsg);
        }
        ///<summary>发送数据</summary>
        public void SendMessage(NetMsg netMsg)
        {
            byte[] datas = SerializerUtil.Serializer(netMsg);
            datas = SerializerUtil.PackLenInfo(datas);
            SendMessage(datas);
        }
        public void SendMessage(byte[] datas)
        {
            try
            {
                NetworkStream networkStream = new NetworkStream(socket);
                networkStream.BeginWrite(datas, 0, datas.Length, AsyncNetworkStreamSend, networkStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("异步发送数据错误：" + ex.ToString());
            }
        }
        private void AsyncNetworkStreamSend(IAsyncResult ar)
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
    }
}