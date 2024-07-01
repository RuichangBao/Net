using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class NetPackage
    {
        public const int headLength = 4;
        public byte[] headBuffer = null;
        public int headIndex;

        public int bodyLength = 0;
        public byte[] bodyBuffer = null;
        public int bodyIndex;
        public NetPackage()
        {
            headBuffer = new byte[headLength];
        }
        public void InitBodyBuff()
        {
            Console.WriteLine();
            bodyLength = BitConverter.ToInt32(headBuffer, 0);
            bodyBuffer = new byte[bodyLength];
        }
    }
}
