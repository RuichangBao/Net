using System;

namespace ConsoleServer
{
    [Serializable]
    public class TestClass
    {
        public MsgType msgType = MsgType.TEST;
        public int a = 1;
        public int b = 2;
        public int c = 3;
        public string stra = "AAAAAAAA";
        public string strb = "BBBBBBBBB";
        public string strc = "CCCCCCCC";
    }
}