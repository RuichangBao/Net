using System.Net.Sockets;

namespace NetTools
{
    public abstract class NetSession<T> where T : NetMsg
    {
        private Socket socket;
        private Action<NetSession<T>> actionClose;
        public void StartRecData(Socket socket, Action<NetSession<T>> actionClose = null)
        {
            this.socket = socket;
            this.actionClose = actionClose;
            OnConnected();
            NetPackage netPackage = new NetPackage();
            try
            {
                socket.BeginReceive(netPackage.headBuffer, 0, NetPackage.headLength, SocketFlags.None, AsyncReceiveHead, netPackage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("建立链接错误，" + ex.ToString());
                CloseSession();
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
                    Console.WriteLine("已经下线");
                    CloseSession();
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
                CloseSession();
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
                    Console.WriteLine("已经下线");
                    CloseSession();
                    return;
                }
                netPackage.bodyIndex += length;
                if (netPackage.bodyIndex < netPackage.bodyLength)
                {
                    socket.BeginReceive(netPackage.bodyBuffer, netPackage.bodyIndex, netPackage.bodyLength - netPackage.bodyIndex, SocketFlags.None, AsyncReceiveBody, netPackage);
                }
                else
                {
                    NetMsg netMsg = SerializerUtil.DeSerializer<NetMsg>(netPackage.bodyBuffer);
                    OnReciveMsg(netMsg);
                    netPackage.Reset();
                    socket.BeginReceive(netPackage.headBuffer, 0, NetPackage.headLength, SocketFlags.None, AsyncReceiveHead, netPackage);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端非正常下线：" + ex.ToString());
                CloseSession();
            }
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
                CloseSession();
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
                CloseSession();
            }
        }

        public void CloseSession()
        {
            OnDisConnected();
            actionClose?.Invoke(this);
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }
        protected virtual void OnConnected()
        {
            Console.WriteLine("建立链接");
        }
        protected virtual void OnReciveMsg(NetMsg netMsg)
        {
            Console.WriteLine(netMsg);
        }
        protected virtual void OnDisConnected()
        {
            Console.WriteLine("断开链接");
        }
    }
}