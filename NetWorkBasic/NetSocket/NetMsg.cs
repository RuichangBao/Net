namespace NetTools
{
    [Serializable]
    public abstract class NetMsg
    {
    }

    [Serializable]
    public class HelloMsg : NetMsg
    {
        public string info;
        public override string ToString()
        {
            return $"info:{info}";
        }
    }

    [Serializable]
    public class BroadcastMsg : NetMsg
    {
        public string info;
        public override string ToString()
        {
            return $"info:{info}";
        }
    }
}
