using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EditorMenu
{
    [MenuItem("Assets/创建Request和Response")]
    public static void CreateReqAck()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreateReqAck>(),"输入创建的协议",null,"222");
    }
}