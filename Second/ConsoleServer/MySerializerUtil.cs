﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServer
{
    class MySerializerUtil
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
    }
}
