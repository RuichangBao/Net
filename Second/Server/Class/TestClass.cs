using ProtoBuf;
using System;

namespace Server
{
    [ProtoContract]
    class TestClass1: Request
    {
        [ProtoMember(2)]
        public int a = 100;
        [ProtoMember(3)]
        public int b = 200;
        [ProtoMember(4)]
        public int c = 300;
        [ProtoMember(5)]
        public string stra = "AAAAAAAA";
        [ProtoMember(6)]
        public string strb = "BBBBBBBBB";
        [ProtoMember(7)]
        public string strc = "CCCCCCCC";

        public override string ToString()
        {
            return "a:" + a + "     b:" + b + "     " + base.ToString();
        }
    }
}