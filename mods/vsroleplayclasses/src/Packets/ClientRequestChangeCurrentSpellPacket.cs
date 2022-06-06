
using ProtoBuf;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ClientRequestChangeCurrentSpellPacket
    {
        public int Position;
    }
}