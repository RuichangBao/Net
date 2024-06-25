using Google.Protobuf;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using Protocol;
using System.Threading;
using UnityEditor.PackageManager;
using System.Text;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class NetManager : Singleton<NetManager>, IInit
{
    private UdpClient udpClient;
    private NetworkStream networkStream;
    private int clientPort = 2022;//客户端端口号
    private int serverPort = 2023;//服务器端口号;
    private string clientIP = "127.0.0.1";
    private string serverIP = "127.0.0.1";
    private CBuffer cRequestBuff;
    private CBuffer cResponseBuff;
    private IPEndPoint serverIPEndPoint;
    public NetManager()
    {
        cRequestBuff = new CBuffer();
        cResponseBuff = new CBuffer();
        serverIPEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

    }
    public void Init()
    {
        IPAddress iPAddress = IPAddress.Parse(clientIP);
        IPEndPoint clientIPEndPoint = new IPEndPoint(iPAddress, clientPort);
        udpClient = new UdpClient(clientIPEndPoint);
        Thread thread = new Thread(Receive);
        thread.Start();
    }

    public bool SendMessage(MsgType msgType, IMessage msg)
    {
        cRequestBuff.Update(msgType, msg);
        byte[] data = cRequestBuff.GetBytes();
        udpClient.Send(data, data.Length, serverIPEndPoint);
        Debug.LogError("向服务器发送消息");
        return true;
    }
    private void Receive()
    {
        while (true)
        {
            byte[] data = udpClient.Receive(ref serverIPEndPoint);
            AnalyzeRequest(data);
            Debug.LogError(serverIPEndPoint);
            Debug.LogError("收到来自客户端的消息");
        }
    }

    private void AnalyzeRequest(byte[] data)
    {
        cResponseBuff.Update(data);
        Debug.LogError("协议：" + cResponseBuff.msgType);
    }
}