using UnityEngine;
using Protocol;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class Test : MonoBehaviour
{
    public Button btn;
    public InputField inputField;
    void Start()
    {
        _ = NetManager.Instance;
        btn.onClick.AddListener(BtnOnClick);
    }

    private void BtnOnClick()
    {
        TestRequest request = new TestRequest
        {
            Num1 = 123,
            Num2 = 456,
            Str1 = "1466"
        };
        NetManager.Instance.SendMessage(MsgType.TestRequest, request);
        //for (int i = 0; i < 100000; i++)
        //{
        //    request.Num1 = i;
        //    NetManager.Instance.SendMessage(MsgType.TestRequest, request);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}