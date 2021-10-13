using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class NetManager
{
    private UdpClient udpcSend;
    private string ip = "127.0.0.1";
    private int port = 8899;
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
        udpcSend = new UdpClient();
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="data"></param>
    public void SendMessaage(byte[]data)
    {
        try
        {
            udpcSend.Send(data, data.Length,ip,port);
        }
        catch { }
    }

    /// <summary>
    /// 接受数据
    /// </summary>
    private void ReceiveMessage(object obj)
    {
        IPEndPoint clienIPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        UdpClient udpcRecv = new UdpClient(clienIPEndPoint);
        while (true)
        {
            try
            {
                byte[] bytRecv = udpcRecv.Receive(ref clienIPEndPoint);
                string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);

                Debug.LogError(string.Format("收到服务器回应 {0}[{1}]", clienIPEndPoint, message));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                break;
            }
        }
    }
}