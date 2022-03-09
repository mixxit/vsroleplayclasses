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
    public class EffectCombo
    {
        public System.Func<Entity, Entity, ExtendedEnumDamageType?, float?, ResistType, bool> Effect;
        public string Name { get; set; }
        public SpellEffectIndex SpellEffectIndex { get; set; }
        public SpellEffectType SpellEffectType { get; set; }

        public static EffectCombo GetEffectCombo(SpellEffectIndex spellEffectIndex, SpellEffectType spellEffectType)
        {
            if (spellEffectType == SpellEffectType.None || spellEffectIndex == SpellEffectIndex.None)
                return null;

            if (spellEffectType == SpellEffectType.CurrentHP && spellEffectIndex == SpellEffectIndex.Direct_Damage)
                return new EffectCombo() { Name = "Blast", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = DD };

            if (spellEffectType == SpellEffectType.CurrentHP && spellEffectIndex == SpellEffectIndex.Heal_Cure)
                return new EffectCombo() { Name = "Heal", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = Heal };

            if (spellEffectType == SpellEffectType.BindAffinity && spellEffectIndex == SpellEffectIndex.Dispell_Sight)
                return new EffectCombo() { Name = "Soul Binding", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = Bind };

            if (spellEffectType == SpellEffectType.Gate && spellEffectIndex == SpellEffectIndex.Vanish)
                return new EffectCombo() { Name = "Return Home", SpellEffectIndex = spellEffectIndex, SpellEffectType = spellEffectType, Effect = Gate };

            return null;
        }

        public static bool Heal(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = ExtendedEnumDamageType.Heal, float? amount = null, ResistType resistType = ResistType.None)
        {
            var result = castOn.ChangeCurrentHp(source, (float)amount, EnumDamageType.Heal);
            if (result && source.IsIServerPlayer())
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, $"You restored your target with {amount} health", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, $"You were restored by {amount} health", EnumChatType.CommandSuccess);
            return result;
        }

        public static bool DD(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = null, float? amount = null, ResistType resistType = ResistType.None)
        {
            if (castOn.IsInvulerable())
            {
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, $"Your target is invulnerable", EnumChatType.CommandSuccess);
                return false;
            }

            var result = castOn.ChangeCurrentHp(source, (float)amount, (EnumDamageType)damageType);
            if (result && source.IsIServerPlayer())
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, $"You blasted your target with {amount} {(resistType).ToString()} damage", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, $"You were blasted by {amount} {(resistType).ToString()} damage", EnumChatType.CommandSuccess);
            return result;
        }

        public static bool Bind(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = null, float? amount = null, ResistType resistType = ResistType.None)
        {
            var result = castOn.BindToLocation();
            if (result && source.IsIServerPlayer())
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "You bound your target to their location", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "You are bound to your location", EnumChatType.CommandSuccess);
            return result;
        }

        public static bool Gate(Entity source, Entity castOn, ExtendedEnumDamageType? damageType = null, float? amount = null, ResistType resistType = ResistType.None)
        {
            var result = castOn.GateToBind();
            if (result && source.IsIServerPlayer())
                source.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Your target stepped through a gate", EnumChatType.CommandSuccess);
            if (result && castOn.IsIServerPlayer())
                castOn.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "You stepped through a gate", EnumChatType.CommandSuccess);
            return result;
        }
    }
}
