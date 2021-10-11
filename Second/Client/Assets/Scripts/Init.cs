using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class Init : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string str = "fdsafasfsdaf";
        byte[] data = Encoding.UTF8.GetBytes(str);
        NetManager.Instance.SendMessaage(data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
