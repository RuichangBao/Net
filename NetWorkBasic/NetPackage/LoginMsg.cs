namespace NetUtilPackage
{
    [Serializable]
    public class LoginMsg: NetMsg
    {
        public int serverId;
        public string account;
        public string password;

        public override string ToString()
        {
            return $"serverId:{serverId} account:{account} password:{password}";
        }
    }
}