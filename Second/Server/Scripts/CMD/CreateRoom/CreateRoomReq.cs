using ProtoBuf;


[ProtoContract]
public class CreateRoomReq : Req
{
    public CreateRoomReq()
    {
    }
    public CreateRoomReq(MsgType msgType)
    {
        this.msgType = msgType;
    }
}