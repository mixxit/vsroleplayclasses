using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src
{
    [JsonObject]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class Ability
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CreatorUid { get; set; }
        public SpellEffectType effectType { get; set; }
        public TargetType TargetType { get; set; }
        public SpellPolarity SpellPolarity { get; set; }
        public ResistType ResistType { get; set; }
        public SpellEffectIndex SpellEffectIndex { get; set; }
        public SpellEffectType SpellEffect { get; set; }
        public PowerLevel PowerLevel { get; set; }

        public static Ability Create(
            long id, 
            string characterName, 
            string creatorUid, 
            SpellEffectIndex spellEffectIndex,
            SpellEffectType spellEffect, 
            ResistType resistType, 
            TargetType targetType,
            SpellPolarity spellPolarity,
            PowerLevel powerLevel
            )
        {
            return new Ability()
            {
                Id = id,
                CreatorUid = creatorUid,
                Name = GenerateName(characterName, spellEffectIndex, spellEffect, resistType, targetType, spellPolarity, powerLevel),
                SpellEffectIndex = spellEffectIndex,
                SpellEffect = spellEffect,
                ResistType = resistType,
                TargetType = targetType,
                SpellPolarity = spellPolarity,
                PowerLevel = powerLevel
            };
        }

        private static string GenerateName(string characterName, SpellEffectIndex spellEffectIndex, SpellEffectType spellEffect, ResistType resistType, TargetType targetType, SpellPolarity spellPolarity, PowerLevel powerLevel)
        {
            var name = $"{characterName}'s ";

            if (spellPolarity == SpellPolarity.Positive)
                name += "Add ";
            if (spellPolarity == SpellPolarity.Negative)
                name += "Remove ";
            if (spellEffectIndex != SpellEffectIndex.None)
                name += spellEffectIndex.ToString() + "ing ";
            if (spellEffect != SpellEffectType.None)
                name += spellEffect.ToString() + " ";
            if (targetType != TargetType.None)
                name += $"to {targetType.ToString()} ";
            if (powerLevel != PowerLevel.None)
                name += $"Rank {powerLevel.ToString()}";

            return name;
        }

        internal void Cast(Entity source, Entity clickedTarget)
        {
            var targets = new List<Entity>();

            if (this.TargetType == TargetType.Self)
                targets.Add(source);
            if (this.TargetType == TargetType.Target)
                targets.Add(clickedTarget);


            var success = false;
            foreach (var target in targets)
            {
                switch (this.SpellEffect)
                {
                    case SpellEffectType.BindAffinity:
                        if (Bind(source, target))
                            success = true;
                        return;
                    case SpellEffectType.Gate:
                        if (Gate(source, target))
                            success = true;
                        return;
                    case SpellEffectType.CurrentHP:
                        if (ChangeCurrentHp(source, target))
                            success = true;
                        return;
                    default:
                        if (source.IsIServerPlayer())
                            source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Your ability is inert", EnumChatType.CommandSuccess);
                        return;
                }
            }

            if (success)
                source.SkillUp(this);
        }

        private bool ChangeCurrentHp(Entity source, Entity castOn)
        {
            var amount = GetDamageAmount();
            var type = GetDamageType();
            var result = castOn.ChangeCurrentHp(source, amount, type);
            if (result && source.IsIServerPlayer())
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, $"You hit your target with {amount} {GetDamageType()}", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, $"You were hit by {amount} {GetDamageType()}", EnumChatType.CommandSuccess);
            return result;
        }

        private EnumDamageType GetDamageType()
        {
            if (this.SpellPolarity == SpellPolarity.Positive)
                return EnumDamageType.Heal;

            if (ResistType == ResistType.Poison)
                return EnumDamageType.Poison;
            if (ResistType == ResistType.Disease)
                return EnumDamageType.Poison;
            if (ResistType == ResistType.Fire)
                return EnumDamageType.Fire;
            if (ResistType == ResistType.Cold)
                return EnumDamageType.Frost;
            if (ResistType == ResistType.Magic)
                return EnumDamageType.Suffocation;

            return EnumDamageType.BluntAttack;
        }

        private float GetDamageAmount()
        {
            var targetTypeModifier = AbilityTools.GetTargetTypeDamageAmountMultiplier(this.TargetType);
            var damageAmount = targetTypeModifier * (int)this.PowerLevel;

            // Essentially a heal
            if (this.SpellPolarity == SpellPolarity.Positive)
                damageAmount *= -1;

            return damageAmount;
        }

        private bool Gate(Entity source, Entity castOn)
        {
            var result = castOn.GateToBind();
            if (result && source.IsIServerPlayer())
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Your target stepped through a gate", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "You stepped through a gate", EnumChatType.CommandSuccess);
            return result;
        }

        private bool Bind(Entity source, Entity castOn)
        {
            var result = castOn.BindToLocation();
            if (result && source.IsIServerPlayer())
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "You bound your target to their location", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "You are bound to your location", EnumChatType.CommandSuccess);
            return result;
        }

        internal float GetManaCost()
        {
            var targetTypeModifier = AbilityTools.GetTargetTypeManaCostMultiplier(this.TargetType);
            return ((float)targetTypeModifier*7)*(int)this.PowerLevel;
        }

    }
}
