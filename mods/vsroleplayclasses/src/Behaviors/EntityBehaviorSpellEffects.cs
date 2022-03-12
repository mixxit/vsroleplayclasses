using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorSpellEffects : EntityBehavior
    {
        ConcurrentDictionary<long, ActiveSpellEffect> activeSpellEffects = new ConcurrentDictionary<long, ActiveSpellEffect>();
        private SystemAbilities abilitiesMod;
        protected long lastTickUnixTimeMs = 0;

        public EntityBehaviorSpellEffects(Entity entity) : base(entity)
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            lastTickUnixTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            abilitiesMod = this.entity.World.Api.ModLoader.GetModSystem<SystemAbilities>();
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
            if (lastTickUnixTimeMs + 8000 > DateTimeOffset.Now.ToUnixTimeMilliseconds())
                return;

            lastTickUnixTimeMs += 8000;
            TickEffects();
        }

        public override string PropertyName()
        {
            return "EntityBehaviorSpellEffects";
        }

        public override void OnEntityLoaded()
        {

        }

        public override void OnEntitySpawn()
        {

        }

        internal void TickEffects()
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            if (abilitiesMod == null)
                return;


            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            foreach(var effect in activeSpellEffects.Keys)
                TickEffect(effect);
        }

        private void TickEffect(long abilityId)
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            if (abilitiesMod == null)
                return;

            if (!this.activeSpellEffects.ContainsKey(abilityId))
                return;

            var ability = abilitiesMod.GetAbilityById(abilityId);

            if (activeSpellEffects[abilityId] == null || activeSpellEffects[abilityId].Duration == 0 || ability == null || ability.GetEffectCombo() == null || activeSpellEffects[abilityId].SourceEntityId < 1)
            {
                UnqueueTickEffect(abilityId);
                return;
            }

            var sourceEntity = this.entity.World.GetEntityById(activeSpellEffects[abilityId].SourceEntityId);
            if (sourceEntity == null)
            {
                UnqueueTickEffect(abilityId);
                return;
            }

            abilitiesMod.GetAbilityById(abilityId).GetEffectCombo().Effect(sourceEntity, this.entity, ability.GetDamageType(ability.AdventureClass), ability.GetDamageAmount(), ability.ResistType, false);
            activeSpellEffects[abilityId].Duration--;
        }

        private void UnqueueTickEffect(long abilityId)
        {
            this.activeSpellEffects.TryRemove(abilityId, out _);
        }

        internal void QueueTickEffect(long sourceEntityId, long abilityId, int duration)
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            activeSpellEffects[abilityId] = new ActiveSpellEffect() { SourceEntityId = sourceEntityId, AbilityId = abilityId, Duration = duration };
        }
    }
}
