using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CBuffer
{
    public int msgType;
    /// <summary>
    /// 数据长度，（序列化对象长度）
    /// </summary>
    public int length;
    public byte[] data;
    public int BytesRead { get; set; }
    private readonly int bufferSize = 65535;
    private byte[] msgTypeBytes;
    private byte[] lengthBytes;
    private int intLength = 0;
    private Queue<IMessage> requestQueue;
    public CBuffer()
    {
        intLength = sizeof(int);
        this.msgType = 0;
        this.length = 0;
        this.data = new byte[bufferSize];
        msgTypeBytes = new byte[intLength];
        lengthBytes = new byte[intLength];
        requestQueue = new Queue<IMessage>();
    }
    public byte[] GetBytes()
    {
        byte[] tempData = new byte[length + intLength * 2];
        Array.Copy(msgTypeBytes, tempData, intLength);
        Array.Copy(lengthBytes, 0, tempData, intLength, intLength);
        Array.Copy(this.data, 0, tempData, intLength * 2, length);

        Debug.LogError("length:" + this.length + "     " + tempData.Length);
        return tempData;
    }
    public void Clear()
    {
        this.msgType = 0;
        this.length = 0;
    }
    public void Update(byte[] data, int startIndex)
    {
        this.BytesRead = data.Length - startIndex;
        Array.Copy(data, startIndex, this.data, 0, BytesRead);
    }
    internal void Update(MsgType msgType, IMessage message)
    {
        this.msgType = (int)msgType;
        this.data = MessageExtensions.ToByteArray(message);
        if (this.data.Length + 2 * intLength > bufferSize)
        {
            Debug.LogError("注意：Response数据过长：" + this.msgType);
        }
        length = this.data.Length;
        msgTypeBytes = BitConverter.GetBytes(this.msgType);
        lengthBytes = BitConverter.GetBytes(length);
    }

    public void AddRequest(IMessage message)
    {
        requestQueue.Enqueue(message);
    }

    public IMessage Dequeue()
    {
        if (requestQueue.Count <= 0)
            return null;
        IMessage message = requestQueue.Dequeue();
        return message;
    }

    /// <summary>
    /// 获取发送长度
    /// </summary>
    /// <returns></returns>
    internal int GetSendLength()
    {
        return this.length + 2 * intLength;
    }
}