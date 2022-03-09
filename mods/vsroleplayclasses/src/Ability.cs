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
using vsroleplayclasses.src.Behaviors;
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
        public ResistType ResistType { get; set; }
        public SpellEffectIndex SpellEffectIndex { get; set; }
        public SpellEffectType SpellEffect { get; set; }
        public PowerLevel PowerLevel { get; set; }
        public AdventureClass AdventureClass { get; set; }
        public static Ability Create(
            long id, 
            string characterName, 
            string creatorUid, 
            SpellEffectIndex spellEffectIndex,
            SpellEffectType spellEffect, 
            ResistType resistType, 
            TargetType targetType,
            PowerLevel powerLevel,
            AdventureClass adventureClass
            )
        {
            return new Ability()
            {
                Id = id,
                CreatorUid = creatorUid,
                Name = GenerateName(characterName, spellEffectIndex, spellEffect, resistType, targetType, powerLevel, adventureClass),
                SpellEffectIndex = spellEffectIndex,
                SpellEffect = spellEffect,
                ResistType = resistType,
                TargetType = targetType,
                PowerLevel = powerLevel,
                AdventureClass = adventureClass
            };
        }

        private static string GenerateName(string characterName, SpellEffectIndex spellEffectIndex, SpellEffectType spellEffect, ResistType resistType, TargetType targetType, PowerLevel powerLevel, AdventureClass adventureClass)
        {
            var name = $"{characterName}'s ";
            if (resistType != ResistType.None)
                name += $"{GetDisplayNameFromResistType(resistType)} ";

            if (spellEffect != SpellEffectType.None && spellEffectIndex != SpellEffectIndex.None)
                name += EffectCombo.GetEffectCombo(spellEffectIndex, spellEffect).Name + " ";

            if (adventureClass != AdventureClass.None)
                name += $"of {GetDisplayNameFromAdventureClass(adventureClass)} ";

            if (targetType != TargetType.None)
                name += $"{GetDisplayNameFromTarget(targetType)} ";

            if (powerLevel != PowerLevel.None)
                name += $"Rank {powerLevel}";

            return name;
        }

        public static string GetDisplayNameFromTarget(TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.None:
                    return "Nothing";
                case TargetType.Self:
                    return "Internalism";
                case TargetType.Target:
                    return "Externalism";
                case TargetType.Group:
                    return "Nepotism";
                case TargetType.AETarget:
                    return "Altruism";
                case TargetType.TargetOptional:
                    return "Mutualism";
                case TargetType.AECaster:
                    return "Pantheism";
                case TargetType.Animal:
                    return "Totemism";
                case TargetType.Undead:
                    return "Animism";
                default:
                    return "Unknown";
            }
        }

        public static string GetDisplayNameFromAdventureClass(AdventureClass adventureClass)
        {
            switch (adventureClass)
            {
                case AdventureClass.None:
                    return "Unknown";
                case AdventureClass.Warrior:
                    return "Vanquishing";
                case AdventureClass.Cleric:
                    return "Holy";
                case AdventureClass.Paladin:
                    return "Virtuous";
                case AdventureClass.Ranger:
                    return "Stalking";
                case AdventureClass.Shadowknight:
                    return "Defiling";
                case AdventureClass.Druid:
                    return "Wild";
                case AdventureClass.Monk:
                    return "Tranquil";
                case AdventureClass.Bard:
                    return "Lyrical";
                case AdventureClass.Rogue:
                    return "Deceitful";
                case AdventureClass.Shaman:
                    return "Mystical";
                case AdventureClass.Necromancer:
                    return "Unholy";
                case AdventureClass.Wizard:
                    return "Evoking";
                case AdventureClass.Magician:
                    return "Conjuring";
                case AdventureClass.Enchanter:
                    return "Beguiling";
                case AdventureClass.Beastlord:
                    return "Feral";
                case AdventureClass.Berserker:
                    return "Raging";
                default:
                    return "Unknown";
            }
        }

        public static string GetDisplayNameFromResistType(ResistType resistType)
        {
            switch (resistType)
            {
                case ResistType.None:
                    return "Nullifying";
                case ResistType.Chromatic:
                    return "Chromatic";
                case ResistType.Cold:
                    return "Freezing";
                case ResistType.Corruption:
                    return "Corrupting";
                case ResistType.Disease:
                    return "Sickening";
                case ResistType.Fire:
                    return "Fiery";
                case ResistType.Magic:
                    return "Magical";
                case ResistType.Physical:
                    return "Wounding";
                case ResistType.Poison:
                    return "Toxic";
                case ResistType.Prismatic:
                    return "Nothing";
                default:
                    return "Unknown";
            }
        }

        internal void StartCast(Entity source)
        {
            EntityBehaviorCasting ebt = source.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return;

            ebt.StartCasting(this.Id,GetCastTimeSeconds());
        }

        public EffectCombo GetEffectCombo()
        {
            return EffectCombo.GetEffectCombo(this.SpellEffectIndex, this.SpellEffect);
        }

        internal void FinishCast(Entity source, Entity clickedTarget)
        {
            var effectCombo = GetEffectCombo();
            if (effectCombo == null || effectCombo.Effect == null)
            {
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Your ability is inert", EnumChatType.CommandSuccess);
                return;
            }

            var targets = new List<Entity>();

            if (this.TargetType == TargetType.Self)
                targets.Add(source);
            if (this.TargetType == TargetType.Target)
                targets.Add(clickedTarget);

            var success = false;
            foreach (var target in targets)
            {
                var result = effectCombo.Effect(source, target, this.GetDamageType(this.AdventureClass), this.GetDamageAmount(), this.ResistType);
                if (result)
                    success = result;
            }

            source.DecreaseMana(GetManaCost());

            if (success)
                source.SkillUp(this);
            if (success)
                source.GrantSmallAmountOfAdventureClassXp(this);
        }

        private ExtendedEnumDamageType GetDamageType(AdventureClass adventureClass)
        {
            return (ExtendedEnumDamageType)(Enum.Parse(typeof(ExtendedEnumDamageType), adventureClass + "_" + this.ResistType));
        }

        private int GetCastTimeSeconds()
        {
            return 3;
        }

        private float GetDamageAmount()
        {
            var targetTypeModifier = AbilityTools.GetTargetTypeDamageAmountMultiplier(this.TargetType);
            var damageAmount = targetTypeModifier * (int)this.PowerLevel;
            return damageAmount;
        }


        internal float GetManaCost()
        {
            var targetTypeModifier = AbilityTools.GetTargetTypeManaCostMultiplier(this.TargetType);
            return ((float)targetTypeModifier*7)*(int)this.PowerLevel;
        }

    }
}
