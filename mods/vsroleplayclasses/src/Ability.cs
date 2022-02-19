using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

namespace vsroleplayclasses.src
{
    public class Ability
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CreatorUid { get; set; }
        public List<MagicaPower> WordsOfMagic { get; set; }

        internal void Cast(Entity target)
        {

        }
    }
}
