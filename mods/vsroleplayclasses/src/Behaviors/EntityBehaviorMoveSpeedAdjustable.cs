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
            if (entity is EntityAgent && !(entity is EntityPlayer))
            {
                var behaviorTaskManager = entity.GetBehavior<EntityBehaviorTaskAI>();
                if (behaviorTaskManager == null)
                    return;

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskFleeEntitySpeed", 0.0f) == 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskFleeEntity>();
                    if (aiTask != null)
                        entity.WatchedAttributes.SetFloat("defaultAiTaskFleeEntitySpeed", aiTask.GetMoveSpeed());
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskGotoEntitySpeed", 0.0f) == 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskGotoEntity>();
                    if (aiTask != null)
                        entity.WatchedAttributes.SetFloat("defaultAiTaskGotoEntitySpeed", aiTask.GetMoveSpeed());
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskSeekEntitySpeed", 0.0f) == 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskSeekEntity>();
                    if (aiTask != null)
                        entity.WatchedAttributes.SetFloat("defaultAiTaskSeekEntitySpeed", aiTask.GetMoveSpeed());
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskStayCloseToEntitySpeed", 0.0f) == 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskStayCloseToEntity>();
                    if (aiTask != null)
                        entity.WatchedAttributes.SetFloat("defaultAiTaskStayCloseToEntitySpeed", aiTask.GetMoveSpeed());
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskWanderSpeed", 0.0f) == 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskWander>();
                    if (aiTask != null)
                        entity.WatchedAttributes.SetFloat("defaultAiTaskWanderSpeed", aiTask.GetMoveSpeed());
                }
            }
        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        internal void ResetHasteRunspeedState()
        {
            if (!(entity is EntityAgent))
                return;

            var calculatedMoveSpeed = CalculateMoveSpeedBonus();

            if (entity is EntityAgent && !(entity is EntityPlayer))
            {
                var behaviorTaskManager = entity.GetBehavior<EntityBehaviorTaskAI>();
                if (behaviorTaskManager == null)
                    return;

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskFleeEntitySpeed", 0.0f) > 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskFleeEntity>();
                    var newTotal = entity.WatchedAttributes.GetFloat("defaultAiTaskFleeEntitySpeed", 0.0f) + calculatedMoveSpeed;
                    if (aiTask != null)
                        aiTask.SetMoveSpeed(newTotal < 0 ? 0 : newTotal);
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskGotoEntitySpeed", 0.0f) > 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskGotoEntity>();
                    var newTotal = entity.WatchedAttributes.GetFloat("defaultAiTaskGotoEntitySpeed", 0.0f) + calculatedMoveSpeed;
                    if (aiTask != null)
                        aiTask.SetMoveSpeed(newTotal < 0 ? 0 : newTotal);
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskSeekEntitySpeed", 0.0f) > 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskSeekEntity>();
                    var newTotal = entity.WatchedAttributes.GetFloat("defaultAiTaskSeekEntitySpeed", 0.0f) + calculatedMoveSpeed;
                    if (aiTask != null)
                        aiTask.SetMoveSpeed(newTotal < 0 ? 0 : newTotal);
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskStayCloseToEntitySpeed", 0.0f) > 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskStayCloseToEntity>();
                    var newTotal = entity.WatchedAttributes.GetFloat("defaultAiTaskStayCloseToEntitySpeed", 0.0f) + calculatedMoveSpeed;
                    if (aiTask != null)
                        aiTask.SetMoveSpeed(newTotal < 0 ? 0 : newTotal);
                }

                if (entity.WatchedAttributes.GetFloat("defaultAiTaskWanderSpeed", 0.0f) > 0.0f)
                {
                    var aiTask = behaviorTaskManager.TaskManager.GetTask<AiTaskWander>();
                    var newTotal = entity.WatchedAttributes.GetFloat("defaultAiTaskWanderSpeed", 0.0f) + calculatedMoveSpeed;
                    if (aiTask != null)
                        aiTask.SetMoveSpeed(newTotal < 0 ? 0 : newTotal);
                }
            }

            //((EntityAgent)entity).ServerControls.MovespeedMultiplier = calculatedMoveSpeed;
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
            return (ability.GetAmount() / 100) * -1;
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

            return ability.GetAmount() / 100;
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }

    }
}
