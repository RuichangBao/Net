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
        public static byte[] Serializer<T>(T netMsg) where T : NetMsg
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
        public static T DeSerializer<T>(byte[] data) where T : NetMsg
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    T netMsg = formatter.Deserialize(memoryStream) as T;
                    return netMsg;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("反序列化失败" + ex.ToString());
                    return default;
                }

            }
        }
    }
}
