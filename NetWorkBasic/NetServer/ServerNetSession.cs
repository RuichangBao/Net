using NetTools;
using Protocol;

namespace NetServer
{
    public class ServerNetSession : NetSession<ProtocolMsg>
    {
        protected override void OnConnected()
        {
            Console.WriteLine("客户端建立链接");
        }
        protected override void OnReciveMsg(NetMsg netMsg)
        {
            Console.WriteLine("收到客户端消息:" + netMsg.ToString());
        }
        protected override void OnDisConnected()
        {
            Console.WriteLine("客户端断开链接");
        }
    }

}
