using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src
{
    class SkillUtils
    {
        internal static SkillType GetSkillTypeFromDamageType(ExtendedEnumDamageType damageType, bool thrownWeapon)
        {
            if (!thrownWeapon)
            switch (damageType)
            {
                case ExtendedEnumDamageType.SlashingAttack:
                    return SkillType.Slashing;
                case ExtendedEnumDamageType.BluntAttack:
                    return SkillType.Crushing;
                case ExtendedEnumDamageType.PiercingAttack:
                    return SkillType.Piercing;
                    default:
                        return SkillType.Crushing;
                }
                            

            if (thrownWeapon)
                switch (damageType)
                {
                    case ExtendedEnumDamageType.BluntAttack:
                        return SkillType.Throwing;
                    case ExtendedEnumDamageType.PiercingAttack:
                        return SkillType.Archery;
                    default:
                        return SkillType.Crushing;
                }

            return SkillType.Crushing;
        }

        internal static int GetCriticalChanceBonus(Entity entity, SkillType skill)
        {
            int critical_chance = 0;

            // All skills + Skill specific
            //critical_chance += entity.getSpellBonuses(SpellEffectType.CriticalHitChance)
            //        + entity.getAABonuses(SpellEffectType.CriticalHitChance) + entity.getItemBonuses(SpellEffectType.CriticalHitChance)
             //       ;

            // TODO - take items, aa spells etc into account
            if (critical_chance < -100)
                critical_chance = -100;

            return critical_chance;
        }

        internal static int GetCritDmgMod(SkillType skill)
        {
            int critDmg_mod = 0;
            // TODO take aa and item bonuses into affect
            return critDmg_mod;
        }
    }
}
