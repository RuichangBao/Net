using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class ServerStart
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            CreateBasicServer();
        }
        private static void CreateBasicServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 17666);
            socket.Bind(endPoint);
            socket.Listen(100);
            Console.WriteLine("服务器已经启动");

            Socket skt = socket.Accept();
            byte[] bytes = Encoding.UTF8.GetBytes("客户端链接成功");
            skt.Send(bytes);

            //接收数据缓存
            byte[] dataRcv = new byte[1024];
            int lenRcv = skt.Receive(dataRcv);

            string msgRcv = Encoding.UTF8.GetString(dataRcv, 0, lenRcv);
            Console.WriteLine("客户端数据：" + msgRcv);
            //https://www.qiqiker.com/course/74/task/2444/show
            while (true) { }
            Console.ReadKey();
        }
    }
}