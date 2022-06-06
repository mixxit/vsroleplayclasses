using ProtoBuf;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ChangeAbilityInMemoryPositionPacket
    {
        public int Position;
    }
}