using System;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
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

        public static void Cleanup(this Entity me)
        {
            EntityBehaviorCasting ebt = me.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return;

            ebt.ClearCasting();
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

        public static int GetLevel(this Entity me, AdventureClass adventureClass)
        {
            if (me == null)
                return 1;

            return me.WatchedAttributes.GetInt(adventureClass.ToString().ToLower() + "level", 1);
        }

        public static bool IsWaitingToCast(this Entity me)
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

            return ebt.IsWaitingToCast();
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
            var baseStat = me.WatchedAttributes.GetInt("base" + type.ToString().ToLower().Substring(0, 3), me.GetFallbackBaseStatisticByType());
            return baseStat;
        }

        public static int GetFallbackBaseStatisticByType(this Entity me)
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

        public static void GrantSmallAmountOfAdventureClassXp(this Entity me, Ability ability)
        {
            if (ability.AdventureClass == AdventureClass.None)
                return;

            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().GrantSmallAmountOfAdventureClassXp(ability);
        }

        public static void SkillUp(this Entity me, Ability ability)
        {
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().SkillUp(ability);
        }

        public static bool GateToBind(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return false;

            return me.GetAsIServerPlayer().GateToBind();
        }

        public static void AwardExperience(this Entity me, AdventureClass experienceType, double experienceAmount)
        {
            if (experienceType == AdventureClass.None)
                return;

            // Only award to players
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().GrantExperience(experienceType, experienceAmount);
        }

        public static int GetLevel(this Entity me)
        {
            if (me.IsIServerPlayer())
                return me.GetAsIServerPlayer().GetLevel();

            return me.GetAiMeleeAttackDamage()+1;
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
