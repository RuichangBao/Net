using Google.Protobuf;
using Protocol;
using System;
using System.Diagnostics;

namespace Server
{
    public class CRuningBuff
    {
        public int msgType;
        public int length;
        public byte[] data;
        public CRuningBuff(MsgType msgType, IMessage msg)
        {
            this.msgType = (int)msgType;
            this.data = MessageExtensions.ToByteArray(msg);
            this.length = this.data.Length + sizeof(int) + sizeof(int);
        }
        public CRuningBuff(byte[] data)
        {
            int intLength = sizeof(int);
            byte[] msgTypeBytes = new byte[intLength];
            byte[] lengthBytes = new byte[intLength];
            Array.Copy(data, 0, msgTypeBytes, 0, intLength);
            Array.Copy(data, intLength, lengthBytes, 0, intLength);
            this.msgType = BitConverter.ToInt32(msgTypeBytes, 0);
            this.length = BitConverter.ToInt32(lengthBytes, 0);
            this.data = new byte[this.length - 2 * intLength];
            Array.Copy(data, intLength * 2, this.data, 0, this.data.Length);
        }

        public byte[] GetBytes()
        {
            int intLength = sizeof(int);
            byte[] msgTypeBytes = BitConverter.GetBytes(msgType);
            byte[] lengthBytes = BitConverter.GetBytes(length);
            byte[] tempData = new byte[this.data.Length + intLength * 2];
            Array.Copy(msgTypeBytes, tempData, intLength);
            Array.Copy(lengthBytes, 0, tempData, intLength, intLength);
            Array.Copy(this.data, 0, tempData, intLength * 2, this.data.Length);

            Console.WriteLine("length:" + this.length + "     " + tempData.Length);
            return tempData;
        }
    }
}