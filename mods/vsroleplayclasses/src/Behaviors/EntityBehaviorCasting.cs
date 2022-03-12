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
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorCasting : EntityBehavior
    {
        protected long abilityId;
        protected long startCastingUnixTime;
        protected long finishCastingUnixTime;
        protected long lastTickUnixTimeMs = 0;

        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorCasting"; }

        public EntityBehaviorCasting(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
            lastTickUnixTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // dont need to store these
            // as casting resets between reload
            //this.castingAbilityId = Convert.ToInt64(attributes["castingAbilityId"].AsString());
            //this.startCastingUnixTime = Convert.ToInt64(attributes["startCastingUnixTime"].AsString());
            //this.finishCastingUnixTime = Convert.ToInt64(attributes["finishCastingUnixTime"].AsString());
        }

        public bool IsWaitingToCast()
        {
            if (entity.WatchedAttributes.GetFloat("castingpct", 1.0F) == 1.0F)
                return true;

            return false;
        }

        public void TryFinishCast()
        {
            if (!IsWaitingToCast())
                return;

            var tempAbilityId = this.abilityId;
            ClearCasting();
            OnFinishCasting(tempAbilityId);
        }

        internal void UpdateCastingPercentAttribute()
        {
            if (startCastingUnixTime < 1 || finishCastingUnixTime < 1)
                return;

            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var totalTime = finishCastingUnixTime - startCastingUnixTime;
            var progress = now - startCastingUnixTime;
            float percentage = (float)((double)progress / (double)totalTime);
            if (percentage < 0.0F)
                percentage = 0.0F;
            if (percentage > 1.0F)
                percentage = 1.0F;
            if (entity.WatchedAttributes.GetFloat("castingpct", 0.0F) != percentage)
                entity.WatchedAttributes.SetFloat("castingpct", percentage);
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
            if (lastTickUnixTimeMs + 500 > DateTimeOffset.Now.ToUnixTimeMilliseconds())
                return;

            lastTickUnixTimeMs+= 500;
            OnHalfSecTick();
        }

        public void OnHalfSecTick()
        {
            if (startCastingUnixTime < 1 || finishCastingUnixTime < 1)
                return;

            var currenttime = DateTimeOffset.Now.ToUnixTimeSeconds();
            UpdateCastingPercentAttribute();
            if (currenttime < finishCastingUnixTime)
                return;
        }

        private void OnFinishCasting(long abilityId)
        {
            if (abilityId == 0)
                return;

            var mod = this.entity.World.Api.ModLoader.GetModSystem<SystemAbilities>();
            if (mod == null)
                return;

            var ability = mod.GetAbilityById(abilityId);
            if (ability == null)
                return;

            ability.FinishCast(this.entity);
        }

        internal void StartCasting(long abilityId, int duration)
        {
            this.abilityId = abilityId;
            entity.WatchedAttributes.SetFloat("castingpct", 0.0f);
            this.startCastingUnixTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            this.finishCastingUnixTime = DateTimeOffset.Now.ToUnixTimeSeconds()+(duration);

        }

        internal void ClearCasting()
        {
            startCastingUnixTime = 0;
            finishCastingUnixTime = 0;
            entity.WatchedAttributes.SetFloat("castingpct", 0.0f);
            this.abilityId = 0;
        }
    }
}
