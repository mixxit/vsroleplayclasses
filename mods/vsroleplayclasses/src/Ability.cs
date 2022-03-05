using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

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

        internal void Cast(Entity target)
        {

        }
    }
}
