using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Packets;
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
            base.OnEntityLoaded();
            if (entity is EntityItem)
                return;

            if (!(entity is EntityPlayer))
                return;

            RegisterEntityEffectsChangedListener();
        }

        private void RegisterEntityEffectsChangedListener()
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return;
                
            entity.WatchedAttributes.RegisterModifiedListener("spelleffects", (System.Action)(() => OnEntityEffectsServerSideChanged()));
        }

        private void OnEntityEffectsServerSideChanged()
        {
            if (!(entity is EntityPlayer))
                return;

            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            if (!(entity.Api is ICoreServerAPI))
                return;

            var activeEffectsMod = entity.World.Api.ModLoader.GetModSystem<SystemActiveEffects>();
            if (activeEffectsMod != null)
                activeEffectsMod.SendHudUpdatePacket(activeSpellEffects, (IServerPlayer)((EntityPlayer)entity).Player);
        }

        public override void OnEntitySpawn()
        {
            base.OnEntitySpawn();
            if (entity is EntityItem)
                return;

            if (!(entity is EntityPlayer))
                return;

            RegisterEntityEffectsChangedListener();
        }

        public ActiveSpellEffect GetActiveEffectOfType(SpellEffectIndex effectIndex, SpellEffectType type)
        {
            foreach(var abilityId in activeSpellEffects.Keys)
            {
                var ability = abilitiesMod.GetAbilityById(abilityId);
                if (ability == null)
                    continue;

                if (ability.SpellEffect == type && ability.SpellEffectIndex == effectIndex)
                    return activeSpellEffects[abilityId];
            }

            return null;
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

            entity.WatchedAttributes.SetString("spelleffects", GetActiveEffectsAsString());
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

            GetAbility(abilityId).GetEffectCombo().Effect(sourceEntity, this.entity, ability.GetDamageType(ability.AdventureClass), ability.GetAmount(), ability.ResistType, false);
            activeSpellEffects[abilityId].Duration--;
        }

        public Ability GetAbility(long abilityId)
        {
            return abilitiesMod.GetAbilityById(abilityId);
        }

        public Entity GetSourceEntity(long entityId)
        {
            return entity.World.GetEntityById(entityId);
        }


        private void UnqueueTickEffect(long abilityId)
        {
            this.activeSpellEffects.TryRemove(abilityId, out _);
            OnActiveSpellEffectUnqueued(GetAbility(abilityId));
        }

        internal bool QueueTickEffect(long sourceEntityId, long abilityId, long duration)
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return false;

            if (activeSpellEffects.Count > WorldLimits.MaxActiveEffectSlots)
                return false;

            activeSpellEffects[abilityId] = new ActiveSpellEffect() { SourceEntityId = sourceEntityId, AbilityId = abilityId, Duration = duration };


            OnActiveSpellEffectQueued(GetAbility(abilityId));

            return true;
        }

        public void OnActiveSpellEffectQueued(Ability ability)
        {
            entity.WatchedAttributes.SetString("spelleffects", GetActiveEffectsAsString());

            if (ability.SpellEffectIndex == SpellEffectIndex.Stat_Buff)
                entity.ResetStatisticState();
        }

        private string GetActiveEffectsAsString()
        {
            var result = activeSpellEffects.Select(e => e.Value.AbilityId +","+e.Value.SourceEntityId+","+e.Value.Duration).ToArray();
            return String.Join("|", result);
        }

        public void OnActiveSpellEffectUnqueued(Ability ability)
        {
            entity.WatchedAttributes.SetString("spelleffects", GetActiveEffectsAsString());

            if (ability.SpellEffectIndex == SpellEffectIndex.Stat_Buff)
                entity.ResetStatisticState();
        }
    }
}
