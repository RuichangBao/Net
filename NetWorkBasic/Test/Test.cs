using NetPackage;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Test
{
    internal class Test
    {
        static void Main(string[] args)
        {
            LoginMsg loginMsg = new LoginMsg
            {
                serverId = 10086,
                account = "Test 10086",
                password = "Test password",
            };
            byte[] data;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, loginMsg);
                memoryStream.Seek(0, SeekOrigin.Begin);
                data = memoryStream.ToArray();
            }
            LoginMsg loginMsg1;
            using (var memoryStream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                loginMsg1 = formatter.Deserialize(memoryStream) as LoginMsg;
            }
            Console.WriteLine(loginMsg1.ToString());
            Console.WriteLine("Hello, World!");
        }
    }
}