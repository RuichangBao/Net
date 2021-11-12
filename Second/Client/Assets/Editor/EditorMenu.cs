using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EditorMenu
{
    [MenuItem("Assets/创建Request和Response")]
    public static void CreateReqAck()
    {
        Debug.LogError("创建AAA");
        //ProjectWindowUtil.CreateFolder();
        //ProjectWindowUtil.CreateAsset(scri,);
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreateReqAck>(),"111",null,"222");
    }
}