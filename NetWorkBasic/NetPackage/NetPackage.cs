namespace NetPackage
{
    [Serializable]
    public class LoginMsg
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