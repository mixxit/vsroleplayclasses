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
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorCasting : EntityBehavior
    {
        protected long abilityId;
        protected string abilityName;

        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorCasting"; }

        public EntityBehaviorCasting(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public void TryFinishCast(bool forceSelf = false)
        {
            OnFinishCasting(this.abilityId, forceSelf);
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }

        private void OnFinishCasting(long abilityId, bool forceSelf = false)
        {
            if (abilityId == 0)
                return;

            var ability = AbilityTools.GetAbility(this.entity.World, abilityId);
            if (ability == null)
                return;

            ability.FinishCast(this.entity, forceSelf);
        }

        internal void ChangeSpell(long abilityId, long duration, string abilityName)
        {
            this.abilityId = abilityId;
            this.abilityName = abilityName;
            entity.WatchedAttributes.SetString("currentAbilityName", abilityName);
            entity.WatchedAttributes.SetLong("currentAbilityId", abilityId);
        }

        public bool HasCastingSpellReady()
        {
            return (int)entity.WatchedAttributes.GetLong("currentAbilityId") > 0;
        }

        internal void ClearCasting()
        {
            entity.WatchedAttributes.SetString("currentAbilityName", "");
            entity.WatchedAttributes.SetLong("currentAbilityId", 0);
            this.abilityId = 0;
            this.abilityName = "";
        }
    }
}
