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

		public static int GetSkillCheckBonus(String skill, AdventureClass adventureClass, int level)
		{
			if (adventureClass == AdventureClass.None)
				return 0;

			if (level < 10)
				return 0;

			switch (skill)
			{
				case "athletics":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 2;
						case "CLERIC":
							return 0;
						case "RANGER":
							return 1;
						case "ROGUE":
							return 1;
						case "WIZARD":
							return -1;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return 1;
						case "SHAMAN":
							return 1;
						case "DRUID":
							return 1;
						case "BARD":
							return 1;
						case "MAGICIAN":
							return -1;
						case "MONK":
							return 2;
						case "NECROMANCER":
							return -1;
						case "ENCHANTER":
							return 0;
						case "BEASTLORD":
							return 2;
						case "BERSERKER":
							return 2;
						default:
							return 0;
					}
				case "acrobatics":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return -2;
						case "CLERIC":
							return 1;
						case "RANGER":
							return 1;
						case "ROGUE":
							return 2;
						case "WIZARD":
							return 1;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return 1;
						case "SHAMAN":
							return 1;
						case "DRUID":
							return 1;
						case "BARD":
							return 2;
						case "MAGICIAN":
							return 1;
						case "MONK":
							return 2;
						case "NECROMANCER":
							return 1;
						case "ENCHANTER":
							return 1;
						case "BEASTLORD":
							return 1;
						case "BERSERKER":
							return 0;
						default:
							return 0;
					}
				case "sleightofhand":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 0;
						case "CLERIC":
							return 0;
						case "RANGER":
							return 0;
						case "ROGUE":
							return 2;
						case "WIZARD":
							return 1;
						case "PALADIN":
							return 0;
						case "SHADOWKNIGHT":
							return 0;
						case "SHAMAN":
							return 0;
						case "DRUID":
							return -1;
						case "BARD":
							return 1;
						case "MAGICIAN":
							return 2;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return 1;
						case "ENCHANTER":
							return 1;
						case "BEASTLORD":
							return 0;
						case "BERSERKER":
							return 0;
						default:
							return 0;
					}
				case "stealth":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return -1;
						case "CLERIC":
							return 0;
						case "RANGER":
							return 1;
						case "ROGUE":
							return 2;
						case "WIZARD":
							return 1;
						case "PALADIN":
							return -1;
						case "SHADOWKNIGHT":
							return 1;
						case "SHAMAN":
							return 0;
						case "DRUID":
							return 1;
						case "BARD":
							return 1;
						case "MAGICIAN":
							return 0;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return 1;
						case "ENCHANTER":
							return 2;
						case "BEASTLORD":
							return 1;
						case "BERSERKER":
							return -1;
						default:
							return 0;
					}
				case "arcana":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return -2;
						case "CLERIC":
							return 0;
						case "RANGER":
							return -1;
						case "ROGUE":
							return -2;
						case "WIZARD":
							return 2;
						case "PALADIN":
							return -1;
						case "SHADOWKNIGHT":
							return 1;
						case "SHAMAN":
							return 1;
						case "DRUID":
							return 1;
						case "BARD":
							return -1;
						case "MAGICIAN":
							return 2;
						case "MONK":
							return -2;
						case "NECROMANCER":
							return 2;
						case "ENCHANTER":
							return 2;
						case "BEASTLORD":
							return -1;
						case "BERSERKER":
							return -2;
						default:
							return 0;
					}
				case "history":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return 1;
						case "RANGER":
							return -2;
						case "ROGUE":
							return -2;
						case "WIZARD":
							return 2;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return 1;
						case "SHAMAN":
							return 0;
						case "DRUID":
							return 1;
						case "BARD":
							return 2;
						case "MAGICIAN":
							return 0;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return 1;
						case "ENCHANTER":
							return -1;
						case "BEASTLORD":
							return -1;
						case "BERSERKER":
							return 1;
						default:
							return 0;
					}
				case "investigation":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return 1;
						case "RANGER":
							return 1;
						case "ROGUE":
							return 0;
						case "WIZARD":
							return 2;
						case "PALADIN":
							return 0;
						case "SHADOWKNIGHT":
							return 0;
						case "SHAMAN":
							return 0;
						case "DRUID":
							return 1;
						case "BARD":
							return 0;
						case "MAGICIAN":
							return 0;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return 1;
						case "ENCHANTER":
							return -1;
						case "BEASTLORD":
							return 2;
						case "BERSERKER":
							return -1;
						default:
							return 0;
					}
				case "nature":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 0;
						case "CLERIC":
							return -1;
						case "RANGER":
							return 2;
						case "ROGUE":
							return 0;
						case "WIZARD":
							return -2;
						case "PALADIN":
							return -1;
						case "SHADOWKNIGHT":
							return -2;
						case "SHAMAN":
							return 2;
						case "DRUID":
							return 2;
						case "BARD":
							return -1;
						case "MAGICIAN":
							return -1;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return -2;
						case "ENCHANTER":
							return -1;
						case "BEASTLORD":
							return 2;
						case "BERSERKER":
							return 0;
						default:
							return 0;
					}
				case "religion":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 0;
						case "CLERIC":
							return 2;
						case "RANGER":
							return -1;
						case "ROGUE":
							return -2;
						case "WIZARD":
							return -1;
						case "PALADIN":
							return 2;
						case "SHADOWKNIGHT":
							return 1;
						case "SHAMAN":
							return 0;
						case "DRUID":
							return 1;
						case "BARD":
							return -1;
						case "MAGICIAN":
							return -1;
						case "MONK":
							return 2;
						case "NECROMANCER":
							return 0;
						case "ENCHANTER":
							return -1;
						case "BEASTLORD":
							return -1;
						case "BERSERKER":
							return 0;
						default:
							return 0;
					}
				case "animalhandling":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 0;
						case "CLERIC":
							return 1;
						case "RANGER":
							return 1;
						case "ROGUE":
							return -1;
						case "WIZARD":
							return -1;
						case "PALADIN":
							return 0;
						case "SHADOWKNIGHT":
							return -1;
						case "SHAMAN":
							return 2;
						case "DRUID":
							return 0;
						case "BARD":
							return 0;
						case "MAGICIAN":
							return 1;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return 0;
						case "ENCHANTER":
							return 1;
						case "BEASTLORD":
							return 2;
						case "BERSERKER":
							return 1;
						default:
							return 0;
					}
				case "insight":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 0;
						case "CLERIC":
							return 2;
						case "RANGER":
							return 0;
						case "ROGUE":
							return 0;
						case "WIZARD":
							return 2;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return 0;
						case "SHAMAN":
							return -1;
						case "DRUID":
							return 0;
						case "BARD":
							return -1;
						case "MAGICIAN":
							return 0;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return -1;
						case "ENCHANTER":
							return 1;
						case "BEASTLORD":
							return -1;
						case "BERSERKER":
							return 1;
						default:
							return 0;
					}
				case "medicine":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 0;
						case "CLERIC":
							return 2;
						case "RANGER":
							return 1;
						case "ROGUE":
							return -1;
						case "WIZARD":
							return -1;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return -1;
						case "SHAMAN":
							return 2;
						case "DRUID":
							return 1;
						case "BARD":
							return 0;
						case "MAGICIAN":
							return 1;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return -1;
						case "ENCHANTER":
							return -1;
						case "BEASTLORD":
							return 1;
						case "BERSERKER":
							return 1;
						default:
							return 0;
					}
				case "perception":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return 0;
						case "RANGER":
							return 2;
						case "ROGUE":
							return 1;
						case "WIZARD":
							return 0;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return 0;
						case "SHAMAN":
							return 1;
						case "DRUID":
							return 1;
						case "BARD":
							return 0;
						case "MAGICIAN":
							return 0;
						case "MONK":
							return 1;
						case "NECROMANCER":
							return 0;
						case "ENCHANTER":
							return 1;
						case "BEASTLORD":
							return 1;
						case "BERSERKER":
							return -1;
						default:
							return 0;
					}
				case "survival":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return 0;
						case "RANGER":
							return 2;
						case "ROGUE":
							return 1;
						case "WIZARD":
							return -1;
						case "PALADIN":
							return 0;
						case "SHADOWKNIGHT":
							return -1;
						case "SHAMAN":
							return 1;
						case "DRUID":
							return 1;
						case "BARD":
							return -1;
						case "MAGICIAN":
							return -2;
						case "MONK":
							return 1;
						case "NECROMANCER":
							return -1;
						case "ENCHANTER":
							return -1;
						case "BEASTLORD":
							return 2;
						case "BERSERKER":
							return 1;
						default:
							return 0;
					}
				case "deception":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return -2;
						case "RANGER":
							return 0;
						case "ROGUE":
							return 2;
						case "WIZARD":
							return -1;
						case "PALADIN":
							return -2;
						case "SHADOWKNIGHT":
							return 2;
						case "SHAMAN":
							return -1;
						case "DRUID":
							return -2;
						case "BARD":
							return 1;
						case "MAGICIAN":
							return 0;
						case "MONK":
							return -1;
						case "NECROMANCER":
							return 2;
						case "ENCHANTER":
							return 2;
						case "BEASTLORD":
							return -1;
						case "BERSERKER":
							return 1;
						default:
							return 0;
					}
				case "intimidation":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return -2;
						case "RANGER":
							return 0;
						case "ROGUE":
							return 1;
						case "WIZARD":
							return 2;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return 2;
						case "SHAMAN":
							return -2;
						case "DRUID":
							return -2;
						case "BARD":
							return -1;
						case "MAGICIAN":
							return 1;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return 2;
						case "ENCHANTER":
							return -1;
						case "BEASTLORD":
							return 0;
						case "BERSERKER":
							return 2;
						default:
							return 0;
					}
				case "performance":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return 0;
						case "RANGER":
							return -2;
						case "ROGUE":
							return 1;
						case "WIZARD":
							return 0;
						case "PALADIN":
							return 0;
						case "SHADOWKNIGHT":
							return 0;
						case "SHAMAN":
							return 0;
						case "DRUID":
							return -1;
						case "BARD":
							return 2;
						case "MAGICIAN":
							return 1;
						case "MONK":
							return 1;
						case "NECROMANCER":
							return 0;
						case "ENCHANTER":
							return 0;
						case "BEASTLORD":
							return -2;
						case "BERSERKER":
							return 1;
						default:
							return 0;
					}
				case "persuasion":
					switch (adventureClass.ToString().ToUpper())
					{
						case "WARRIOR":
							return 1;
						case "CLERIC":
							return 1;
						case "RANGER":
							return 0;
						case "ROGUE":
							return 1;
						case "WIZARD":
							return 1;
						case "PALADIN":
							return 1;
						case "SHADOWKNIGHT":
							return -1;
						case "SHAMAN":
							return -1;
						case "DRUID":
							return -1;
						case "BARD":
							return 1;
						case "MAGICIAN":
							return 0;
						case "MONK":
							return 0;
						case "NECROMANCER":
							return 0;
						case "ENCHANTER":
							return 2;
						case "BEASTLORD":
							return -1;
						case "BERSERKER":
							return 0;
						default:
							return 0;
					}
				default:
					return 0;
			}
		}
	}
}
