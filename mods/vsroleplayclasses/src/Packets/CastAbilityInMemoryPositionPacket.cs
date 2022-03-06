using ProtoBuf;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class CastAbilityInMemoryPositionPacket
    {
        public int Position;
    }
}