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

public class NetManager : Singleton<NetManager>, Init
{
    private TcpClient tcpClient;
    NetworkStream networkStream;
    private int serverPort = 2023;//服务器端口号
    private int clientPort = 2024;//客户端端口号
    private string clientIP = "127.0.0.1";
    private string serverIP = "127.0.0.1";

    //private Socket clientSocket;
    public void Init()
    {
        IPAddress clientIPAddress = IPAddress.Parse(clientIP);
        IPEndPoint clientIPEndPoint = new IPEndPoint(clientIPAddress, clientPort);
        tcpClient = new TcpClient(clientIPEndPoint);

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
        CRuningBuff cRuningBuff = new CRuningBuff(msgType, msg);
        byte[] data = cRuningBuff.GetBytes();
        networkStream.BeginWrite(data, 0, data.Length, HandleDatagramWritten, tcpClient);
        return true;
    }

    public void SendMessage(byte[] data)
    {
        //networkStream.Write(data, 0, data.Length);
        networkStream.BeginWrite(data, 0, data.Length, HandleDatagramWritten, tcpClient);
        //clientSocket.Send(data);
    }

    public bool SendMessage(uint messageId, IMessage msg)
    {
        //tcpClient.se
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

            CRuningBuff cRuningBuff = new CRuningBuff(receivedBytes);
            Debug.LogError("协议：" + cRuningBuff.msgType);
            networkStream.BeginRead(buffer, 0, buffer.Length, HandleDatagramReceived, buffer);
        }
        catch (Exception ex)
        {
            Debug.LogError("读取服务器发送消息错误:" + ex);
        }
    }
}