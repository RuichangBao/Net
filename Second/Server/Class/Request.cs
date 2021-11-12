using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [ProtoContract]
    class Request
    {
        [ProtoMember(1)]
        public MsgType msgType;
        public Request()
        {

        }
        public Request(MsgType msgType)
        {
            this.msgType = msgType;
        }
    }
}
