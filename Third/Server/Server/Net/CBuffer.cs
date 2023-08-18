using Google.Protobuf;
using Protocol;
using System;
using System.Diagnostics;

namespace Server
{
    public class CBuffer
    {
        public int msgType;
        /// <summary>
        /// 数据长度，（序列化对象长度）
        /// </summary>
        public int length;
        public byte[] data;
        private readonly int bufferSize = 65535;
        private byte[] msgTypeBytes;
        private byte[] lengthBytes;
        private int intLength = 0;
        public CBuffer()
        {
            intLength = sizeof(int);
            this.msgType = 0;
            this.length = 0;
            this.data = new byte[bufferSize];
            msgTypeBytes = new byte[intLength];
            lengthBytes = new byte[intLength];
        }
        public byte[] GetBytes()
        {
            byte[] tempData = new byte[length + intLength * 2];
            Array.Copy(msgTypeBytes, tempData, intLength);
            Array.Copy(lengthBytes, 0, tempData, intLength, intLength);
            Array.Copy(this.data, 0, tempData, intLength * 2, length);

            Console.WriteLine("length:" + this.length + "     " + tempData.Length);
            return tempData;
        }
        public void Clear()
        {
            this.msgType = 0;
            this.length = 0;
        }

        internal void Update(byte[] data)
        {
            Array.Copy(data, 0, msgTypeBytes, 0, intLength);
            Array.Copy(data, intLength, lengthBytes, 0, intLength);
            msgType = BitConverter.ToInt32(msgTypeBytes, 0);
            length = BitConverter.ToInt32(lengthBytes, 0);
            Array.Copy(data, intLength * 2, this.data, 0, length);
        }
        internal void Update(MsgType msgType, IMessage message)
        {
            this.msgType = (int)msgType;
            this.data = MessageExtensions.ToByteArray(message);
            if (this.data.Length + 2 * intLength > bufferSize)
            {
                Console.WriteLine("注意：Response数据过长：" + this.msgType);
            }
            length = this.data.Length;
            msgTypeBytes = BitConverter.GetBytes(this.msgType);
            lengthBytes = BitConverter.GetBytes(length);
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
}