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
    public void SendMessage(Req req)
    {
        byte[] data = MySerializerUtil.Serialize(req);
        SendMessage(data);
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="data"></param>
    public void SendMessage(byte[] data)
    {
        try
        {
            udpcSend.Send(data, data.Length, ip, serverPort);
        }
        catch (Exception e)
        {
            Debug.LogError("发送消息失败" + e.Message);
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
                byte[] data = udpcRecv.Receive(ref clienIPEndPoint);
                Ack ack = MySerializerUtil.Deserialize<Ack>(data);
                if (ack != null)
                {
                    ParsingData(ack.msgType, data);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                break;
            }
        }
    }
    private byte[] ParsingData(MsgType msgType, byte[] data)
    {
        switch (msgType)
        {
            case MsgType.TEST:
                TestClass1 testClass1 = MySerializerUtil.Deserialize<TestClass1>(data);
                if (testClass1 != null)
                {
                    Console.WriteLine("AAA" + testClass1.ToString());
                }
                break;
            case MsgType.CreateRoom:
                CreateRoom(data);
                break;
            case MsgType.JoinRoom:
                JoinRoom(data);
                break;
            default:
                return data;
        }
        return null;
    }
    private void CreateRoom(byte[] data)
    {
        CreateRoomAck ack = MySerializerUtil.Deserialize<CreateRoomAck>(data);
    }
    private void JoinRoom(byte[] data)
    {
        JoinRoomAck ack = MySerializerUtil.Deserialize<JoinRoomAck>(data);
    }
}