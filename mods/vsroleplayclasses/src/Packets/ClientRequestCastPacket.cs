using ProtoBuf;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ClientRequestCastPacket
    {
        public string playerUid;
        public bool isForceSelf;
    }
}