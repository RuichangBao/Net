using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// 主逻辑
    /// </summary>
    class LoginManager
    {
        public Queue<byte[]> QueueReceive { get; }
        private static LoginManager instance;
        public static LoginManager Instance
        {
            get 
            {
                if (instance == null)
                    instance = new LoginManager();
                return instance;
            }
        }
        private LoginManager()
        {
            QueueReceive = new Queue<byte[]>();
        }

        public void AddReceive(byte[]data)
        {
            QueueReceive.Enqueue(data);
        }
        private void Run()
        {
            while (true)
            {
                while (QueueReceive.Count>0)
                {
                    byte[] data = QueueReceive.Dequeue();
                    RunServerReceive(data);
                }
            }
           
        } 
        private void RunServerReceive(byte[] data)
        {
            Req req = MySerializerUtil.Deserialize<Req>(data);
            RunServerReceive(req.msgType, data);
        }
        private void RunServerReceive(MsgType msgType, byte[] data)
        {
            switch (msgType)
            {
                case MsgType.TEST:
                    break;
                case MsgType.CreateRoom:
                    break;
                case MsgType.JoinRoom:
                    break;
                case MsgType.Chat:
                    break;
                default:
                    break;
            }
        }

    }
}
