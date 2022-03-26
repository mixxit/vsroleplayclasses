using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class UpdateMemorisedSpellsPacket
    {
        public ConcurrentDictionary<int, Tuple<int,string>> memorisedAbilities;

        internal static UpdateMemorisedSpellsPacket From(IServerPlayer player)
        {
            var updateMemorisedSpellsPacket = new UpdateMemorisedSpellsPacket()
            {
                memorisedAbilities = new ConcurrentDictionary<int, Tuple<int, string>>()
            };

            for (int i = 1; i <= WorldLimits.MaxMemorisedSlots; i++)
            {
                var ability = player.GetAbilityInMemoryPosition(i);
                if (ability == null)
                {
                    updateMemorisedSpellsPacket.memorisedAbilities[i - 1] = new Tuple<int, string>(-1,null);
                    continue;
                }

                updateMemorisedSpellsPacket.memorisedAbilities[i - 1] = new Tuple<int, string>(ability.GetIcon(), ability.Name);
            }

            return updateMemorisedSpellsPacket;
        }
    }
}
