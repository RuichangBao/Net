using NetTools;

namespace Protocol
{
    [Serializable]
    public class ProtocolMsg : NetMsg
    {
    }
    [Serializable]
    public class HelloMsg : ProtocolMsg
    {
        public string info;
        public override string ToString()
        {
            return $"info:{info}";
        }
    }

    [Serializable]
    public class BroadcastMsg : ProtocolMsg
    {
        public string info;
        public override string ToString()
        {
            return $"info:{info}";
        }
    }
}
