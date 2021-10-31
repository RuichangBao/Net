using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServer
{
    class Test
    {
        public Test()
        {
            TestClass testClass = new TestClass();
            byte[] data = MySerializerUtil.ObjectToBytes(testClass);
            Console.WriteLine(data.Length);
           
            object obj = MySerializerUtil.BytesToObject(data);
            if (obj != null)
            {
                TestClass testClass1 = obj as TestClass;
                if (testClass1 != null)
                {
                    Console.WriteLine(testClass1.msgType);
                    Console.WriteLine(testClass1.strc);
                }
            }
        }
    }
}
