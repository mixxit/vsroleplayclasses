using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SpendPendingExperiencePacket
    {
        public AdventureClass adventureClass;
    }
}
