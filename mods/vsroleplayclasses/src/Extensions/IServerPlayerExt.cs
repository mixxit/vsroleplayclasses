using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src.Extensions
{
    public static class IServerPlayerExt
    {

        public static void ResetExperience(this IServerPlayer player)
        {
            if (player.GetCharClassOrDefault() == null)
                return;

            foreach (EnumAdventuringClass adventuringClass in Enum.GetValues(typeof(EnumAdventuringClass)))
            {
                if (adventuringClass == EnumAdventuringClass.None)
                    continue;

                player.SetExperience(adventuringClass, 0);
            }
            
        }

        public static void TryUpdateLevel(this IServerPlayer player)
        {
            if (player.GetLevel() == player.CalculateLevel())
                return;

            player.SetLevel();
        }


        public static List<Tuple<string,double>> GetExperienceValues(this IServerPlayer player)
        {
            var result = new List<Tuple<string, double>>();

            if (player.GetCharClassOrDefault() == null)
                return result;

            foreach (EnumAdventuringClass adventuringClass in Enum.GetValues(typeof(EnumAdventuringClass)))
            {
                if (adventuringClass == EnumAdventuringClass.None)
                    continue;
                result.Add(new Tuple<string, double>(adventuringClass.ToString().ToLower(), player.GetExperience(adventuringClass)));
            }

            return result;
        }

        public static double GetExperience(this IServerPlayer player)
        {
            return player.GetExperienceValues().Sum(e => e.Item2);
        }

        public static double GetExperience(this IServerPlayer player, EnumAdventuringClass experienceType)
        {
            if (experienceType == EnumAdventuringClass.None)
                return 0D;

            return player.Entity.WatchedAttributes.GetDouble(experienceType.ToString().ToLower() + "xp", 0);
        }

        public static void SetExperience(this IServerPlayer player, EnumAdventuringClass experienceType, double xp)
        {
            if (experienceType == EnumAdventuringClass.None)
                return;

            if ((player.GetExperience() + xp) > WorldLimits.GetMaxExperience())
                xp = WorldLimits.GetMaxExperience();

            player.Entity.WatchedAttributes.SetDouble(experienceType.ToString().ToLower() + "xp", xp);
        }

        public static void ResetMana(this IServerPlayer player)
        {
            player.SetMana(player.GetMaxMana());
        }

        public static void ResetMaxMana(this IServerPlayer player)
        {
            player.SetMaxMana(player.CalculateMaxMana());
        }

        public static float CalculateMaxHealth(this IServerPlayer player)
        {
            // Get highest class HP
            float highestStatHp = 1;
            foreach (var classExperiences in player.GetExperienceValues())
            {
                var statHp = EntityUtils.GetStatMaxHP(classExperiences.Item1, player.GetLevel(), player.GetStatistic(StatType.Stamina));
                if (statHp > highestStatHp)
                    highestStatHp = statHp;
            }

            //double itemHp = getItemHp();
            //double totalHp = statHp + itemHp;

            return highestStatHp;
        }

        public static float GetMaxHealth(this IServerPlayer player)
        {
            var behavior = player.Entity.GetBehavior<EntityBehaviorHealth>();
            if (behavior != null)
                return behavior.MaxHealth;

            return 1;
        }

        public static float GetHealth(this IServerPlayer player)
        {
            var behavior = player.Entity.GetBehavior<EntityBehaviorHealth>();
            if (behavior != null)
                return behavior.Health;

            return 1;
        }

        public static void ResetMaxHealth(this IServerPlayer player)
        {
            var behavior = player.Entity.GetBehavior<EntityBehaviorHealth>();
            if (behavior != null)
            {
                behavior.BaseMaxHealth = player.CalculateMaxHealth();
                behavior.UpdateMaxHealth();
            }
        }

        public static int GetLevel(this IServerPlayer player)
        {
            return player.Entity.WatchedAttributes.GetInt("level", 1);
        }

        public static void ResetStatisticState(this IServerPlayer player)
        {
            player.SetStatistic(StatType.Strength);
            player.SetStatistic(StatType.Stamina);
            player.SetStatistic(StatType.Agility);
            player.SetStatistic(StatType.Dexterity);
            player.SetStatistic(StatType.Intelligence);
            player.SetStatistic(StatType.Wisdom);
            player.SetStatistic(StatType.Charisma);
        }

        public static string GetPlayerOverviewAsText(this IServerPlayer player)
        {
            var text = "Overall Level ["+player.GetLevel()+"] progress: " + player.GetExperiencePercentage() + "% into level - XP: " + player.GetExperience() + "/" + PlayerUtils.GetExperienceRequirementForLevel(player.GetLevel()) + Environment.NewLine;
            text += 
                "STR: " + player.GetStatistic(StatType.Strength) +
                " STA: " + player.GetStatistic(StatType.Stamina) +
                " AGI: " + player.GetStatistic(StatType.Agility) +
                " DEX: " + player.GetStatistic(StatType.Dexterity) +
                " INT: " + player.GetStatistic(StatType.Intelligence) +
                " WIS: " + player.GetStatistic(StatType.Wisdom) +
                " CHA: " + player.GetStatistic(StatType.Charisma) + 
                Environment.NewLine
                ;
            text += "HP: " + player.GetHealth() + "/" + player.GetMaxHealth() + 
                " MP: " + player.GetMana() + "/" + player.GetMaxMana()
                + Environment.NewLine;

            return text;
        }

        public static int GetExperiencePercentage(this IServerPlayer player)
        {
            return PlayerUtils.GetExperiencePercentage(player.GetLevel(), player.GetExperience());
        }

        public static int CalculateLevel(this IServerPlayer player)
        {
            return PlayerUtils.GetLevelFromExperience(player.GetExperience());
        }

        public static void SetLevel(this IServerPlayer player)
        {
            player.Entity.WatchedAttributes.SetInt("level", player.CalculateLevel());
        }

        private static double GetMaxMana(this IServerPlayer player)
        {
            return player.Entity.WatchedAttributes.GetDouble("maxmana", player.CalculateMaxMana());
        }

        public static void SetMaxMana(this IServerPlayer player, double maxmana)
        {
            player.Entity.WatchedAttributes.SetDouble("maxmana", maxmana);
        }

        private static double CalculateMaxMana(this IServerPlayer player)
        {
            // take whatever is highest starting with agililty
            int wisintagi = new[] { player.GetStatistic(StatType.Wisdom), player.GetStatistic(StatType.Intelligence), player.GetStatistic(StatType.Agility) }.Max(); ;

            double maxmana = ((850 * player.GetLevel()) + (85 * wisintagi * player.GetLevel())) / 425;
            //maxmana += getItemMana();

            return (double)Math.Floor(maxmana);
        }

        public static int GetStatistic(this IServerPlayer player, StatType type)
        {
            return player.Entity.WatchedAttributes.GetInt("stat_"+type.ToString(), 0);
        }

        public static void SetStatistic(this IServerPlayer player, StatType type)
        {
            player.Entity.WatchedAttributes.SetInt("stat_" + type.ToString(), player.CalculateStatistic(type));
        }

        // from roleplayraces mod
        public static int GetBaseStatistic(this IServerPlayer player, StatType type)
        {
            var baseStat = player.Entity.WatchedAttributes.GetInt("base" + type.ToString().ToLower().Substring(0,3), 75);
            return baseStat;
        }

        private static int CalculateStatistic(this IServerPlayer player, StatType type)
        {
            var stat = player.GetBaseStatistic(type);
            // items
            // effects
            if (stat > player.GetMaxStatistic(type))
                return player.GetMaxStatistic(type);

            return stat;
        }

        public static int GetMaxStatistic(this IServerPlayer player, StatType type)
        {
            return WorldLimits.MAX_STATISTIC;
        }

        public static double GetMana(this IServerPlayer player)
        {
            return player.Entity.WatchedAttributes.GetDouble("mana", 0);
        }

        public static void SetMana(this IServerPlayer player, double mana)
        {
            if (mana > player.GetMaxMana())
                mana = player.GetMaxMana();

            player.Entity.WatchedAttributes.SetDouble("mana", mana);
        }

        public static void GrantExperience(this IServerPlayer player, EnumAdventuringClass experienceType, double xp)
        {
            if (experienceType == EnumAdventuringClass.None)
                return;

            // needs to have chosen their profession
            if (player.GetCharClassOrDefault() == null)
                return;

            if ((player.GetExperience() + xp) > WorldLimits.GetMaxExperience())
                xp = WorldLimits.GetMaxExperience() - player.GetExperience();

            if (!player.CanAddExp(xp))
                return;

            player.SetExperience(experienceType, GetExperience(player, experienceType) + xp);
        }

        public static bool CanAddExp(this IServerPlayer player, double xp)
        {
            if ((player.GetExperience()+xp) < WorldLimits.GetMaxExperience())
                return true;

            return false;
        }

        public static CharacterClass GetCharClassOrDefault(this IServerPlayer player)
        {
            if (player.GetSelectedClassCode() == null)
                return null;

            VSRoleplayClassesMod mod = player.Entity.World.Api.ModLoader.GetModSystem<VSRoleplayClassesMod>();

            var charClass = mod.GetCharacterClassesItems(player.GetSelectedClassCode());
            return charClass;
        }

        public static void CastSpell(this IServerPlayer player, int memorisedSpellSlot)
        {
            if (GetCurrentAbility() != null)
                GetCurrentAbility().Cast(player.GetTarget());
        }

        private static Entity GetTarget(this IServerPlayer player)
        {
            // Default to self
            return player.Entity;
        }

        private static Ability GetCurrentAbility()
        {
            return null;
        }

        public static void GrantInitialClassItems(this IServerPlayer player)
        {
            var charClass = player.GetCharClassOrDefault();
            if (charClass == null)
                return;

            var gear = new List<JsonItemStack>();
            // Filter out clothing
            foreach(var jsonItemStack in charClass.Gear)
            {
                if (!jsonItemStack.Resolve(player.Entity.World, "character class gear", true))
                    continue;

                ItemStack itemstack = jsonItemStack.ResolvedItemstack?.Clone();
                if (itemstack == null)
                    continue;

                gear.Add(jsonItemStack);
            }

            player.DeliverItems(gear);
        }

        public static void DeliverItems(this IServerPlayer player, List<JsonItemStack> gear)
        {
            // Worn items already get delivered by the SurvivalMod, we are merely giving the rest
            foreach (JsonItemStack jsonItemStack in gear)
                player.DeliverItem(jsonItemStack);
        }

        public static void DeliverItem(this IServerPlayer player, JsonItemStack jsonItemStack)
        {
            // Skip unknown item
            if (!jsonItemStack.Resolve(player.Entity.World, "character class gear", false))
                return;

            ItemStack itemstack = jsonItemStack.ResolvedItemstack?.Clone();
            if (itemstack == null)
                return;

            player.Entity.TryGiveItemStack(itemstack);
        }

        public static string GetSelectedClassCode(this IServerPlayer player)
        {
            string classCode = player.Entity.WatchedAttributes.GetString("characterClass", (string)null);
            if (String.IsNullOrEmpty(classCode))
                return null;

            return classCode;
        }

    }
}
