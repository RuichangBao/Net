using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
public class NetManager
{
    private static NetManager instance;
    public static NetManager Instance
    {
        get
        {
            if (instance == null)
                instance = new NetManager();
            return instance;
        }
    }
    private NetManager()
    {
        Debug.Log("NetNanager");
        UdpClient udpClient = new UdpClient(8818);//端口号
        //udpClient.
  } }