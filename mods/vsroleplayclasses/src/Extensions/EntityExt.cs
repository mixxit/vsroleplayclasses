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
            if (me.IsUnfinishedCasting())
                return true;

            if (me.IsWaitingToReleaseCast())
                return true;

            return false;
        }

        public static int GetLevel(this Entity me, AdventureClass adventureClass)
        {
            if (me == null)
                return 1;

            return me.WatchedAttributes.GetInt(adventureClass.ToString().ToLower() + "level", 1);
        }

        public static bool IsUnfinishedCasting(this Entity me)
        {
            if (me.Api.Side == EnumAppSide.Client)
            {
                if (me.WatchedAttributes.GetLong("finishCastingUnixTime") <= 0)
                    return false;

                if (DateTimeOffset.Now.ToUnixTimeMilliseconds() <= me.WatchedAttributes.GetLong("finishCastingUnixTime"))
                    return true;
                else
                    return false;
            }

            EntityBehaviorCasting ebt = me.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return false;

            return ebt.IsUnfinishedCasting();
        }

        public static bool IsWaitingToReleaseCast(this Entity me)
        {
            if (me.Api.Side == EnumAppSide.Client)
            {
                if (me.WatchedAttributes.GetLong("finishCastingUnixTime") <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                    return true;
                else
                    return false;
            }

            EntityBehaviorCasting ebt = me.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return false;

            return ebt.IsWaitingToReleaseCastCast();
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
                return me.GetAiMeleeAttackDamage() * 7;

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
            if (!(me is EntityPlayer) && me.GetAiMeleeAttackDamage() < 1)
                return 5;

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
            return me.GetAiMeleeAttackDamage()+1;
        }

        public static int GetHighestLevel(this Entity me, List<AdventureClass> adventureClasses)
        {
            if (adventureClasses == null || adventureClasses.Count() < 1)
                return 0;

            // temporary work around - based on npc damage
            if (!me.IsIServerPlayer())
                return me.GetAiMeleeAttackDamage() + 1;

            var highestLevel = 0;
            foreach (var adventureClass in adventureClasses)
            {
                var level = me.GetLevel(adventureClass);
                if (level > highestLevel)
                    highestLevel = level;
            }

            return highestLevel;
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
    }
}
