using System.Runtime.Serialization.Formatters.Binary;

namespace NetTools
{
    public class SerializerUtil
    {
        public static byte[] PackLenInfo(byte[] datas)
        {
            int dataLength = datas.Length;
            byte[] resultDatas = new byte[dataLength + 4];
            byte[] byteLength = BitConverter.GetBytes(dataLength);
            byteLength.CopyTo(resultDatas, 0);
            datas.CopyTo(resultDatas, 4);
            return resultDatas;
        }
        public static byte[] Serializer(NetMsg netMsg)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, netMsg);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return memoryStream.ToArray();
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("序列化错误：" + ex.ToString());
                    return null;
                }

            }
        }
        public static NetMsg DeSerializer(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    NetMsg netMsg = formatter.Deserialize(memoryStream) as NetMsg;
                    return netMsg;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("反序列化失败" + ex.ToString());
                    return null;
                }

            }
        }
    }
}
