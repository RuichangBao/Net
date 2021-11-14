using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
namespace Server
{
    public class MySerializerUtil
    {
        #region C#自带序列化反序列化
        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }
        public static object BytesToObject(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                IFormatter formatter = new BinaryFormatter();
                object obj = formatter.Deserialize(ms);
                return obj;
            }
        }
        #endregion

        #region protobuf序列化反序列化
        /// <summary>
        /// 序列化protobuf
        /// </summary>
        public static byte[] Serialize(object obj)
        {
            try
            {
                byte[] array;
                if (obj == null)
                {
                    array = new byte[0];
                    return array;
                }

                MemoryStream memoryStream = new MemoryStream();
                Serializer.Serialize(memoryStream, obj);
                array = new byte[memoryStream.Length];
                memoryStream.Position = 0L;
                memoryStream.Read(array, 0, array.Length);
                memoryStream.Dispose();
                return array;
            }
            catch (Exception ex)
            {
                Console.WriteLine("序列化失败：" + ex);
                return new byte[0];
            }
        }
        /// <summary>
        /// 反序列化protobuf
        /// </summary>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                Console.WriteLine("反序列化失败 data为空或者 data长度0");
                return default(T);
            }
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Position = 0L;
                T result = Serializer.Deserialize<T>(memoryStream);
                memoryStream.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("反序列化失败:" + ex);
                return default(T);
            }
        }
        #endregion
    }
}
