using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Items;

namespace vsroleplayclasses.src.Models
{
    public class MemorisedAbilityHudEntry
    {
        public long Id { get; set; }
        public int Icon { get; set; }
        public string Name { get; set; }

        internal static MemorisedAbilityHudEntry From(IServerPlayer player, int slot)
        {
            var ability = player.GetAbilityInMemoryPosition(slot);

            if (ability == null)
                return new MemorisedAbilityHudEntry() { Id = -1, Icon = -1, Name = null };

            return new MemorisedAbilityHudEntry() { Id = ability.Id, Icon = ability.GetIcon(), Name = ability.Name };
        }
    }
}
