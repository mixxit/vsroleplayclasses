using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src.Packets
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class UpdateActiveEffectsHudPacket
    {
        public string SerializedActiveSpellEffectHudEntryList;

        internal static UpdateActiveEffectsHudPacket From(Vintagestory.API.Common.IWorldAccessor world, ConcurrentDictionary<long, ActiveSpellEffect> activeSpellEffects)
        {
            var spellEffectsConverted = new List<ActiveSpellEffectHudEntry>();
            foreach(var entry in activeSpellEffects)
            {
                if (entry.Value == null)
                    continue;

                var ability = AbilityTools.GetAbility(world, entry.Value.AbilityId);
                if (ability == null)
                    continue;

                spellEffectsConverted.Add(new ActiveSpellEffectHudEntry()
                {
                    AbilityId = entry.Value.AbilityId,
                    Duration = entry.Value.Duration,
                    SourceEntityId = entry.Value.SourceEntityId,
                    Icon = ability.GetIcon(),
                    Name = ability.Name
                });
            }

            return new UpdateActiveEffectsHudPacket()
            {
                SerializedActiveSpellEffectHudEntryList = JsonConvert.SerializeObject(spellEffectsConverted)
            };
        }
    }
}
