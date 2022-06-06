using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Extensions
{
    public static class EntityExt
    {
        public static bool IsIServerPlayer(this Entity me)
        {
            if (!(me is EntityPlayer))
                return false;

            if (((EntityPlayer)me).Player == null)
                return false;

            if (!(((EntityPlayer)me).Player is IServerPlayer))
                return false;

            return true;
        }

        public static void ResetHasteRunspeedState(this Entity me)
        {
            EntityBehaviorMoveSpeedAdjustable ebs = me.GetBehavior("EntityBehaviorMoveSpeedAdjustable") as EntityBehaviorMoveSpeedAdjustable;
            if (ebs != null)
                ebs.ResetHasteRunspeedState();
        }

        public static int GetHighestWarriorClassLevel(this Entity me)
        {
            var highest = 0;

            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var level = me.GetLevel(adventureClass);
                if (level > highest)
                    highest = level;
            }
            return highest;
        }

        public static int GetMitigationAC(this Entity me)
        {
            return me.ACSum();
        }

        public static int ACSum(this Entity me)
        {
            double ac = 0;
            // nm everything gets it
            // players and core pets get AC from Items they are wearing
            //if (this.isPlayer() || IsCorePet())
            
            //ac += getTotalItemAC();

            double shield_ac = 0;

            int highestLevel = me.GetHighestLevel();

            // EQ math
            ac = (ac * 4) / 3;
            // anti-twink
            if (me.IsIServerPlayer() && highestLevel < 50)
                ac = Math.Min(ac, 25 + 6 * highestLevel);

            //ac = Math.Max(0, ac + getClassRaceACBonus());

            if (!me.IsIServerPlayer())
            {

                ac += 10;
                //ac += getDefaultNpcAC();

                // TODO Pet avoidance
                ac += me.GetSkill(SkillType.Defense) / 5;

                double spell_aa_ac = 0;
                // TODO AC AA and Spell bonuses

                //spell_aa_ac += getSpellBonuses(SpellEffectType.ArmorClass);

                //spell_aa_ac += getAABonuses(SpellEffectType.ArmorClass);

                // enchanter lesser ac bonus here
                // ac += spell_aa_ac / 3;
                ac += spell_aa_ac / 4;

            }
            else
            {
                double spell_aa_ac = 0;
                // TODO AC AA and Spell bonuses
                //spell_aa_ac += getSpellBonuses(SpellEffectType.ArmorClass);

                //spell_aa_ac += getAABonuses(SpellEffectType.ArmorClass);

                // enchanter lesser ac bonus here
                //ac += getSkill(SkillType.Defense) / 2 + spell_aa_ac / 3;
                ac += me.GetSkill(SkillType.Defense) / 3 + spell_aa_ac / 4;
            }

            if (me.GetStatistic(StatType.Agility) > 70)
                ac += me.GetStatistic(StatType.Agility) / 20;
            if (ac < 0)
                ac = 0;

            if (me.IsIServerPlayer())
            {
                double softcap = me.GetHighestACSoftcap();
                double returns = me.GetHighestSoftcapReturns();

                // TODO itembonuses

                int total_aclimitmod = 0;

                //total_aclimitmod += getSpellBonuses(SpellEffectType.CombatStability);

                //total_aclimitmod += getAABonuses(SpellEffectType.CombatStability);

                if (total_aclimitmod > 0)
                    softcap = (softcap * (100 + total_aclimitmod)) / 100;
                softcap += shield_ac;

                if (ac > softcap)
                {
                    double over_cap = ac - softcap;
                    ac = softcap + (over_cap * returns);
                }
            }

            return (int)ac;
        }

        public static int GetSkillDmgTaken(this Entity me, SkillType skillType)
        {
            int skilldmg_mod = 0;

            if (skilldmg_mod < -100)
                skilldmg_mod = -100;

            return skilldmg_mod;
        }

        public static int GetFcDamageAmtIncoming(this Entity me, int spell_id, bool use_skill, SkillType skillType)
        {
            // Used to check focus derived from SE_FcDamageAmtIncoming which adds direct
            // damage to Spells or Skill based attacks.
            int dmg = 0;
            return dmg;
        }

        private static double GetHighestSoftcapReturns(this Entity me)
        {
            double highest = 0;
            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var softCap = me.GetSoftcapReturns(adventureClass);
                if (softCap > highest)
                    highest = softCap;
            }

            return highest;
        }

        private static double GetSoftcapReturns(this Entity me, AdventureClass adventureClass)
        {
            if (adventureClass == AdventureClass.None)
                return 0.3;

            switch (adventureClass)
            {
                case AdventureClass.Warrior:
                    return 0.35;
                case AdventureClass.Cleric:
                case AdventureClass.Bard:
                case AdventureClass.Monk:
                    return 0.3;
                case AdventureClass.Paladin:
                case AdventureClass.Shadowknight:
                    return 0.33;
                case AdventureClass.Ranger:
                    return 0.315;
                case AdventureClass.Druid:
                    return 0.265;
                case AdventureClass.Rogue:
                case AdventureClass.Shaman:
                case AdventureClass.Beastlord:
                case AdventureClass.Berserker:
                    return 0.28;
                case AdventureClass.Necromancer:
                case AdventureClass.Wizard:
                case AdventureClass.Magician:
                case AdventureClass.Enchanter:
                    return 0.25;
                default:
                    return 0.3;
            }
        }

        private static int GetHighestACSoftcap(this Entity me)
        {
            int highest = 0;
            foreach(AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var acSoftCap = me.GetACSoftcap(adventureClass);
                if (acSoftCap > highest)
                    highest = acSoftCap;
            }

            return highest;
        }

        private static int GetACSoftcap(this Entity me, AdventureClass adventureClass)
        {
            if (adventureClass == AdventureClass.None)
                return 350;

            int[] war_softcaps = { 312, 314, 316, 318, 320, 322, 324, 326, 328, 330, 332, 334, 336, 338, 340, 342, 344, 346,
                348, 350, 352, 354, 356, 358, 360, 362, 364, 366, 368, 370, 372, 374, 376, 378, 380, 382, 384, 386, 388,
                390, 392, 394, 396, 398, 400, 402, 404, 406, 408, 410, 412, 414, 416, 418, 420, 422, 424, 426, 428, 430,
                432, 434, 436, 438, 440, 442, 444, 446, 448, 450, 452, 454, 456, 458, 460, 462, 464, 466, 468, 470, 472,
                474, 476, 478, 480, 482, 484, 486, 488, 490, 492, 494, 496, 498, 500, 502, 504, 506, 508, 510, 512, 514,
                516, 518, 520 };

            int[] clrbrdmnk_softcaps = { 274, 276, 278, 278, 280, 282, 284, 286, 288, 290, 292, 292, 294, 296, 298, 300,
                302, 304, 306, 308, 308, 310, 312, 314, 316, 318, 320, 322, 322, 324, 326, 328, 330, 332, 334, 336, 336,
                338, 340, 342, 344, 346, 348, 350, 352, 352, 354, 356, 358, 360, 362, 364, 366, 366, 368, 370, 372, 374,
                376, 378, 380, 380, 382, 384, 386, 388, 390, 392, 394, 396, 396, 398, 400, 402, 404, 406, 408, 410, 410,
                412, 414, 416, 418, 420, 422, 424, 424, 426, 428, 430, 432, 434, 436, 438, 440, 440, 442, 444, 446, 448,
                450, 452, 454, 454, 456 };

            int[] palshd_softcaps = { 298, 300, 302, 304, 306, 308, 310, 312, 314, 316, 318, 320, 322, 324, 326, 328, 330,
                332, 334, 336, 336, 338, 340, 342, 344, 346, 348, 350, 352, 354, 356, 358, 360, 362, 364, 366, 368, 370,
                372, 374, 376, 378, 380, 382, 384, 384, 386, 388, 390, 392, 394, 396, 398, 400, 402, 404, 406, 408, 410,
                412, 414, 416, 418, 420, 422, 424, 426, 428, 430, 432, 432, 434, 436, 438, 440, 442, 444, 446, 448, 450,
                452, 454, 456, 458, 460, 462, 464, 466, 468, 470, 472, 474, 476, 478, 480, 480, 482, 484, 486, 488, 490,
                492, 494, 496, 498 };

            int[] rng_softcaps = { 286, 288, 290, 292, 294, 296, 298, 298, 300, 302, 304, 306, 308, 310, 312, 314, 316, 318,
                320, 322, 322, 324, 326, 328, 330, 332, 334, 336, 338, 340, 342, 344, 344, 346, 348, 350, 352, 354, 356,
                358, 360, 362, 364, 366, 368, 368, 370, 372, 374, 376, 378, 380, 382, 384, 386, 388, 390, 390, 392, 394,
                396, 398, 400, 402, 404, 406, 408, 410, 412, 414, 414, 416, 418, 420, 422, 424, 426, 428, 430, 432, 434,
                436, 436, 438, 440, 442, 444, 446, 448, 450, 452, 454, 456, 458, 460, 460, 462, 464, 466, 468, 470, 472,
                474, 476, 478 };

            int[] dru_softcaps = { 254, 256, 258, 260, 262, 264, 264, 266, 268, 270, 272, 272, 274, 276, 278, 280, 282, 282,
                284, 286, 288, 290, 290, 292, 294, 296, 298, 300, 300, 302, 304, 306, 308, 308, 310, 312, 314, 316, 318,
                318, 320, 322, 324, 326, 328, 328, 330, 332, 334, 336, 336, 338, 340, 342, 344, 346, 346, 348, 350, 352,
                354, 354, 356, 358, 360, 362, 364, 364, 366, 368, 370, 372, 372, 374, 376, 378, 380, 382, 382, 384, 386,
                388, 390, 390, 392, 394, 396, 398, 400, 400, 402, 404, 406, 408, 410, 410, 412, 414, 416, 418, 418, 420,
                422, 424, 426 };

            int[] rogshmbstber_softcaps = { 264, 266, 268, 270, 272, 272, 274, 276, 278, 280, 282, 282, 284, 286, 288, 290,
                292, 294, 294, 296, 298, 300, 302, 304, 306, 306, 308, 310, 312, 314, 316, 316, 318, 320, 322, 324, 326,
                328, 328, 330, 332, 334, 336, 338, 340, 340, 342, 344, 346, 348, 350, 350, 352, 354, 356, 358, 360, 362,
                362, 364, 366, 368, 370, 372, 374, 374, 376, 378, 380, 382, 384, 384, 386, 388, 390, 392, 394, 396, 396,
                398, 400, 402, 404, 406, 408, 408, 410, 412, 414, 416, 418, 418, 420, 422, 424, 426, 428, 430, 430, 432,
                434, 436, 438, 440, 442 };

            int[] necwizmagenc_softcaps = { 248, 250, 252, 254, 256, 256, 258, 260, 262, 264, 264, 266, 268, 270, 272, 272,
                274, 276, 278, 280, 280, 282, 284, 286, 288, 288, 290, 292, 294, 296, 296, 298, 300, 302, 304, 304, 306,
                308, 310, 312, 312, 314, 316, 318, 320, 320, 322, 324, 326, 328, 328, 330, 332, 334, 336, 336, 338, 340,
                342, 344, 344, 346, 348, 350, 352, 352, 354, 356, 358, 360, 360, 362, 364, 366, 368, 368, 370, 372, 374,
                376, 376, 378, 380, 382, 384, 384, 386, 388, 390, 392, 392, 394, 396, 398, 400, 400, 402, 404, 406, 408,
                408, 410, 412, 414, 416 };

            int level = Math.Min(105, me.GetLevel(adventureClass)) - 1;

            switch (adventureClass)
            {
                case AdventureClass.Warrior:
                    return war_softcaps[level];
                case AdventureClass.Cleric:
                case AdventureClass.Bard:
                case AdventureClass.Monk:
                    return clrbrdmnk_softcaps[level];
                case AdventureClass.Paladin:
                case AdventureClass.Shadowknight:
                    return palshd_softcaps[level];
                case AdventureClass.Ranger:
                    return rng_softcaps[level];
                case AdventureClass.Druid:
                    return dru_softcaps[level];
                case AdventureClass.Rogue:
                case AdventureClass.Shaman:
                case AdventureClass.Beastlord:
                case AdventureClass.Berserker:
                    return rogshmbstber_softcaps[level];
                case AdventureClass.Necromancer:
                case AdventureClass.Wizard:
                case AdventureClass.Magician:
                case AdventureClass.Enchanter:
                    return necwizmagenc_softcaps[level];
                default:
                    return 350;
            }
        }

        public static DamageHitInfo MeleeMitigation(this Entity me, Entity attacker, DamageHitInfo hit)
        {
            if (hit.damage_done < 0 || hit.base_damage == 0)
                return hit;

            int mitigation = me.GetMitigationAC();

            if (me.IsIServerPlayer() && attacker.IsIServerPlayer())
            {
                mitigation = mitigation * 80 / 100; // PvP
            }

            int roll = (int)me.RollD20(hit.offense, mitigation);

            // +0.5 for rounding, min to 1 dmg
            hit.damage_done = Math.Max((int)(roll * (double)(hit.base_damage) + 0.5), 1);
            return hit;
        }

        public static double RollD20(this Entity me, int offense, int mitigation)
        {
            var mods = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9,
                2.0 };

            // always meditating, ignore this
            // if (isPlayer() && isMeditating())
            // return mods[19];

            if (offense < 1)
                offense = 1;

            if (mitigation < 1)
                mitigation = 1;

            int atk_roll = MathUtils.RandomBetween(0, offense + 5);
            int def_roll = MathUtils.RandomBetween(0, mitigation + 5);

            int avg = (offense + mitigation + 10) / 2;
            int index = Math.Max(0, (atk_roll - def_roll) + (avg / 2));

            index = (int)MathUtils.Clamp((index * 20) / avg, 0, 19);

            return mods[index];
        }

        public static bool CheckHitChance(this Entity me, DamageHitInfo hit)
        {
            int avoidance = me.GetTotalDefense();
            int accuracy = hit.tohit;

            Random rand = new Random();

            double hitRoll = MathUtils.RandomBetween(0, (int)Math.Floor((decimal)accuracy));
            double avoidRoll = MathUtils.RandomBetween(0, (int)Math.Floor((decimal)avoidance));

            // tie breaker? Don't want to be biased any one way
            return hitRoll > avoidRoll;
        }

        public static int GetTotalDefense(this Entity me)
        {
            return me.GetSkill(SkillType.Defense);
        }

        public static void TryFinishCast(this Entity me, bool forceSelf = false)
        {
            if (me.Api.Side != EnumAppSide.Server)
                return;

            EntityBehaviorCasting ebt = me.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return;

            ebt.TryFinishCast(forceSelf);
        }

        public static void ClearCasting(this Entity me)
        {
            if (me == null)
                return;

            EntityBehaviorCasting ebt = me.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return;

            ebt.ClearCasting();
        }

        public static void Interupt(this Entity me)
        {
            if (me == null)
                return;

            if (!me.IsDoingSomethingInteruptable())
                return;

            me.ClearCasting();

            if (me.IsIServerPlayer())
                me.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup,"You have been interupted", EnumChatType.CommandSuccess);
        }

        private static bool IsDoingSomethingInteruptable(this Entity me)
        {
            return false;
        }

        public static bool HasCastingSpellReady(this Entity me)
        {
            return me.CurrentAbilityId() > 0;
        }

        public static long CurrentAbilityId(this Entity me)
        {
            return (long)me.WatchedAttributes.GetLong("currentAbilityId");
        }

        public static int GetLevel(this Entity me, AdventureClass adventureClass)
        {
            if (me == null)
                return 1;

            return me.WatchedAttributes.GetInt(adventureClass.ToString().ToLower() + "level", 1);
        }

        public static bool IsInvulerable(this Entity me)
        {
            if ((!me.Alive || me.IsActivityRunning("invulnerable")))
                return true;

            return false;
        }
        public static ActiveSpellEffect GetActiveEffectOfType(this Entity me, SpellEffectIndex effectIndex, SpellEffectType type)
        {
            EntityBehaviorSpellEffects ebt = me.GetBehavior("EntityBehaviorSpellEffects") as EntityBehaviorSpellEffects;
            if (ebt == null)
                return null;

            return ebt.GetActiveEffectOfType(effectIndex, type);
        }

        public static int GetEffectStatisticBuff(this Entity me, StatType type)
        {
            var activeEffect = me.GetActiveEffectOfType(SpellEffectIndex.Stat_Buff, GetEffectTypeFromStatType(type));
            if (activeEffect == null)
                return 0;

            var ability = AbilityTools.GetAbility(me.World, activeEffect.AbilityId);

            var amount = ability.GetAmount();
            if (amount > int.MaxValue)
                amount = int.MaxValue;

            if (amount > WorldLimits.MAX_STATISTIC)
                return WorldLimits.MAX_STATISTIC;

            return (int)amount;
        }

        private static SpellEffectType GetEffectTypeFromStatType(StatType type)
        {
            switch(type)
            {
                case StatType.Strength:
                    return SpellEffectType.STR;
                case StatType.Stamina:
                    return SpellEffectType.STA;
                case StatType.Dexterity:
                    return SpellEffectType.DEX;
                case StatType.Agility:
                    return SpellEffectType.AGI;
                case StatType.Intelligence:
                    return SpellEffectType.INT;
                case StatType.Wisdom:
                    return SpellEffectType.WIS;
                case StatType.Charisma:
                    return SpellEffectType.CHA;
                default:
                    return SpellEffectType.None;
            }
        }

        public static bool QueueTickEffect(this Entity me, Entity source, Ability ability, long duration)
        {
            EntityBehaviorSpellEffects ebt = me.GetBehavior("EntityBehaviorSpellEffects") as EntityBehaviorSpellEffects;
            if (ebt == null)
                return false;
            return ebt.QueueTickEffect(source.EntityId, ability.Id, duration);
        }

        public static void ResetMana(this Entity me)
        {
            me.SetMana(me.GetMaxMana());
        }

        public static float GetMaxMana(this Entity me)
        {
            if (me.WatchedAttributes.TryGetFloat("maxmana") == null)
                me.WatchedAttributes.SetFloat("maxmana", me.CalculateMaxMana());

            return me.WatchedAttributes.GetFloat("maxmana");
        }

        public static void SetMaxMana(this Entity me, float maxmana)
        {
            me.WatchedAttributes.SetFloat("maxmana", maxmana);
        }

        private static float CalculateMaxMana(this Entity me)
        {
            // take whatever is highest starting with agililty
            int wisintagi = new[] { me.GetStatistic(StatType.Wisdom), me.GetStatistic(StatType.Intelligence), me.GetStatistic(StatType.Agility) }.Max(); ;

            double maxmana = ((850 * me.GetLevel()) + (85 * wisintagi * me.GetLevel())) / 425;
            //maxmana += getItemMana();

            if (maxmana > float.MaxValue)
                maxmana = float.MaxValue;

            return (float)Math.Floor(maxmana);
        }

        public static void TickInnateMana(this Entity me)
        {
            me.SetMana(me.GetMana() + me.GetInnateManaRegen());
        }

        public static float GetInnateManaRegen(this Entity me)
        {
            return 1;
        }

        public static int GetSkill(this Entity me, SkillType type)
        {
            return me.WatchedAttributes.GetInt("skill_" + type.ToString(), 0);
        }

        public static void SetSkill(this Entity me, SkillType type, int skill)
        {
            me.WatchedAttributes.SetInt("skill_" + type.ToString(), skill);
        }

        public static int GetMaxSkill(this Entity me, SkillType type)
        {
            return me.WatchedAttributes.GetInt("maxskill_" + type.ToString(), me.CalculateMaxSkillBasedOnLevel(type));
        }

        public static void SetMaxSkill(this Entity me, SkillType type, int skill)
        {
            me.WatchedAttributes.SetInt("maxskill_" + type.ToString(), skill);
        }

        public static int GetStatistic(this Entity me, StatType type)
        {
            return me.WatchedAttributes.GetInt("stat_" + type.ToString(), 0);
        }

        public static void SetStatistic(this Entity me, StatType type)
        {
            me.WatchedAttributes.SetInt("stat_" + type.ToString(), me.CalculateStatistic(type));
        }

        // from roleplayraces mod
        public static int GetBaseStatistic(this Entity me, StatType type)
        {
            var baseStat = me.WatchedAttributes.GetInt("base" + type.ToString().ToLower().Substring(0, 3), me.GetFallbackBaseStatistic());
            return baseStat;
        }

        public static int GetFallbackBaseStatistic(this Entity me)
        {
            if (me is EntityPlayer)
                return 75;

            if (me.GetAiMeleeAttackDamage() > 0)
                return me.GetAiMeleeAttackDamage() * 3;

            return 12;
        }

        public static int GetAiMeleeAttackDamage(this Entity me)
        {
            if (me is EntityPlayer)
                return 0;

            // Use melee attack value to determine its general level
            var aiMeleeAttack = me.GetBehavior<EntityBehaviorTaskAI>()?.TaskManager.AllTasks?.FirstOrDefault(e => e is AiTaskMeleeAttack);
            if (aiMeleeAttack != null && aiMeleeAttack is AiTaskMeleeAttack)
            {
                FieldInfo fieldInfo = typeof(AiTaskMeleeAttack).GetField("damage", BindingFlags.NonPublic | BindingFlags.Instance);

                var damageVal = (float)fieldInfo.GetValue(((AiTaskMeleeAttack)aiMeleeAttack));
                if (damageVal > 0)
                    return (int)damageVal;
            }

            return 0;
        }

        private static int CalculateStatistic(this Entity me, StatType type)
        {
            var stat = me.GetBaseStatistic(type);
            // items
            // effects
            stat += me.GetEffectStatisticBuff(type);

            if (stat > me.GetMaxStatistic(type))
                return me.GetMaxStatistic(type);

            return stat;
        }

        public static int GetMaxStatistic(this Entity me, StatType type)
        {
            return WorldLimits.MAX_STATISTIC;
        }

        public static float GetMana(this Entity me)
        {
            if (me.WatchedAttributes.TryGetFloat("currentmana") == null)
                me.SetMana(me.GetMaxMana());

            return me.WatchedAttributes.GetFloat("currentmana");
        }


        public static void ResetMaxMana(this Entity me)
        {
            me.SetMaxMana(me.CalculateMaxMana());
        }

        public static Dictionary<SkillType, Tuple<double,double>> GetSkillsValues(this Entity me)
        {
            var results = new Dictionary<SkillType, Tuple<double, double>>();
            foreach(SkillType skillType in Enum.GetValues(typeof(SkillType)))
                results.Add(skillType, new Tuple<double, double>(me.GetSkill(skillType), me.GetMaxSkill(skillType)));

            return results;
        }

        public static float CalculateMaxHealth(this Entity me)
        {
            if (!(me is EntityPlayer))
            {
                var meleeDamage = me.GetAiMeleeAttackDamage();
                if (meleeDamage < 1)
                    return 5;

                return meleeDamage * 4;
            }

            // Get highest class HP
            float highestStatHp = 1;
            foreach (AdventureClass adventuringClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var statHp = EntityUtils.GetStatMaxHP(adventuringClass, me.GetLevel(), me.GetStatistic(StatType.Stamina));
                if (statHp > highestStatHp)
                    highestStatHp = statHp;
            }

            //double itemHp = getItemHp();
            //double totalHp = statHp + itemHp;

            return highestStatHp;
        }

        public static void IncreaseMana(this Entity me, float mana)
        {
            me.SetMana(me.GetMana() + mana);
        }

        public static void DecreaseMana(this Entity me, float mana)
        {
            me.SetMana(me.GetMana() - mana);
        }

        public static void SetMana(this Entity me, float mana)
        {
            if (mana > me.GetMaxMana())
                mana = me.GetMaxMana();

            if (mana < 0)
                mana = 0;

            me.WatchedAttributes.SetFloat("currentmana", mana);
        }


        public static void ResetStatisticState(this Entity me)
        {
            me.SetStatistic(StatType.Strength);
            me.SetStatistic(StatType.Stamina);
            me.SetStatistic(StatType.Agility);
            me.SetStatistic(StatType.Dexterity);
            me.SetStatistic(StatType.Intelligence);
            me.SetStatistic(StatType.Wisdom);
            me.SetStatistic(StatType.Charisma);
        }

        public static void ResetSkillsToZeroAndRecalculateMaxSkills(this Entity me)
        {
            foreach(SkillType skillType in Enum.GetValues(typeof(SkillType)))
                me.SetSkill(skillType, 0);

            me.ResetMaxSkills();
        }

        public static void ResetMaxSkills(this Entity me)
        {
            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
                me.SetMaxSkill(skillType, me.CalculateMaxSkillBasedOnLevel(skillType));
        }

        public static float GetMaxHealth(this Entity me)
        {
            var behavior = me.GetBehavior<EntityBehaviorHealth>();
            if (behavior != null)
                return behavior.MaxHealth;

            return 1;
        }

        public static float GetHealth(this Entity me)
        {
            var behavior = me.GetBehavior<EntityBehaviorHealth>();
            if (behavior != null)
                return behavior.Health;

            return 1;
        }

        public static void ResetMaxHealth(this Entity me)
        {
            var behavior = me.GetBehavior<EntityBehaviorHealth>();
            if (behavior != null)
            {
                behavior.BaseMaxHealth = me.CalculateMaxHealth();
                behavior.UpdateMaxHealth();
            }
        }

        public static bool ChangeCurrentHp(this Entity me, Entity sourceEntity, float amount, EnumDamageType type)
        {
            return me.ReceiveDamage(
                new DamageSource() { 
                Source = sourceEntity is EntityPlayer ? EnumDamageSource.Player : EnumDamageSource.Entity,
                SourceEntity = sourceEntity, 
                Type = type
                },
                amount
                );
        }

        public static bool BindToLocation(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return false;

            return me.GetAsIServerPlayer().BindToLocation();
        }

        public static IServerPlayer GetAsIServerPlayer(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return null;

            return ((IServerPlayer)((EntityPlayer)me).Player);
        }
        public static bool GateToBind(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return false;

            return me.GetAsIServerPlayer().GateToBind();
        }

        public static void AwardPendingExperience(this Entity me, double experienceAmount)
        {
            // Only award to players
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().GrantPendingExperience(experienceAmount);
        }

        public static int GetLevel(this Entity me)
        {
            if (me.IsIServerPlayer())
                return me.GetAsIServerPlayer().GetLevel();

            // temporary work around - based on npc damage
            return (me.GetAiMeleeAttackDamage())+1;
        }

        public static Tuple<AdventureClass,int> GetHighestLevelOrNone(this Entity me, List<AdventureClass> adventureClasses)
        {
            if (adventureClasses == null || adventureClasses.Count() < 1)
                return new Tuple<AdventureClass, int>(AdventureClass.None,0);

            // temporary work around - based on npc damage
            if (!me.IsIServerPlayer())
                return new Tuple<AdventureClass, int>(AdventureClass.None, me.GetAiMeleeAttackDamage() + 1);

            var highestLevel = 0;
            var highestclass = AdventureClass.None;
            foreach (var adventureClass in adventureClasses)
            {
                var level = me.GetLevel(adventureClass);
                if (level > highestLevel)
                {
                    highestclass = adventureClass;
                    highestLevel = level;
                }
            }

            return new Tuple<AdventureClass, int>(highestclass, highestLevel);
        }

        public static int GetHighestLevel(this Entity me)
        {
            return me.GetHighestLevelOrNone().Item2;
        }

        public static string GetRaceName(this Entity me)
        {
            var raceName = me.WatchedAttributes.GetString("racename");
            if (!String.IsNullOrEmpty(raceName))
                return raceName;

            return "Unknown";
        }

        public static Tuple<AdventureClass, int> GetHighestLevelOrNone(this Entity me)
        {
            List<AdventureClass> adventureClasses = new List<AdventureClass>();
            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
                adventureClasses.Add(adventureClass);
            return me.GetHighestLevelOrNone(adventureClasses);
        }



        public static int GetHighestLevel(this Entity me, List<AdventureClass> adventureClasses)
        {
            return me.GetHighestLevelOrNone(adventureClasses).Item2;
        }

        public static int CalculateMaxSkillBasedOnLevel(this Entity me, SkillType skillType)
        {
            switch(skillType)
            {
                case SkillType.Archery:
                case SkillType.Throwing:
                    return me.CalculateMaxSkillRangedWeaponTypesBasedOnLevel(skillType);
                case SkillType.Defense:
                    return me.CalculateMaxSkillDefenseBasedOnLevel();
                case SkillType.Offense:
                    return me.CalculateMaxSkillOffenseBasedOnLevel();
                case SkillType.Slashing:
                case SkillType.Crushing:
                case SkillType.Piercing:
                case SkillType.HandtoHand:
                case SkillType.TwoHandBlunt:
                case SkillType.TwoHandPiercing:
                case SkillType.TwoHandSlashing:
                    return me.CalculateMaxSkillWeaponTypesBasedOnLevel(skillType);
            }

            return 0;
        }

        public static int CalculateMaxSkillOffenseBasedOnLevel(this Entity me)
        {
            var highest = 0;

            // take the highest out of classes
            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var maxSkill = me.CalculateMaxSkillOffenseBasedOnLevel(adventureClass);
                if (maxSkill > highest)
                    highest = maxSkill;
            }

            return highest;
        }

        public static int CalculateMaxSkillDefenseBasedOnLevel(this Entity me)
        {
            var highest = 0;

            // take the highest out of classes
            foreach(AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var maxSkill = me.CalculateMaxSkillDefenseBasedOnLevel(adventureClass);
                if (maxSkill > highest)
                    highest = maxSkill;
            }

            return highest;
        }

        public static int CalculateMaxSkillRangedWeaponTypesBasedOnLevel(this Entity me, SkillType skillType)
        {
            if (skillType != SkillType.Archery && skillType != SkillType.Throwing)
                return 0;

            var highest = 0;

            // take the highest out of classes
            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var maxSkill = 0;

                if (skillType == SkillType.Archery)
                    maxSkill = me.CalculateMaxSkillArcheryBasedOnLevel(adventureClass);
                if (skillType == SkillType.Throwing)
                    maxSkill = me.CalculateMaxSkillThrowingBasedOnLevel(adventureClass);

                if (maxSkill > highest)
                    highest = maxSkill;
            }

            return highest;
        }

        public static int CalculateMaxSkillWeaponTypesBasedOnLevel(this Entity me, SkillType skillType)
        {
            var highest = 0;

            // take the highest out of classes
            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                var maxSkill = me.CalculateMaxSkillWeaponTypesBasedOnLevel(adventureClass, skillType);
                if (maxSkill > highest)
                    highest = maxSkill;
            }

            return highest;
        }

        public static void TryIncreaseSkill(this Entity me, SkillType skillType, int skillupamount)
        {
            var currentskill = me.GetSkill(skillType);
            int skillcap = me.GetMaxSkill(skillType);
            if ((currentskill + skillupamount) > skillcap)
            {
                return;
            }

            int chance = 10 + ((252 - currentskill) / 20);
            if (chance < 1)
            {
                chance = 1;
            }

            Random r = new Random();
            int randomInt = r.Next(100) + 1;
            if (randomInt < chance)
                me.SetSkill(skillType, currentskill + skillupamount);
        }

        public static int CalculateMaxSkillThrowingBasedOnLevel(this Entity me, AdventureClass adventureClass)
        {
            int r_value = 0;

            switch (adventureClass)
            {
                // Melee
                case AdventureClass.Berserker:
                case AdventureClass.Rogue:
                    {
                        var besroglevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Berserker, AdventureClass.Rogue });

                        // 220 250
                        r_value = ((besroglevel * 5) + 5);
                        if (besroglevel < 51)
                        {
                            if (r_value > 220)
                            {
                                r_value = 220;
                            }
                        }
                        if (r_value > 250)
                        {
                            r_value = 250;
                        }
                        break;
                    }
                case AdventureClass.Warrior:
                case AdventureClass.Monk:
                    {
                        var warmonklvl = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Warrior, AdventureClass.Monk });

                        // 113 200
                        r_value = ((warmonklvl * 5) + 5);
                        if (warmonklvl < 51)
                        {
                            if (r_value > 113)
                            {
                                r_value = 113;
                            }
                        }
                        if (r_value > 200)
                        {
                            r_value = 200;
                        }
                        break;
                    }
                // Hybrid
                case AdventureClass.Beastlord:
                case AdventureClass.Bard:
                case AdventureClass.Ranger:
                    {
                        var bstbrdrnglvl = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Beastlord, AdventureClass.Bard, AdventureClass.Ranger });

                        // 113
                        r_value = ((bstbrdrnglvl * 5) + 5);
                        if (r_value > 113)
                        {
                            r_value = 113;
                        }
                        break;
                    }
                // Pure
                case AdventureClass.Necromancer:
                case AdventureClass.Wizard:
                case AdventureClass.Magician:
                case AdventureClass.Enchanter:
                    {
                        var casterlvl = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Necromancer, AdventureClass.Wizard, AdventureClass.Magician, AdventureClass.Enchanter});

                        // 75
                        r_value = ((casterlvl * 3) + 3);
                        if (r_value > 75)
                        {
                            r_value = 75;
                        }
                        break;
                    }
                // No skill classes
                case AdventureClass.Druid:
                case AdventureClass.Shaman:
                case AdventureClass.Cleric:
                case AdventureClass.Paladin:
                case AdventureClass.Shadowknight:
                default:
                    r_value = 0;
                    break;
            }

            if (r_value > 252)
            {
                r_value = 252;
            }
            return r_value;
        }

        public static int CalculateMaxSkillArcheryBasedOnLevel(this Entity me, AdventureClass adventureClass)
        {
            int r_value = 0;

            switch (adventureClass)
            {
                // Melee
                case AdventureClass.Rogue:
                case AdventureClass.Warrior:
                    {
                        var rogwarlvl = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Rogue, AdventureClass.Warrior });
                        // 200 240
                        r_value = ((rogwarlvl * 5) + 5);
                        if (rogwarlvl < 51 && r_value > 200)
                        {
                            r_value = 200;
                        }
                        if (r_value > 240)
                        {
                            r_value = 240;
                        }
                        break;
                    }
                // Hybrid
                case AdventureClass.Paladin:
                case AdventureClass.Shadowknight:
                    {
                        var palshdlvl = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Paladin, AdventureClass.Shadowknight });

                        // 75 75
                        r_value = ((palshdlvl * 5) + 5);
                        if (r_value > 75)
                        {
                            r_value = 75;
                        }
                        break;
                    }
                case AdventureClass.Ranger:
                    {
                        // 240 240
                        r_value = ((me.GetLevel(AdventureClass.Ranger) * 5) + 5);
                        if (r_value > 240)
                        {
                            r_value = 240;
                        }
                        break;
                    }
                // Pure
                // No skill classes
                // Melee
                case AdventureClass.Monk:
                // Priest
                case AdventureClass.Druid:
                case AdventureClass.Shaman:
                case AdventureClass.Cleric:
                // Pure
                case AdventureClass.Necromancer:
                case AdventureClass.Wizard:
                case AdventureClass.Magician:
                case AdventureClass.Enchanter:
                // Hybrid
                case AdventureClass.Beastlord:
                case AdventureClass.Bard:
                default:
                    r_value = 0;
                    break;
            }

            if (r_value > 252)
            {
                r_value = 252;
            }
            return r_value;
        }

        public static int CalculateMaxSkillWeaponTypesBasedOnLevel(this Entity me, AdventureClass adventureClass, SkillType skillType)
        {
            int r_value = 0;

            switch (adventureClass)
            {
                // Pure melee classes
                case AdventureClass.Warrior:
                    {
                        r_value = 5 + (me.GetLevel(AdventureClass.Warrior) * 5);
                        if (me.GetLevel(AdventureClass.Warrior) < 51 && r_value > 200)
                        {
                            r_value = 200;
                        }
                        if (me.GetLevel(AdventureClass.Warrior) > 50 && r_value > 250)
                        {
                            r_value = 250;
                        }
                        switch (skillType)
                        {
                            case SkillType.Piercing:
                                {
                                    if (r_value > 240)
                                    {
                                        r_value = 240;
                                    }
                                    break;
                                }
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 100)
                                    {
                                        r_value = 100;
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case AdventureClass.Monk:
                    {
                        r_value = 5 + (me.GetLevel(AdventureClass.Monk) * 5);
                        if (me.GetLevel(AdventureClass.Monk) < 51 && r_value > 240)
                            if (r_value > 240)
                            {
                                r_value = 240;
                            }
                        switch (skillType)
                        {
                            case SkillType.Crushing:
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 225 && me.GetLevel(AdventureClass.Monk) < 51)
                                    {
                                        r_value = 225;
                                    }
                                    break;
                                }
                            case SkillType.Piercing:
                            case SkillType.Slashing:
                            case SkillType.TwoHandSlashing:
                                {
                                    r_value = 0;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case AdventureClass.Rogue:
                    {
                        r_value = 5 + (me.GetLevel(AdventureClass.Rogue) * 5);
                        if (me.GetLevel(AdventureClass.Rogue) > 50 && r_value > 250)
                        {
                            r_value = 250;
                        }
                        if (me.GetLevel(AdventureClass.Rogue) < 51)
                        {
                            if (r_value > 200 && skillType != SkillType.Piercing)
                            {
                                r_value = 200;
                            }
                            if (r_value > 210 && skillType == SkillType.Piercing)
                            {
                                r_value = 210;
                            }
                        }
                        if (skillType == SkillType.HandtoHand && r_value > 100)
                        {
                            r_value = 100;
                        }
                        break;
                    }
                case AdventureClass.Berserker:
                    {
                        r_value = 5 + (me.GetLevel(AdventureClass.Berserker) * 5);
                        if (me.GetLevel(AdventureClass.Berserker) < 51 && r_value > 240)
                        {
                            r_value = 240;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 198)
                                    {
                                        r_value = 198;
                                    }
                                    break;
                                }
                            case SkillType.Piercing:
                                {
                                    if (r_value > 240)
                                    {
                                        r_value = 240;
                                    }
                                    break;
                                }
                            case SkillType.Slashing:
                            case SkillType.Crushing:
                            case SkillType.TwoHandBlunt:
                            case SkillType.TwoHandSlashing:
                                {
                                    if (r_value > 252)
                                    {
                                        r_value = 252;
                                    }
                                    break;
                                }
                            default:
                                r_value = 0;
                                break;
                        }
                        break;
                    }
                // Priest classes
                case AdventureClass.Cleric:
                    {
                        r_value = 4 + (me.GetLevel(AdventureClass.Cleric) * 4);
                        if (r_value > 175)
                        {
                            r_value = 175;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 75)
                                    {
                                        r_value = 75;
                                    }
                                    break;
                                }
                            case SkillType.Piercing:
                            case SkillType.Slashing:
                            case SkillType.TwoHandSlashing:
                                {
                                    r_value = 0;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case AdventureClass.Druid:
                    {
                        r_value = 4 + (me.GetLevel(AdventureClass.Druid) * 4);
                        if (r_value > 175)
                        {
                            r_value = 175;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 75)
                                    {
                                        r_value = 75;
                                    }
                                }
                                break;
                            case SkillType.Piercing:
                            case SkillType.TwoHandSlashing:
                                {
                                    r_value = 0;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case AdventureClass.Shaman:
                    {
                        r_value = 4 + (me.GetLevel(AdventureClass.Shaman) * 4);
                        if (r_value > 200)
                        {
                            r_value = 200;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 75)
                                    {
                                        r_value = 75;
                                    }
                                }
                                break;
                            case SkillType.Slashing:
                            case SkillType.TwoHandSlashing:
                                {
                                    r_value = 0;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                // Hybrids
                case AdventureClass.Ranger:
                    {
                        r_value = 5 + (me.GetLevel(AdventureClass.Ranger) * 5);
                        if (me.GetLevel(AdventureClass.Ranger) > 50)
                        {
                            if (r_value > 250)
                            {
                                r_value = 250;
                            }
                            switch (skillType)
                            {
                                case SkillType.Piercing:
                                    {
                                        if (r_value > 240)
                                        {
                                            r_value = 240;
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        else if (me.GetLevel(AdventureClass.Ranger) < 51)
                        {
                            if (r_value > 200)
                            {
                                r_value = 200;
                            }
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 100)
                                    {
                                        r_value = 100;
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case AdventureClass.Paladin:
                case AdventureClass.Shadowknight:
                    {
                        var highestKnightLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Paladin, AdventureClass.Shadowknight });

                        r_value = 5 + (highestKnightLevel * 5);
                        if (highestKnightLevel > 50 && r_value > 225)
                        {
                            r_value = 225;
                        }
                        if (highestKnightLevel < 51 && r_value > 200)
                        {
                            r_value = 200;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 100)
                                    {
                                        r_value = 100;
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case AdventureClass.Bard:
                    {
                        r_value = 5 + (me.GetLevel(AdventureClass.Bard) * 5);
                        if (me.GetLevel(AdventureClass.Bard) > 51 && r_value > 225)
                        {
                            r_value = 225;
                        }
                        if (me.GetLevel(AdventureClass.Bard) < 51 && r_value > 200)
                        {
                            r_value = 200;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 100)
                                    {
                                        r_value = 100;
                                    }
                                    break;
                                }
                            case SkillType.TwoHandBlunt:
                            case SkillType.TwoHandSlashing:
                                {
                                    r_value = 0;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                case AdventureClass.Beastlord:
                    {
                        r_value = 4 + (me.GetLevel(AdventureClass.Beastlord) * 4);
                        if (me.GetLevel(AdventureClass.Beastlord) > 51)
                        {
                            if (r_value > 225)
                            {
                                r_value = 225;
                            }
                        }
                        if (me.GetLevel(AdventureClass.Beastlord) < 51 && r_value > 200)
                        {
                            r_value = 200;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    r_value = 5 + (me.GetLevel(AdventureClass.Beastlord) * 5); // Beastlords use different max skill formula only for h2h 200/250
                                    if (me.GetLevel(AdventureClass.Beastlord) < 51)
                                    {
                                        r_value = 200;
                                    }
                                    break;
                                }
                            case SkillType.Slashing:
                            case SkillType.TwoHandSlashing:
                                {
                                    r_value = 0;
                                    break;
                                }
                            default:
                                break;
                        }
                        if (r_value > 250)
                        {
                            r_value = 250;
                        }
                        break;
                    }
                // Pure casters
                case AdventureClass.Necromancer:
                case AdventureClass.Wizard:
                case AdventureClass.Magician:
                case AdventureClass.Enchanter:
                    {
                        var highestCasterLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Necromancer, AdventureClass.Wizard, AdventureClass.Magician, AdventureClass.Enchanter });

                        r_value = 3 + (highestCasterLevel * 3);
                        if (r_value > 110)
                        {
                            r_value = 110;
                        }
                        switch (skillType)
                        {
                            case SkillType.HandtoHand:
                                {
                                    if (r_value > 75)
                                    {
                                        r_value = 75;
                                    }
                                }
                                break;
                            case SkillType.Slashing:
                            case SkillType.TwoHandSlashing:
                                {
                                    r_value = 0;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                default:
                    break;
            }

            if (r_value > 252)
            {
                r_value = 252;
            }
            return r_value;
        }

        public static int CalculateMaxSkillOffenseBasedOnLevel(this Entity me, AdventureClass adventureClass)
        {
            int r_value = 0;

			switch (adventureClass)
			{
                // Melee
                case AdventureClass.Warrior:
                case AdventureClass.Berserker:
                case AdventureClass.Rogue:
                    {
                        var highestMeleeLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Warrior, AdventureClass.Berserker, AdventureClass.Rogue });

                        // 210 252 5*level+5
                        r_value = ((highestMeleeLevel * 5) + 5);
                        if (highestMeleeLevel < 51)
                        {
                            if (r_value > 210)
                            {
                                r_value = 210;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Monk:
                    {
                        // 230 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Monk) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Monk) < 51)
                        {
                            if (r_value > 230)
                            {
                                r_value = 230;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                // Priest
                case AdventureClass.Druid:
                case AdventureClass.Shaman:
                case AdventureClass.Cleric:
                    {
                        var highestPriestLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Druid, AdventureClass.Shaman, AdventureClass.Cleric });

                        // 200 200 4*level+4
                        r_value = ((highestPriestLevel * 4) + 4);
                        if (r_value > 200)
                        {
                            r_value = 200;
                        }
                        break;
                    }
                // Hybrid
                case AdventureClass.Beastlord:
                    {
                        // 200 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Beastlord) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Beastlord) < 51)
                        {
                            if (r_value > 200)
                            {
                                r_value = 200;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Paladin:
                case AdventureClass.Bard:
                case AdventureClass.Shadowknight:
                    {
                        var highestKnightLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Paladin, AdventureClass.Bard, AdventureClass.Shadowknight });

                        // 200 225 5*level+5
                        r_value = ((highestKnightLevel * 5) + 5);
                        if (highestKnightLevel < 51)
                        {
                            if (r_value > 200)
                            {
                                r_value = 200;
                            }
                        }
                        if (r_value > 225)
                        {
                            r_value = 225;
                        }
                        break;
                    }
                case AdventureClass.Ranger:
                    {
                        // 210 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Ranger) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Ranger) < 51)
                        {
                            if (r_value > 210)
                            {
                                r_value = 210;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                // Pure
                case AdventureClass.Necromancer:
                case AdventureClass.Wizard:
                case AdventureClass.Magician:
                case AdventureClass.Enchanter:
                    {
                        var highestCasterLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Necromancer, AdventureClass.Wizard, AdventureClass.Magician, AdventureClass.Enchanter});

                        // 140 140 level*4
                        r_value = (highestCasterLevel * 4);
                        if (r_value > 140)
                        {
                            r_value = 140;
                        }
                        break;
                    }
                default:
                    break;
            }

			if (r_value > 252)
			{
				r_value = 252;
			}
			return r_value;
		}

        public static int CalculateMaxSkillDefenseBasedOnLevel(this Entity me, AdventureClass adventureClass)
        {
            int r_value = 0;

            switch (adventureClass)
            {
                // Melee
                case AdventureClass.Warrior:
                    {
                        // 210 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Warrior) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Warrior) < 51)
                        {
                            if (r_value > 210)
                            {
                                r_value = 210;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Rogue:
                    {
                        // 200 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Rogue) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Rogue) < 51)
                        {
                            if (r_value > 200)
                            {
                                r_value = 200;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Monk:
                    {
                        // 230 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Monk) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Monk) < 51)
                        {
                            if (r_value > 230)
                            {
                                r_value = 230;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Berserker:
                    {
                        // 230 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Berserker) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Berserker) < 51)
                        {
                            if (r_value > 230)
                            {
                                r_value = 230;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                // Priest
                case AdventureClass.Druid:
                case AdventureClass.Shaman:
                case AdventureClass.Cleric:
                    {
                        var highestPriestLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Druid , AdventureClass.Shaman, AdventureClass.Cleric } );

                        // 200 200 4*level+4
                        r_value = ((highestPriestLevel * 4) + 4);
                        if (r_value > 200)
                        {
                            r_value = 200;
                        }
                        break;
                    }
                // Hybrid
                case AdventureClass.Beastlord:
                    {
                        // 210 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Beastlord) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Beastlord) < 51)
                        {
                            if (r_value > 210)
                            {
                                r_value = 210;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Paladin:
                case AdventureClass.Shadowknight:
                    {
                        var highestKnightLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Paladin, AdventureClass.Shadowknight });

                        // 210 252 5*level+5
                        r_value = ((highestKnightLevel * 5) + 5);
                        if (highestKnightLevel < 51)
                        {
                            if (r_value > 210)
                            {
                                r_value = 210;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Bard:
                    {
                        // 200 252 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Bard) * 5) + 5);
                        if (me.GetLevel(AdventureClass.Bard) < 51)
                        {
                            if (r_value > 200)
                            {
                                r_value = 200;
                            }
                        }
                        if (r_value > 252)
                        {
                            r_value = 252;
                        }
                        break;
                    }
                case AdventureClass.Ranger:
                    {
                        // 200 200 5*level+5
                        r_value = ((me.GetLevel(AdventureClass.Ranger) * 5) + 5);
                        if (r_value > 200)
                        {
                            r_value = 200;
                        }
                        break;
                    }
                // Pure
                case AdventureClass.Necromancer:
                case AdventureClass.Wizard:
                case AdventureClass.Magician:
                case AdventureClass.Enchanter:
                    {
                        var highestCasterLevel = me.GetHighestLevel(new List<AdventureClass>() { AdventureClass.Necromancer, AdventureClass.Wizard, AdventureClass.Magician, AdventureClass.Enchanter });

                        // 145 145 level*4
                        r_value = (highestCasterLevel * 4);
                        if (r_value > 140)
                        {
                            r_value = 140;
                        }
                        break;
                    }
                default:
                    break;
            }

			// Switch skill
			if (r_value > 252)
			{
				r_value = 252;
			}
			return r_value;
		}

        public static double GetExperienceWorth(this Entity killed, IServerPlayer killer)
        {
            // Only award when killing npcs
            if ((killed is EntityPlayer))
                return 0D;

            return (EntityUtils.GetExperienceRewardAverageForLevel(killed.GetLevel(), killer.GetLevel()));
        }

        public static void GrantSmallAmountOfPendingExperience(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().GrantSmallAmountOfPendingExperience();
        }
    }
}
