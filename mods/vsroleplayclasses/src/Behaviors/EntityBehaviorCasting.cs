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

        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorCasting"; }

        public EntityBehaviorCasting(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public bool IsWaitingToCast()
        {
            if (entity.WatchedAttributes.GetLong("finishCastingUnixTime") < 1)
                return false;

            if ((entity.WatchedAttributes.GetLong("finishCastingUnixTime") <= DateTimeOffset.Now.ToUnixTimeMilliseconds()))
                return true;

            return false;
        }

        public void TryFinishCast(bool forceSelf = false)
        {
            if (!IsWaitingToCast())
                return;

            var tempAbilityId = this.abilityId;
            ClearCasting();
            OnFinishCasting(tempAbilityId, forceSelf);
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }

        private void OnFinishCasting(long abilityId, bool forceSelf = false)
        {
            if (abilityId == 0)
                return;

            var mod = this.entity.World.Api.ModLoader.GetModSystem<SystemAbilities>();
            if (mod == null)
                return;

            var ability = mod.GetAbilityById(abilityId);
            if (ability == null)
                return;

            ability.FinishCast(this.entity, forceSelf);
        }

        internal void StartCasting(long abilityId, int duration)
        {
            this.abilityId = abilityId;
            entity.World.PlaySoundAt(new AssetLocation("vsroleplayclasses","sounds/effect/spelcast"), entity, null, false, 14);
            entity.WatchedAttributes.SetLong("startCastingUnixTime", DateTimeOffset.Now.ToUnixTimeMilliseconds());
            entity.WatchedAttributes.SetLong("finishCastingUnixTime", DateTimeOffset.Now.ToUnixTimeMilliseconds() + (duration * 1000));
        }

        internal void ClearCasting()
        {
            entity.WatchedAttributes.SetLong("startCastingUnixTime", 0);
            entity.WatchedAttributes.SetLong("finishCastingUnixTime", 0);
            this.abilityId = 0;
        }
    }
}
