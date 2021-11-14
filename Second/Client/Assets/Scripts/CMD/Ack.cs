using ProtoBuf;

[ProtoContract]
public class Ack
{
    [ProtoMember(1)]
    public MsgType msgType;
}