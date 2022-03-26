using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorWeapon : EntityBehavior
    {
        protected long abilityId;

        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorWeapon"; }

        public EntityBehaviorWeapon(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }

        internal float CalculateWeaponDamage(EnumDamageType damageType, int baseDamage, EntityAgent target)
        {
			DamageHitInfo my_hit = new DamageHitInfo();
			my_hit.base_damage = baseDamage;

			my_hit.min_damage = 0;
			my_hit.damage_done = 1;

			int chance_mod = 0;

			var ranged = false;

			my_hit.skill = SkillUtils.GetSkillTypeFromDamageType((ExtendedEnumDamageType)damageType, ranged);
			my_hit.offense = Offense(my_hit.skill);
			my_hit.tohit = GetTotalToHit(my_hit.skill, chance_mod);
			//my_hit.hand = ranged;

			var damageHitInfo = DoAttack(target, my_hit);
            return damageHitInfo.damage_done;
        }

        private int GetTotalToHit(SkillType skillType, int hitChanceBonus)
        {
			if (hitChanceBonus >= 10000) // override for stuff like SE_SkillAttack
				return -1;

			// calculate attacker's accuracy
			double accuracy = ComputeToHit(skillType) + 10;
			if (hitChanceBonus > 0) // multiplier
				accuracy *= hitChanceBonus;

			accuracy = (accuracy * 121) / 100;

			if (skillType != SkillType.Archery && skillType != SkillType.Throwing)
			{
				//accuracy += GetItemBonuses(SpellEffectType.HitChance);
			}

			// TODO
			double accuracySkill = 0;
			accuracy += accuracySkill;

			// TODO
			double buffItemAndAABonus = 0;
			//buffItemAndAABonus += getSpellBonuses(SpellEffectType.HitChance);

			accuracy = (accuracy * (100 + buffItemAndAABonus)) / 100;
			return (int)Math.Floor(accuracy);
		}

        private int ComputeToHit(SkillType skillType)
        {
			double tohit = entity.GetSkill(SkillType.Offense) + 7;
			tohit += entity.GetSkill(skillType);
			if (!entity.IsIServerPlayer())
			{
				tohit += GetNPCDefaultAccuracyRating();
			}
			/*
			if (entity.IsIServerPlayer())
			{
				double reduction = getIntoxication() / 2.0;
				if (reduction > 20.0)
				{
					reduction = Math.min((110 - reduction) / 100.0, 1.0);
					tohit = reduction * (double)(tohit);
				}
				else if (isBerserk())
				{
					tohit += (getMentorLevel() * 2) / 5;
				}
			}
			*/

			return (int)Math.Max(tohit, 1);
		}

        private double GetNPCDefaultAccuracyRating()
        {
			return (int)Math.Ceiling((double)(entity.GetLevel() / 7) * 10);
		}

        private int Offense(SkillType skillType)
        {
			int offense = entity.GetSkill(skillType);
			int stat_bonus = entity.GetStatistic(StatType.Strength);
			if (skillType == SkillType.Archery || skillType == SkillType.Throwing)
			{
				stat_bonus = entity.GetStatistic(StatType.Dexterity);
			}

			if (stat_bonus >= 75)
			{
				offense += (2 * stat_bonus - 150) / 3;
			}

			int attk = GetAtk();
			offense += attk;
			return offense;
		}

		public int GetAtk()
		{
			// this should really only be happening for npcs
			int attackItemBonuses = 0;
			// todo, item bonuses

			int attackSpellBonsues = 0;

			//attackSpellBonsues += getSpellBonuses(SpellEffectType.ATK);

			int ATK = 0;
			// npc
			if (!this.entity.IsIServerPlayer())
			{
				ATK = GetNPCDefaultAtk();
			}
			// this is from the bot code..
			return ATK + attackItemBonuses + attackSpellBonsues; // todo AA bonuses;
		}

        private int GetNPCDefaultAtk()
        {
			return (int)Math.Ceiling((decimal)(entity.GetLevel() / 7) * 10);
		}

        private DamageHitInfo DoAttack(EntityAgent target, DamageHitInfo hit)
		{
			if (target == null || !target.Alive)
				return hit;

			int originalDamage = hit.damage_done;

			if (hit.damage_done >= 0)
			{
				if (target.CheckHitChance(hit))
				{
					hit = target.MeleeMitigation(this.entity, hit);
					if (hit.damage_done > 0)
					{
						hit = ApplyDamageTable(hit);
						hit = CommonOutgoingHitSuccess(target, hit);
					}
				}
				else
				{
					// missed
					hit.damage_done = 0;
				}
			}

			return hit;
		}

		private DamageHitInfo CommonOutgoingHitSuccess(Entity defender, DamageHitInfo hit)
		{
			if (defender == null)
				return hit;

			if (hit.skill == SkillType.Archery 
				// todo berserker
				//|| (hit.skill == SkillType.Throwing && getClassObj() != null && getClassObj().getName().equals("BERSERKER"))
					)
				hit.damage_done /= 2;

			if (hit.damage_done < 1)
				hit.damage_done = 1;

			int extra_mincap = 0;
			int min_mod = hit.base_damage * GetMeleeMinDamageMod_SE(hit.skill) / 100;
			
			// this has some weird ordering
			// Seems the crit message is generated before some of them :P

			// worn item +skill dmg, SPA 220, 418. Live has a normalized version that should be here too
			hit.min_damage += GetSkillDmgAmt(hit.skill);

			hit.damage_done = ApplyMeleeDamageMods(hit.skill, hit.damage_done, defender);
			min_mod = Math.Max(min_mod, extra_mincap);
			if (min_mod > 0 && hit.damage_done < min_mod) // SPA 186
				hit.damage_done = min_mod;

			hit = TryCriticalHit(defender, hit);

			hit.damage_done += hit.min_damage;

			// this appears where they do special attack dmg mods
			int spec_mod = 0;

			// TODO RAMPAGE

			if (spec_mod > 0)
				hit.damage_done = (hit.damage_done * spec_mod) / 100;

			hit.damage_done += (hit.damage_done * defender.GetSkillDmgTaken(hit.skill) / 100)
					+ (defender.GetFcDamageAmtIncoming(0, true, hit.skill));

			//CheckNumHitsRemaining(NumHit.OutgoingHitSuccess);
			return hit;
		}

		public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
		{
			base.OnEntityReceiveDamage(damageSource, ref damage);

			var sourceName = "unknown";
			if (damageSource.SourceEntity != null)
				sourceName = damageSource.SourceEntity?.GetName();
			if (damageSource.SourceEntity is EntityPlayer)
				sourceName = damageSource.SourceEntity.GetBehavior<EntityBehaviorNameTag>().DisplayName;

			var targetName = this.entity.GetName();
			if (this.entity is EntityPlayer)
				targetName = this.entity.GetBehavior<EntityBehaviorNameTag>().DisplayName;

			// Allow receiving skill xp for defense if being damaged
			if (this.entity is EntityPlayer && damageSource.SourceEntity is EntityPlayer)
				this.entity.GetAsIServerPlayer().SendMessage(GlobalConstants.DamageLogChatGroup, sourceName + " hit " + targetName + " for " + damage + " points of " + damageSource.Type.ToString().ToLower() + " damage (" + this.entity.GetHealth() + ")", EnumChatType.CommandSuccess);

			// Allow receiving skill xp for players attacking and source is the actual player not a ranged entity
			if (damageSource.Source == EnumDamageSource.Player && damageSource.SourceEntity != null && damageSource.SourceEntity is EntityPlayer && damageSource.SourceEntity.Alive)
				damageSource.SourceEntity.GetAsIServerPlayer().SendMessage(GlobalConstants.DamageLogChatGroup, sourceName + " hit " + targetName + " for " + damage + " points of " + damageSource.Type.ToString().ToLower() + " damage ("+this.entity.GetHealth()+")", EnumChatType.CommandSuccess);
		}

		private DamageHitInfo TryCriticalHit(Entity defender, DamageHitInfo hit)
        {
			if (defender == null)
				return hit;

			if (hit.damage_done < 1)
				return hit;

			
			// 2: Try Melee Critical

			bool innateCritical = false;
			int critChance = SkillUtils.GetCriticalChanceBonus(entity, hit.skill);
			if (entity.GetLevel(AdventureClass.Warrior) >= 12 || entity.GetLevel(AdventureClass.Berserker) >= 12)
				innateCritical = true;
			else if (entity.GetLevel(AdventureClass.Ranger) >= 12 && hit.skill == SkillType.Archery)
				innateCritical = true;
			else if (entity.GetLevel(AdventureClass.Rogue) >= 12 && hit.skill == SkillType.Throwing)
				innateCritical = true;

			// we have a chance to crit!
			if (innateCritical || critChance > 0)
			{
				int difficulty = 0;
				if (hit.skill == SkillType.Archery)
					difficulty = 3400;
				else if (hit.skill == SkillType.Throwing)
					difficulty = 1100;
				else
					difficulty = 8900;

				// attacker.sendMessage("You have a chance to cause a critical (Diffulty dice
				// roll: " + difficulty);
				int roll = MathUtils.RandomBetween(1, difficulty);
				// attacker.sendMessage("Critical chance roll ended up as: " + roll);

				int dex_bonus = entity.GetStatistic(StatType.Dexterity);
				if (dex_bonus > 255)
					dex_bonus = 255 + ((dex_bonus - 255) / 5);
				dex_bonus += 45;
				// attacker.sendMessage("Critical dex bonus was: " + dex_bonus);

				// so if we have an innate crit we have a better chance, except for ber throwing
				if (!innateCritical)// || (className.equals("BERSERKER") && hit.skill.equals(SkillType.Throwing)))
					dex_bonus = dex_bonus * 3 / 5;

				if (critChance > 0)
					dex_bonus += dex_bonus * critChance / 100;

				// attacker.sendMessage("Checking if your roll: " + roll + " is less than the
				// dexbonus: " + dex_bonus);

				// check if we crited
				if (roll < dex_bonus)
				{

					// TODO: Finishing Blow

					// step 2: calculate damage
					hit.damage_done = Math.Max(hit.damage_done, hit.base_damage) + 5;
					// attacker.sendMessage("Taking the maximum out of damageDone: " + damageDone +
					// " vs baseDamage: " + baseDamage + " adding 5 to it");

					double og_damage = hit.damage_done;
					int crit_mod = 170 + SkillUtils.GetCritDmgMod(hit.skill);
					if (crit_mod < 100)
					{
						crit_mod = 100;
					}
					// attacker.sendMessage("Crit mod was: " + crit_mod);

					hit.damage_done = hit.damage_done * crit_mod / 100;
					// attacker.sendMessage("DamageDone was calculated at: " + damageDone);

					// TODO Spell bonuses && holyforge
					double totalCritBonus = (hit.damage_done - hit.base_damage);

					if (entity.IsIServerPlayer())
						entity.GetAsIServerPlayer().SendMessage(GlobalConstants.DamageLogChatGroup, "* Your score a critical hit (" + totalCritBonus + ")!", EnumChatType.CommandSuccess);
					return hit;
				}

			}

			return hit;
		}

        private int GetMeleeMinDamageMod_SE(SkillType skill)
        {
			int dmg_mod = 0;

			/*
			// Needs to pass skill as parameter
			dmg_mod = getItemBonuses(SpellEffectType.MinDamageModifier, skillType) +
			getSpellBonuses(SpellEffectType.MinDamageModifier, skillType) //+
			// itembonuses.MinDamageModifier[EQEmu::skills::HIGHEST_SKILL + 1] +
			// spellbonuses.MinDamageModifier[EQEmu::skills::HIGHEST_SKILL + 1]
					;
			 */
			if (dmg_mod < -100)
				dmg_mod = -100;

			return dmg_mod;
		}

        private int ApplyMeleeDamageMods(SkillType skillType, int damage_done, Entity defender)
        {
			int dmgbonusmod = 0;

			dmgbonusmod += GetMeleeDamageMod_SE(skillType);

			if (defender != null)
			{
				if (defender.IsIServerPlayer() && defender.GetLevel(AdventureClass.Warrior) > 1)
					dmgbonusmod -= 5;
				// 168 defensive MeleeMitigation
				//dmgbonusmod += (defender.getSpellBonuses(SpellEffectType.MeleeMitigation) +
				//getItemBonuses(SpellEffectType.MeleeMitigation) + getAABonuses(SpellEffectType.MeleeMitigation));
			}

			damage_done += damage_done * dmgbonusmod / 100;

			return damage_done;
		}

        private int GetMeleeDamageMod_SE(SkillType skillType)
        {
			int dmg_mod = 0;

			//int spellDamageModifier = getSpellBonuses(SpellEffectType.DamageModifier);
			//int aaDamageModifier = getAABonuses(SpellEffectType.DamageModifier);

			//dmg_mod += spellDamageModifier;
			//dmg_mod += aaDamageModifier;

			if (dmg_mod < -100)
				dmg_mod = -100;

			return dmg_mod;
		}

        private int GetSkillDmgAmt(SkillType skill)
        {
			int skill_dmg = 0;
			return skill_dmg;
		}

        private DamageHitInfo ApplyDamageTable(DamageHitInfo hit)
        {
			// someone may want to add this to custom servers, can remove this if that's the case
			if (!entity.IsIServerPlayer())
				return hit;

			if (hit.offense < 115)
				return hit;

			if (hit.damage_done < 2)
				return hit;

			var highestWarriorClassLevel = entity.GetHighestWarriorClassLevel();
			var highestClassLevel = entity.GetHighestLevel();
			var monkLevel = entity.GetLevel(AdventureClass.Monk);

			// 0 = max_extra
			// 1 = chance
			// 2 = minusfactor
			int[] damage_table = GetDamageTable(highestClassLevel, highestWarriorClassLevel, monkLevel);
			if (MathUtils.Roll(damage_table[1]))
				return hit;

			int basebonus = hit.offense - damage_table[2];

			basebonus = Math.Max(10, basebonus / 2);
			int extrapercent = MathUtils.RandomBetween(0, basebonus);
			int percent = Math.Min(100 + extrapercent, damage_table[0]);
			hit.damage_done = (hit.damage_done * percent) / 100;
			if (highestWarriorClassLevel > 54)
				hit.damage_done++;

			return hit;
		}

        private int[] GetDamageTable(int highestLevel, int highestWarriorClassLevel, int monkLevel = 0)
        {
			var dmg_table = new List<int[]> {
				new int[] { 210, 49, 105 }, // 1-50
				new int[] { 245, 35, 80 }, // 51
				new int[] { 245, 35, 80 }, // 52
				new int[] { 245, 35, 80 }, // 53
				new int[] { 245, 35, 80 }, // 54
				new int[] { 245, 35, 80 }, // 55
				new int[] { 265, 28, 70 }, // 56
				new int[] { 265, 28, 70 }, // 57
				new int[] { 265, 28, 70 }, // 58
				new int[] { 265, 28, 70 }, // 59
				new int[] { 285, 23, 65 }, // 60
				new int[] { 285, 23, 65 }, // 61
				new int[] { 285, 23, 65 }, // 62
				new int[] { 290, 21, 60 }, // 63
				new int[] { 290, 21, 60 }, // 64
				new int[] { 295, 19, 55 }, // 65
				new int[] { 295, 19, 55 }, // 66
				new int[] { 300, 19, 55 }, // 67
				new int[] { 300, 19, 55 }, // 68
				new int[] { 300, 19, 55 }, // 69
				new int[] { 305, 19, 55 }, // 70
				new int[] { 305, 19, 55 }, // 71
				new int[] { 310, 17, 50 }, // 72
				new int[] { 310, 17, 50 }, // 73
				new int[] { 310, 17, 50 }, // 74
				new int[] { 315, 17, 50 }, // 75
				new int[] { 315, 17, 50 }, // 76
				new int[] { 325, 17, 45 }, // 77
				new int[] { 325, 17, 45 }, // 78
				new int[] { 325, 17, 45 }, // 79
				new int[] { 335, 17, 45 }, // 80
				new int[] { 335, 17, 45 }, // 81
				new int[] { 345, 17, 45 }, // 82
				new int[] { 345, 17, 45 }, // 83
				new int[] { 345, 17, 45 }, // 84
				new int[] { 355, 17, 45 }, // 85
				new int[] { 355, 17, 45 }, // 86
				new int[] { 365, 17, 45 }, // 87
				new int[] { 365, 17, 45 }, // 88
				new int[] { 365, 17, 45 }, // 89
				new int[] { 375, 17, 45 }, // 90
				new int[] { 375, 17, 45 }, // 91
				new int[] { 380, 17, 45 }, // 92
				new int[] { 380, 17, 45 }, // 93
				new int[] { 380, 17, 45 }, // 94
				new int[] { 385, 17, 45 }, // 95
				new int[] { 385, 17, 45 }, // 96
				new int[] { 390, 17, 45 }, // 97
				new int[] { 390, 17, 45 }, // 98
				new int[] { 390, 17, 45 }, // 99
				new int[] { 395, 17, 45 }, // 100
				new int[] { 395, 17, 45 }, // 101
				new int[] { 400, 17, 45 }, // 102
				new int[] { 400, 17, 45 }, // 103
				new int[] { 400, 17, 45 }, // 104
				new int[] { 405, 17, 45 } // 105
			}.ToArray();

			var mnk_table = new List<int[]> {
				new int[] { 220, 45, 100 }, // 1-50
				new int[] { 245, 35, 80 }, // 51
				new int[] { 245, 35, 80 }, // 52
				new int[] { 245, 35, 80 }, // 53
				new int[] { 245, 35, 80 }, // 54
				new int[] { 245, 35, 80 }, // 55
				new int[] { 285, 23, 65 }, // 56
				new int[] { 285, 23, 65 }, // 57
				new int[] { 285, 23, 65 }, // 58
				new int[] { 285, 23, 65 }, // 59
				new int[] { 290, 21, 60 }, // 60
				new int[] { 290, 21, 60 }, // 61
				new int[] { 290, 21, 60 }, // 62
				new int[] { 295, 19, 55 }, // 63
				new int[] { 295, 19, 55 }, // 64
				new int[] { 300, 17, 50 }, // 65
				new int[] { 300, 17, 50 }, // 66
				new int[] { 310, 17, 50 }, // 67
				new int[] { 310, 17, 50 }, // 68
				new int[] { 310, 17, 50 }, // 69
				new int[] { 320, 17, 50 }, // 70
				new int[] { 320, 17, 50 }, // 71
				new int[] { 325, 15, 45 }, // 72
				new int[] { 325, 15, 45 }, // 73
				new int[] { 325, 15, 45 }, // 74
				new int[] { 330, 15, 45 }, // 75
				new int[] { 330, 15, 45 }, // 76
				new int[] { 335, 15, 40 }, // 77
				new int[] { 335, 15, 40 }, // 78
				new int[] { 335, 15, 40 }, // 79
				new int[] { 345, 15, 40 }, // 80
				new int[] { 345, 15, 40 }, // 81
				new int[] { 355, 15, 40 }, // 82
				new int[] { 355, 15, 40 }, // 83
				new int[] { 355, 15, 40 }, // 84
				new int[] { 365, 15, 40 }, // 85
				new int[] { 365, 15, 40 }, // 86
				new int[] { 375, 15, 40 }, // 87
				new int[] { 375, 15, 40 }, // 88
				new int[] { 375, 15, 40 }, // 89
				new int[] { 385, 15, 40 }, // 90
				new int[] { 385, 15, 40 }, // 91
				new int[] { 390, 15, 40 }, // 92
				new int[] { 390, 15, 40 }, // 93
				new int[] { 390, 15, 40 }, // 94
				new int[] { 395, 15, 40 }, // 95
				new int[] { 395, 15, 40 }, // 96
				new int[] { 400, 15, 40 }, // 97
				new int[] { 400, 15, 40 }, // 98
				new int[] { 400, 15, 40 }, // 99
				new int[] { 405, 15, 40 }, // 100
				new int[] { 405, 15, 40 }, // 101
				new int[] { 410, 15, 40 }, // 102
				new int[] { 410, 15, 40 }, // 103
				new int[] { 410, 15, 40 }, // 104
				new int[] { 415, 15, 40 }, // 105
			}.ToArray();

			bool melee = highestWarriorClassLevel > 1;
			bool monk = monkLevel >= highestWarriorClassLevel;

			// tables caped at 105 for now -- future proofed for a while at least :P

			int level = Math.Min(highestWarriorClassLevel, WorldLimits.MAX_LEVEL);

			if (!melee || (!monk && highestLevel < 51))
				return dmg_table[0];

			if (monk && monkLevel < 51)
				return mnk_table[0];

			int[][] which = monk ? mnk_table : dmg_table;
			return which[level - 50];
		}
    }
}
