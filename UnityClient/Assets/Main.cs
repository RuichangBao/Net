using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        Test1Request test1Request = new Test1Request
        {
            Query = "123",
            PageNumber = 1,
            ResultPerPage = 10,
        };
        Debug.LogError(test1Request.ToString());
    }

    void Update()
    {
        
    }
}
public partial class Test1Request
{
   
}