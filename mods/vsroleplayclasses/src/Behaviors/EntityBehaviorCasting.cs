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

        public bool IsWaitingToReleaseCastCast()
        {
            if (entity.WatchedAttributes.GetLong("finishCastingUnixTime") < 1)
                return false;

            if ((entity.WatchedAttributes.GetLong("finishCastingUnixTime") <= DateTimeOffset.Now.ToUnixTimeMilliseconds()))
                return true;

            return false;
        }

        public bool IsUnfinishedCasting()
        {
            if (entity.WatchedAttributes.GetLong("finishCastingUnixTime") <= 0)
                return false;

            if (DateTimeOffset.Now.ToUnixTimeMilliseconds() <= entity.WatchedAttributes.GetLong("finishCastingUnixTime"))
                return true;
            else
                return false;
        }

        public void TryFinishCast(bool forceSelf = false)
        {
            if (!IsWaitingToReleaseCastCast())
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

            var ability = AbilityTools.GetAbility(this.entity.World, abilityId);
            if (ability == null)
                return;

            ability.FinishCast(this.entity, forceSelf);
        }

        internal void StartCasting(long abilityId, long duration, string abilityName)
        {
            this.abilityId = abilityId;
            this.abilityName = abilityName;
            entity.World.PlaySoundAt(new AssetLocation("vsroleplayclasses","sounds/effect/spelcast"), entity, null, false, 14);
            entity.WatchedAttributes.SetLong("startCastingUnixTime", DateTimeOffset.Now.ToUnixTimeMilliseconds());
            entity.WatchedAttributes.SetLong("finishCastingUnixTime", DateTimeOffset.Now.ToUnixTimeMilliseconds() + (duration * 1000));
            entity.WatchedAttributes.SetString("startCastingAbilityName", abilityName);
        }

        internal void ClearCasting()
        {
            entity.WatchedAttributes.SetLong("startCastingUnixTime", 0);
            entity.WatchedAttributes.SetLong("finishCastingUnixTime", 0);
            entity.WatchedAttributes.SetString("startCastingAbilityName", "");
            this.abilityId = 0;
            this.abilityName = "";
        }
    }
}
