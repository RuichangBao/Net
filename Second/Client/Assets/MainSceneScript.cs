using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneScript : MonoBehaviour
{
    public Button btnCreateRoom;
    public Button btnJoinRoom;

    void Start()
    {
        btnCreateRoom.onClick.AddListener(OnClickCreateRoom);
        btnJoinRoom.onClick.AddListener(OnClickJoinRoom);
    }

    private void OnClickCreateRoom()
    {
        CreateRoomReq req = new CreateRoomReq(MsgType.CreateRoom);
        NetManager.Instance.SendMessage(req);
    }
    private void OnClickJoinRoom()
    {
        JoinRoomReq req = new JoinRoomReq 
        {
            msgType = MsgType.JoinRoom,
            roomId = 1000,
        };
        NetManager.Instance.SendMessage(req);
    }
}
   