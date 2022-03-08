using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorSpellTargetable : EntityBehavior
    {
        public EntityBehaviorSpellTargetable(Entity entity) : base(entity)
        {

        }

        public override string PropertyName()
        {
            return "EntityBehaviorSpellTargetable";
        }

        public override void OnEntityLoaded()
        {

        }

        public override void OnEntitySpawn()
        {

        }
    }
}
