using System.Net;
using System.Net.Sockets;

namespace NetTools
{
    public class NetToolsSocket<T, K> where T : NetSession<K>, new() where K : NetMsg
    {
        private Socket socket;
        public T netSession;
        public int backlog = 100;
        private List<T> listSessions;
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
                netSession = new T();
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
            listSessions = new List<T>();
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
                T netSession = new T();
                netSession.StartRecData(clientSocket, RemoverNetSession);
                listSessions.Add(netSession);
                socket.BeginAccept(ClientConnectCB, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("客户端建立链接错误：" + ex);
            }
        }



        #endregion
        private void RemoverNetSession(NetSession<K> session)
        {
            if (listSessions == null)
                return;
            if (listSessions.Contains(session))
                listSessions.Remove(session as T);
        }

        public List<T> GetSessionList()
        {
            return listSessions;
        }

        public void CloseClient()
        {
            if (netSession != null)
                netSession.CloseSession();
        }
        public void CloseServer()
        {
            for (int i = 0, length = listSessions.Count; i < length; i++)
            {
                listSessions[i].CloseSession();
            }
            listSessions = null;
            if (socket != null)
                socket.Close();
        }
    }
}
