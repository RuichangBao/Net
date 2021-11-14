using ProtoBuf;

[ProtoContract]
public class Req
{
    public Req()
    {

    }
    public Req(MsgType msgType)
    {
        this.msgType = msgType;
    }
    [ProtoMember(1)]
    public MsgType msgType;
    public void Test()
    {
        
    }
}