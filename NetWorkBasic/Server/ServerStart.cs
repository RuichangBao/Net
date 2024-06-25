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
            Console.WriteLine("服务器");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            EndPoint endPoint = new IPEndPoint(iPAddress, 1994);
            socket.Bind(endPoint);
            socket.Listen(100);
            Console.WriteLine("服务器已经启动");

            Socket clientSocket = socket.Accept();
            byte[] bytes = Encoding.UTF8.GetBytes("客户端链接成功");
            clientSocket.Send(bytes);

            //接收数据缓存
            byte[] dataRcv = new byte[1024];
            int lenRcv = clientSocket.Receive(dataRcv);
            string msgRcv = Encoding.UTF8.GetString(dataRcv, 0, lenRcv);
            Console.WriteLine("客户端数据：" + msgRcv);
            clientSocket.Send(Encoding.UTF8.GetBytes("服务器向客户端发送数据"));
            while (true) { }
        }
    }
}