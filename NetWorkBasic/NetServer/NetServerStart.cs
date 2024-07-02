using NetTools;
using Protocol;
using System.Net.Sockets;

namespace NetServer
{
    internal class NetServerStart
    {
        static void Main(string[] args)
        {
            NetToolsSocket<ServerNetSession, ProtocolMsg> netToolsSocket = new NetToolsSocket<ServerNetSession, ProtocolMsg>();
            netToolsSocket.StartAsServer("127.0.0.1", 1994);
            while (true)
            {
                string userWrite;
                do
                {
                    userWrite = Console.ReadLine();
                }
                while (userWrite.Length <= 0);
                if (userWrite.Equals("close"))
                {
                    netToolsSocket.CloseServer();
                    return;
                }
                else
                {
                    BroadcastMsg broadcastMsg = new BroadcastMsg { info = "服务器广播数据：" + userWrite };
                    Console.WriteLine("服务器向客户端发送消息");
                    List<ServerNetSession> listNetSessions = netToolsSocket.GetSessionList();
                    byte[] datas = SerializerUtil.Serializer(broadcastMsg);
                    datas = SerializerUtil.PackLenInfo(datas);
                    for (int i = 0, length = listNetSessions.Count; i < length; i++)
                    {
                        listNetSessions[i].SendMessage(datas);
                    }
                }
            }
        }
    }
}