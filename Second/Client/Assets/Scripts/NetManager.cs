using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class NetManager
{
    private static NetManager instance;
    public static NetManager Instance
    {
        get
        {
            if (instance == null)
                instance = new NetManager();
            return instance;
        }
    }


    private NetManager()
    {
        /// <summary>
        /// 用于UDP发送的网络服务类
        /// </summary>

        IPEndPoint localIpep = null;

        //localIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888); // 本机IP和监听端口号
        //UdpClient udpcSend = new UdpClient(localIpep);
        UdpClient udpcSend = new UdpClient();

        try
        {
            string message = "AAAAAA123456789BBBBBBBBB";
            Debug.LogError(message.Length);
            byte[] sendbytes = Encoding.UTF8.GetBytes(message);
            //IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8899); // 发送到的IP地址和端口号
            //udpcSend.Send(sendbytes, sendbytes.Length, "127.0.0.1", 8899);
            udpcSend.Send(sendbytes, sendbytes.Length);
            udpcSend.Close();
        }
        catch { }
        Debug.LogError("发送结束");
    }
}