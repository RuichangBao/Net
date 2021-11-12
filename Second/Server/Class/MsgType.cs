using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public enum MsgType
{
    TEST = -100,
    CreateRoom = 100,   //创建房间
    JoinRoom = 101,     //加入房间
    Chat = 102,         //聊天
}