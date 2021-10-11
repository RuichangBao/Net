using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

    public void SendMessaage(byte[]data)
    {
        try
        {
            udpcSend.Send(data, data.Length,ip,port);
        }
        catch { }
    }
}