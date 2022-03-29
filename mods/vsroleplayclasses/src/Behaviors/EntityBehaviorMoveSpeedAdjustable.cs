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
using Vintagestory.GameContent;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorMoveSpeedAdjustable : EntityBehavior
    {
        protected long abilityId;
        protected string abilityName;
        protected float defaultRunspeed = 1.0F;
        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorMoveSpeedAdjustable"; }

        public EntityBehaviorMoveSpeedAdjustable(Entity entity) : base(entity) {

        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public override void OnEntityLoaded()
        {
            base.OnEntityLoaded();
            if (entity is EntityItem)
                return;

            entity.ResetHasteRunspeedState();
        }

        public override void OnEntitySpawn()
        {
            base.OnEntitySpawn();
            if (entity is EntityItem)
                return;

            entity.ResetHasteRunspeedState();
        }


        internal void ResetHasteRunspeedState()
        {
            if (!(entity is EntityAgent))
                return;

            var walkSpeedBonus = CalculateMoveSpeedBonus();

            if (entity is EntityAgent)
                entity.WatchedAttributes.SetFloat("currentWalkSpeedBonus", walkSpeedBonus);
        }

        private float CalculateMoveSpeedBonus()
        {
            return SnareSpeedEffectModifier() + RootEffectModifier() + RunSpeedEffectModifier();
        }

        private float SnareSpeedEffectModifier()
        {
            EntityBehaviorSpellEffects ebt = entity.GetBehavior("EntityBehaviorSpellEffects") as EntityBehaviorSpellEffects;
            if (ebt == null)
                return 0.0f;

            // just root for now
            var modifier = ebt.GetActiveEffectOfType(SpellEffectIndex.Haste_Runspeed, SpellEffectType.MovementSpeed);

            if (modifier == null)
                return 0.0f;

            var ability = ebt.GetAbility(modifier.AbilityId);
            if (ability == null)
                return 0.0f;

            // Negative runspeed
            return (ability.GetAmount() / 10) * -1;
        }

        private float RootEffectModifier()
        {
            EntityBehaviorSpellEffects ebt = entity.GetBehavior("EntityBehaviorSpellEffects") as EntityBehaviorSpellEffects;
            if (ebt == null)
                return 0.0f;

            // just root for now
            var modifier = ebt.GetActiveEffectOfType(SpellEffectIndex.Haste_Runspeed, SpellEffectType.Root);

            if (modifier == null)
                return 0.0f;

            var ability = ebt.GetAbility(modifier.AbilityId);
            if (ability == null)
                return 0.0f;

            // Negative runspeed at super high amount
            return (ability.GetAmount())*-1;
        }

        private float RunSpeedEffectModifier()
        {
            EntityBehaviorSpellEffects ebt = entity.GetBehavior("EntityBehaviorSpellEffects") as EntityBehaviorSpellEffects;
            if (ebt == null)
                return 0.0f;

            // just root for now
            var modifier = ebt.GetActiveEffectOfType(SpellEffectIndex.Stat_Buff, SpellEffectType.MovementSpeed);

            if (modifier == null)
                return 0.0f;

            var ability = ebt.GetAbility(modifier.AbilityId);
            if (ability == null)
                return 0.0f;

            return ability.GetAmount() / 10;
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }

    }
}
