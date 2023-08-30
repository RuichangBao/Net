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
            while (true) { }
        }
    }
}