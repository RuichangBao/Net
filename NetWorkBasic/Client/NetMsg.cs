using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [Serializable]
    public abstract class NetMsg
    { }
    [Serializable]
    public class LoginMsg : NetMsg
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
