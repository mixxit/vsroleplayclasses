using ProtoBuf;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ClientRequestFinishCastingPacket
    {
        public string playerUid;
    }
}