using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorInteruptable : EntityBehavior
    {
        protected long abilityId;

        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorInteruptable"; }

        public EntityBehaviorInteruptable(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {
            base.OnEntityReceiveDamage(damageSource, ref damage);
            this.entity.Interupt();
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }
    }
}
