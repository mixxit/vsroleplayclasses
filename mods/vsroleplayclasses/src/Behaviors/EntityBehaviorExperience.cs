using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorExperience : EntityBehavior
    {
        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorExperience"; }

        public EntityBehaviorExperience(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            base.OnEntityDeath(damageSourceForDeath);

            if (sapi == null)
                return;

            if (damageSourceForDeath.SourceEntity == null)
                return;

            if (!(damageSourceForDeath.SourceEntity is EntityPlayer))
                return;

            if (!(((EntityPlayer)damageSourceForDeath.SourceEntity).Player is IServerPlayer))
                return;

            
            var expType = DamageTypeClass.Convert((ExtendedEnumDamageType)damageSourceForDeath.Type).AdventureClass;
            damageSourceForDeath.SourceEntity.AwardExperience(expType, this.entity.GetExperienceWorth((IServerPlayer)((EntityPlayer)damageSourceForDeath.SourceEntity).Player));
        }
    }
}
