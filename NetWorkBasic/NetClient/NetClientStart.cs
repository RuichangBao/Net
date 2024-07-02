using NetTools;
using Protocol;

namespace NetClient
{
    internal class NetClientStart
    {
        static void Main(string[] args)
        {
            NetToolsSocket<ClientNetSession,ProtocolMsg> netToolsSocket = new NetToolsSocket<ClientNetSession, ProtocolMsg>();
            netToolsSocket.StartAsClient("127.0.0.1", 1994);
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
                    netToolsSocket.CloseClient();
                    return;
                }
                else
                {
                    HelloMsg helloMsg = new HelloMsg { info = userWrite };
                    Console.WriteLine("客户端向服务器发送消息");
                    netToolsSocket.netSession.SendMessage(helloMsg);
                }
            }
        }
    }
}