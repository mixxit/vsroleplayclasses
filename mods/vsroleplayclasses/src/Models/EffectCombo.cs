using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src.Models
{
    public class EffectCombo
    {
        public System.Func<Entity, Entity, ExtendedEnumDamageType?, float?, ResistType, bool, bool> Effect;
        public string Name { get; set; }
        public SpellEffectIndex SpellEffectIndex { get; set; }
        public SpellEffectType SpellEffectType { get; set; }
        public long Duration { get; set; }

        public static EffectCombo GetEffectCombo(SpellEffectIndex spellEffectIndex, SpellEffectType spellEffectType, long duration)
        {
            if (spellEffectType == SpellEffectType.None || spellEffectIndex == SpellEffectIndex.None)
                return null;

            if (spellEffectType == SpellEffectType.STR && spellEffectIndex == SpellEffectIndex.Stat_Buff)
                return new EffectCombo() { Name = "Strength", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = StatBuffSTR, Duration = duration };
            if (spellEffectType == SpellEffectType.STA && spellEffectIndex == SpellEffectIndex.Stat_Buff)
                return new EffectCombo() { Name = "Stamina", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = StatBuffSTA, Duration = duration };
            if (spellEffectType == SpellEffectType.DEX && spellEffectIndex == SpellEffectIndex.Stat_Buff)
                return new EffectCombo() { Name = "Dexterity", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = StatBuffDEX, Duration = duration };
            if (spellEffectType == SpellEffectType.AGI && spellEffectIndex == SpellEffectIndex.Stat_Buff)
                return new EffectCombo() { Name = "Agility", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = StatBuffAGI, Duration = duration };
            if (spellEffectType == SpellEffectType.WIS && spellEffectIndex == SpellEffectIndex.Stat_Buff)
                return new EffectCombo() { Name = "Wisdom", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = StatBuffWIS, Duration = duration };
            if (spellEffectType == SpellEffectType.INT && spellEffectIndex == SpellEffectIndex.Stat_Buff)
                return new EffectCombo() { Name = "Intelligence", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = StatBuffINT, Duration = duration };
            if (spellEffectType == SpellEffectType.CHA && spellEffectIndex == SpellEffectIndex.Stat_Buff)
                return new EffectCombo() { Name = "Charisma", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = StatBuffCHA, Duration = duration };

            if (spellEffectType == SpellEffectType.CurrentHP && spellEffectIndex == SpellEffectIndex.Direct_Damage)
                return new EffectCombo() { Name = "Blast", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = DD, Duration = duration };

            if (spellEffectType == SpellEffectType.CurrentMana && spellEffectIndex == SpellEffectIndex.Mana_Regen_Resist_Song)
                return new EffectCombo() { Name = "Clarity", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = HealMana, Duration = duration };

            if (spellEffectType == SpellEffectType.CurrentHP && spellEffectIndex == SpellEffectIndex.Mana_Regen_Resist_Song)
                return new EffectCombo() { Name = "Regen", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = Heal, Duration = duration };

            if (spellEffectType == SpellEffectType.CurrentHP && spellEffectIndex == SpellEffectIndex.Heal_Cure)
                return new EffectCombo() { Name = "Heal", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = Heal, Duration = duration };

            if (spellEffectType == SpellEffectType.BindAffinity && spellEffectIndex == SpellEffectIndex.Dispell_Sight)
                return new EffectCombo() { Name = "Soul Binding", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = Bind, Duration = duration };

            if (spellEffectType == SpellEffectType.Gate && spellEffectIndex == SpellEffectIndex.Vanish)
                return new EffectCombo() { Name = "Return Home", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = Gate, Duration = duration };

            return null;
        }

        public static bool StatBuffSTR(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You reinforced your target with {amount} strength", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer() && firstRun)
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were reinforced by {amount} strength", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool StatBuffSTA(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You reinforced your target with {amount} stamina", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer() && firstRun)
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were reinforced by {amount} stamin", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool StatBuffDEX(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You reinforced your target with {amount} dexterity", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer() && firstRun)
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were reinforced by {amount} dexterity", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool StatBuffAGI(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You reinforced your target with {amount} agility", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer() && firstRun)
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were reinforced by {amount} agility", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool StatBuffWIS(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You reinforced your target with {amount} wisdom", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer() && firstRun)
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were reinforced by {amount} wisdom", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool StatBuffINT(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You reinforced your target with {amount} intelligence", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer() && firstRun)
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were reinforced by {amount} intelligence", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool StatBuffCHA(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You reinforced your target with {amount} charisma", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer() && firstRun)
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were reinforced by {amount} charisma", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool HealMana(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            castOn.IncreaseMana((float)amount);
            if (source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You restored your target with {amount} mana", EnumChatType.CommandSuccess);
            if (castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were restored by {amount} mana", EnumChatType.CommandSuccess);
            return true;
        }

        public static bool Heal(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            var result = castOn.ChangeCurrentHp(source, (float)amount, EnumDamageType.Heal);
            if (result && source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You restored your target with {amount} health", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"You were restored by {amount} health", EnumChatType.CommandSuccess);
            return result;
        }

        public static bool DD(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = null, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            // ignore invulnerable
            if (castOn.IsInvulerable())
                return false;

            // ignore self
            if (source.EntityId == castOn.EntityId)
                return false;

            var result = castOn.ChangeCurrentHp(source, (float)amount, (EnumDamageType)damageType);

            if (result && source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.DamageLogChatGroup, $"You blasted your target with {amount} {(resistType).ToString()} damage", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.DamageLogChatGroup, $"You were blasted by {amount} {(resistType).ToString()} damage", EnumChatType.CommandSuccess);
            return result;
        }

        public static bool Bind(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = null, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            var result = castOn.BindToLocation();
            if (result && source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, "You bound your target to their location", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, "You are bound to your location", EnumChatType.CommandSuccess);
            return result;
        }

        public static bool Gate(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = null, float? amount = null, ResistType resistType = ResistType.None, bool firstRun = true)
        {
            var result = castOn.GateToBind();
            if (result && source.IsIServerPlayer() && firstRun)
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, "Your target stepped through a gate", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, "You stepped through a gate", EnumChatType.CommandSuccess);
            return result;
        }
    }
}
