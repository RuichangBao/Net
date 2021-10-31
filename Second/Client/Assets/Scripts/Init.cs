using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Init : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //string str = "fdsafasfsdaf";
        //byte[] data = Encoding.UTF8.GetBytes(str);
        //NetManager.Instance.SendMessage(data);
        TestClass testClass = new TestClass();
        byte[] data = MySerializerUtil.ObjectToBytes(testClass);
        Debug.LogError(data.Length);
        NetManager.Instance.SendMessage(data);
        object obj = MySerializerUtil.BytesToObject(data);
        if(obj!=null)
        {
            TestClass testClass1 = obj as TestClass;
            if(testClass1!=null)
            {
                Debug.LogError(testClass1.msgType);
                Debug.LogError(testClass1.strc);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}
