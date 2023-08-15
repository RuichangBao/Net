using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Protocol;
public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError((int)MsgType.MessageVersion);
        Debug.LogError((int)ErrorCode.Unknown);
        TestRequest testRequest = new TestRequest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
