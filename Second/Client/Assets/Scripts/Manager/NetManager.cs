using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;

public class NetManager
{
    private UdpClient udpcSend;
    private string ip = "127.0.0.1";
    private int serverPort = 8899;
    private int clientPort = 8898;
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
        Thread thrRecv = new Thread(ReceiveMessage);
        thrRecv.Start();
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="data"></param>
    public void SendMessaage(byte[] data)
    {
        Debug.LogError("向服务器发送消息");
        try
        {
            udpcSend.Send(data, data.Length, ip, serverPort);
        }
        catch(Exception e)
        {
            Debug.LogError("发送消息失败"+e.Message);
        }
    }

    /// <summary>
    /// 接受数据
    /// </summary>
    private void ReceiveMessage(object obj)
    {
        IPEndPoint clienIPEndPoint = new IPEndPoint(IPAddress.Parse(ip), clientPort);
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