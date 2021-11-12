
using UnityEngine;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System.Text;

public class CreateReqAck : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        string[] filePaths = pathName.Split('/');
        string name = filePaths[filePaths.Length - 1];
        string filePath = "Assets/Scripts/Message/" + name;
        if (Directory.Exists(filePath))
        {
            Debug.LogError("该文件夹已经存在，请删除后重试");
            return;
        }
        Directory.CreateDirectory(filePath);
        string reqName = filePath + "/" + name + "Req.cs";
        string ackName = filePath + "/" + name + "Ack.cs";

        string req = string.Format("using ProtoBuf;\n\n\n[ProtoContract]\npublic class {0}{1} : Req\n{{\n}}", name, "Req");
        string ack = string.Format("using ProtoBuf;\n\n\n[ProtoContract]\npublic class {0}{1} : Ack\n{{\n}}", name, "Ack"); ;
        File.WriteAllText(reqName, req, Encoding.UTF8);
        File.WriteAllText(ackName, ack, Encoding.UTF8);
        //AssetDatabase.Refresh();
    }
}
