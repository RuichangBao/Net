using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Init : MonoBehaviour
{
    void Start()
    {
        _ = NetManager.Instance;
        //TestClass1 testClass = new TestClass1();
        //byte[] data = MySerializerUtil.Serialize(testClass);
        //Debug.LogError(data.Length);
        //NetManager.Instance.SendMessage(data);
        //TestClass1 TestClass2 = MySerializerUtil.Deserialize<TestClass1>(data);
        //Debug.LogError(TestClass2.ToString());
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}
