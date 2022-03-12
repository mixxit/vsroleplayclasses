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
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Entities;
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
                name += EffectCombo.GetEffectCombo(spellEffectIndex, spellEffect, 0).Name + " ";

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
            return EffectCombo.GetEffectCombo(this.SpellEffectIndex, this.SpellEffect, GetDuration());
        }

        private int GetDuration()
        {
            if (this.SpellEffectIndex == SpellEffectIndex.Mana_Regen_Resist_Song)
                return (int)this.PowerLevel * 8;

            // most things are zero
            return 0;
        }

        internal void FinishCast(Entity source, bool forceSelf = false)
        {
            var effectCombo = GetEffectCombo();
            if (effectCombo == null || effectCombo.Effect == null)
            {
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Your ability is inert", EnumChatType.CommandSuccess);
                return;
            }

            var targets = new List<Entity>();

            // treat as though collided
            if (this.TargetType == TargetType.Self || forceSelf)
                OnSpellCollidedEntity(source, source, effectCombo, this.GetDamageType(this.AdventureClass), this.GetDamageAmount(), this.ResistType);

            if (this.TargetType == TargetType.Target && !forceSelf)
                FlingSpellForward(source, effectCombo, this.GetDamageType(this.AdventureClass), this.GetDamageAmount(), this.ResistType);

            source.DecreaseMana(GetManaCost());
            source.SkillUp(this);
            source.GrantSmallAmountOfAdventureClassXp(this);
        }

        private void FlingSpellForward(Entity byEntity, EffectCombo effectCombo, ExtendedEnumDamageType extendedEnumDamageType, float damageAmount, ResistType resistType)
        {
            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer) byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
            byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/throw"), byEntity, byPlayer, false, 8);

            EntityProperties type = byEntity.World.GetEntityType(new AssetLocation("vsroleplayclasses:magicprojectile"));
            EntityMagicProjectile enpr = byEntity.World.ClassRegistry.CreateEntity(type) as EntityMagicProjectile;
            enpr.FiredBy = byEntity;
            enpr.AbilityId = this.Id;
            enpr.WatchedAttributes.SetLong("particle_red", GetParticleColour()[0]);
            enpr.WatchedAttributes.SetLong("particle_green", GetParticleColour()[1]);
            enpr.WatchedAttributes.SetLong("particle_blue", GetParticleColour()[2]);

            Vec3d pos = byEntity.ServerPos.XYZ.Add(0, byEntity.LocalEyePos.Y - 0.2, 0);

            Vec3d aheadPos = pos.AheadCopy(1, byEntity.ServerPos.Pitch, byEntity.ServerPos.Yaw);
            Vec3d velocity = (aheadPos - pos) * 0.65;
            Vec3d spawnPos = byEntity.ServerPos.BehindCopy(0.21).XYZ.Add(byEntity.LocalEyePos.X, byEntity.LocalEyePos.Y - 0.2, byEntity.LocalEyePos.Z);

            enpr.ServerPos.SetPos(spawnPos);
            enpr.ServerPos.Motion.Set(velocity);

            enpr.Pos.SetFrom(enpr.ServerPos);
            enpr.World = byEntity.World;
            ((EntityMagicProjectile)enpr).SetRotation();

            byEntity.World.SpawnEntity(enpr);
            byEntity.StartAnimation("throw");
        }

        private int[] GetParticleColour()
        {
            switch(ResistType)
            {
                case ResistType.Cold:
                    return new int[] { 0, 0, 255, 255 };
                case ResistType.Fire:
                    return new int[] { 255, 0, 0, 255 };
                case ResistType.Disease:
                    return new int[] { 139, 69, 19, 255 };
                case ResistType.Poison:
                    return new int[] { 107, 142, 35, 255 };
                case ResistType.Physical:
                    return new int[] { 128, 128, 128, 255 };
                case ResistType.Chromatic:
                    return new int[] { 219, 226, 233, 255 };
                case ResistType.Prismatic:
                    return new int[] { 255, 255,255, 255 };
                case ResistType.Corruption:
                    return new int[] { 0, 0, 0, 255 };
                case ResistType.Magic:
                    return new int[] { 255, 192, 203, 255 };
                default:
                    return new int[] { 255, 255, 0, 255 };
            }

        }

        internal void OnSpellCollidedEntity(Entity source, Entity target)
        {
            OnSpellCollidedEntity(source, target, this.GetEffectCombo(), this.GetDamageType(this.AdventureClass), this.GetDamageAmount(), this.ResistType);
        }

        public void OnSpellCollidedEntity(Entity source, Entity target, EffectCombo effectCombo, ExtendedEnumDamageType extendedEnumDamageType, float damageAmount, ResistType resistType)
        {
            var result = effectCombo.Effect(source, target, extendedEnumDamageType, damageAmount, resistType, true);
            if (effectCombo.Duration > 0)
                target.QueueTickEffect(source, this, effectCombo.Duration);
        }

        public ExtendedEnumDamageType GetDamageType(AdventureClass adventureClass)
        {
            return (ExtendedEnumDamageType)(Enum.Parse(typeof(ExtendedEnumDamageType), adventureClass + "_" + this.ResistType));
        }

        private int GetCastTimeSeconds()
        {
            return 3;
        }

        public float GetDamageAmount()
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
