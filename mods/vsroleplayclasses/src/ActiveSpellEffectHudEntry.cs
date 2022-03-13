using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public class ActiveSpellEffectHudEntry
    {
        public long SourceEntityId { get; set; }
        public long AbilityId { get; set; }
        public long Duration { get; set; }
        public int Icon { get; set; }
        public string Name { get; set; }
    }
}
