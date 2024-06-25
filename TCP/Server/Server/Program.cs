using Protocol;
using System.Drawing;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("s:开启服务器");
            Console.WriteLine("c:退出");
            Thread thread = new Thread(NetManager.Instance.Start);
            thread.Start();
            while (true)
            {
                string inPut = Console.ReadLine();
                if (string.Compare(inPut, "clear", true) == 0)
                {
                    Console.Clear();
                }
            }
        }
    }
}