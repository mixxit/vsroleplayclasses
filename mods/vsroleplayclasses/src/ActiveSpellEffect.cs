using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    class ActiveSpellEffect
    {
        public long SourceEntityId { get; set; }
        public long AbilityId { get; set; }
        public int Duration { get; set; }
    }
}
