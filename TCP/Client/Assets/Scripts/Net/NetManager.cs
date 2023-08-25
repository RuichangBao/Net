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
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private int serverPort = 2023;//服务器端口号;
    private string serverIP = "127.0.0.1";
    private CBuffer cRequestBuff;
    private CBuffer cResponseBuff;
    public NetManager()
    {
        cRequestBuff = new CBuffer();
        cResponseBuff = new CBuffer();
    }
    public void Init()
    {
        tcpClient = new TcpClient();
        IPAddress serverIPAddress = IPAddress.Parse(serverIP);
        try
        {
            Debug.LogError("客户端请求连接到服务器");
            tcpClient.BeginConnect(serverIPAddress, serverPort, HandleTcpServerConnected, tcpClient);
        }
        catch (Exception ex)
        {
            Debug.LogError("连接到服务器失败：" + ex.Message);
        }
    }
    public bool SendMessage(MsgType msgType, IMessage msg)
    {
        cRequestBuff.Update(msgType,msg);
        byte[] data = cRequestBuff.GetBytes();
        int sendLength = cRequestBuff.GetSendLength();
        networkStream.BeginWrite(data, 0, sendLength, HandleDatagramWritten, tcpClient);
        return true;
    }
    private void HandleDatagramWritten(IAsyncResult ar)
    {
    }
    private void HandleTcpServerConnected(IAsyncResult ar)
    {
        Debug.LogError("连接到服务器回调函数");
        try
        {
            networkStream = tcpClient.GetStream();
            tcpClient.EndConnect(ar);
            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            tcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, HandleDatagramReceived, buffer);
        }
        catch (Exception ex)
        {
            Debug.LogError("连接到服务器回调函数:" + ex.Message);
        }
    }

    private void HandleDatagramReceived(IAsyncResult ar)
    {
        Debug.LogError("读取服务器发送的数据？？？？");
        int numberOfReadBytes = 0;
        try
        {
            numberOfReadBytes = networkStream.EndRead(ar);
            if (numberOfReadBytes < 1)
            {
                Debug.LogError("被动断开接收字节为0，视为断开连接");
                return;
            }
            byte[] buffer = (byte[])ar.AsyncState;
            byte[] receivedBytes = new byte[numberOfReadBytes];
            Buffer.BlockCopy(buffer, 0, receivedBytes, 0, numberOfReadBytes);
            this.AnalyzeResponse(receivedBytes);
            networkStream.BeginRead(buffer, 0, buffer.Length, HandleDatagramReceived, buffer);
        }
        catch (Exception ex)
        {
            Debug.LogError("读取服务器发送消息错误:" + ex);
        }
    }
    private void AnalyzeResponse(byte[] data)
    {
        cResponseBuff.Update(data);
        Debug.LogError("协议：" + cResponseBuff.msgType);
    }
}