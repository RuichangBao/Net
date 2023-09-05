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
using static Google.Protobuf.Reflection.FieldOptions.Types;

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
        cRequestBuff.Update(msgType, msg);
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
            //读取的字节数
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
        int bytesRead = data.Length;
        byte[] tempData;
        int cacheLength = cResponseBuff.BytesRead;
        if (cacheLength != 0)
        {
            tempData = new byte[bytesRead + cacheLength];
            Array.Copy(cResponseBuff.data, 0, tempData, 0, cacheLength);
            Array.Copy(data, 0, tempData, cacheLength, bytesRead);
        }
        else
        {
            tempData = new byte[bytesRead];
            Array.Copy(data, 0, tempData, 0, bytesRead);
        }

        int intLength = sizeof(int);
        byte[] msgTypeBytes = new byte[intLength];
        byte[] lengthBytes = new byte[intLength];
        int msgType = 0;
        int length = 0;
        int startIndex = 0;
        //剩余数据长度
        int endNum = 0;
        while (true)
        {
            endNum = tempData.Length - startIndex - 2 * intLength;
            //剩余数据不是完整的协议号和长度
            if (endNum < 0)
            {
                cResponseBuff.Update(tempData, startIndex);
                break;
            }
            Array.Copy(tempData, startIndex, msgTypeBytes, 0, intLength);
            Array.Copy(tempData, startIndex + intLength, lengthBytes, 0, intLength);
            msgType = BitConverter.ToInt32(msgTypeBytes, 0);
            length = BitConverter.ToInt32(lengthBytes, 0);
            if (endNum < length)
            {
                cResponseBuff.Update(tempData, startIndex);
                break;
            }
            byte[] requestData = new byte[length];
            Array.Copy(tempData, startIndex + 2 * intLength, requestData, 0, length);
            IMessage message = this.Deserialization(msgType, requestData, length);
            startIndex = startIndex + 2 * intLength + length;
            cResponseBuff.AddRequest(message);
            if (endNum <= 0)
            {
                break;
            }
        }
        while (true)
        {
            IMessage message = cResponseBuff.Dequeue();
            if (message == null)
                break;
            TestResponse testResponse = message as TestResponse;
            if (testResponse != null)
            {
                Debug.LogError(testResponse.Num1);
            }
        }
    }

    private IMessage Deserialization(int msgType, byte[] data, int length)
    {
        MsgType msgType1 = (MsgType)msgType;
        IMessage message = null;
        switch (msgType1)
        {
            case MsgType.Zero:

                break;
            case MsgType.TestRequest:
                message = new TestRequest();
                message.MergeFrom(data, 0, length);
                break;
            case MsgType.TestResponse:
                message = new TestResponse();
                message.MergeFrom(data, 0, length);
                break;
            case MsgType.CreateRoomRequest:
                break;
            case MsgType.CreateRoomResponse:
                break;
            default:
                break;
        }
        return message;
    }
}