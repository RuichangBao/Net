using System.Net;
using System.Net.Sockets;

namespace NetTools
{
    public class NetToolsSocket
    {
        private Socket socket;
        public NetSession netSession;
        public int backlog = 100;
        private List<NetSession> listSessions;
        public NetToolsSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        #region 客户端
        public void StartAsClient(string ip, int port)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                socket.BeginConnect(endPoint, ServerConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("建立链接失败：" + ex.ToString());
            }
        }
        ///<summary>链接到服务器</summary>
        private void ServerConnectCB(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);
                Console.WriteLine("连接服务器线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
                netSession = new NetSession();
                netSession.StartRecData(socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 服务器
        public void StartAsServer(string ip, int port)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            listSessions = new List<NetSession>();
            try
            {
                socket.Bind(endPoint);
                socket.Listen(backlog);
                socket.BeginAccept(ClientConnectCB, socket);
                Console.WriteLine("服务器启动成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        ///<summary>客户端链接</summary>
        private void ClientConnectCB(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("客户端链接线程id:" + Thread.CurrentThread.ManagedThreadId.ToString());
                Socket clientSocket = socket.EndAccept(ar);
                NetSession netSession = new NetSession();
                netSession.StartRecData(clientSocket);
                listSessions.Add(netSession);
                socket.BeginAccept(ClientConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端建立链接错误：" + ex);
            }
        }
        #endregion

        public List<NetSession> GetSessionList()
        {
            return listSessions;
        }

        public void Close()
        {
            if (socket == null)
                return;
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
