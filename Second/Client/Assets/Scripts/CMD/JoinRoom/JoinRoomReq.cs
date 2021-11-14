using ProtoBuf;


[ProtoContract]
public class JoinRoomReq : Req
{
    [ProtoMember(2)]
    public int roomId;
}