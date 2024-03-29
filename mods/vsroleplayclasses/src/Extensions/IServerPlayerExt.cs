﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Items;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Extensions
{
    public static class IServerPlayerExt
    {
        public static void SpendPendingExperience(this IServerPlayer player, AdventureClass adventureClass)
        {
            if (adventureClass == AdventureClass.None)
                return;

            var currentExperience = (double)player.GetExperience(adventureClass);
            var lvl = PlayerUtils.GetLevelFromExperience(currentExperience);
            var nextXp = PlayerUtils.GetExperienceRequirementForLevel(lvl + 1);
            var pendingXp = player.GetPendingExperience();
            var needed = nextXp - currentExperience;

            // Only spend pending experience if they have enough to level
            if (pendingXp < needed)
                return;

            player.SetPendingExperience(player.GetPendingExperience() - needed);
            player.GrantExperience(adventureClass, needed);
        }

        public static void GrantSmallAmountOfPendingExperience(this IServerPlayer me)
        {
            me.GrantPendingExperience(10);
        }

        public static void SendEmote(this IServerPlayer sourcePlayer, string message, bool prefixNonUserEmote)
        {
            foreach (var player in sourcePlayer.Entity.World.AllOnlinePlayers)
            {
                if (!(player is IServerPlayer))
                    continue;

                if (player.Entity.ServerPos.SquareDistanceTo(sourcePlayer.Entity.ServerPos) > WorldLimits.LocalChatDistance)
                    continue;

                SendEmoteLocally(sourcePlayer, (IServerPlayer)player, message, prefixNonUserEmote);
            }
        }

        private static void SendEmoteLocally(this IServerPlayer sourcePlayer, IServerPlayer destinationPlayer, string message, bool prefixNonUserEmote)
        {
            var chatType = EnumChatType.OwnMessage;
            if (!sourcePlayer.PlayerUID.Equals(destinationPlayer.PlayerUID))
                chatType = EnumChatType.OthersMessage;

            var nonUserPrefix = "[A] ";
            var prefix = $"{nonUserPrefix}* " + PlayerNameUtils.GetFullRoleplayNameAsDisplayFormat(sourcePlayer.Entity) + " ";
            destinationPlayer.SendMessage(GlobalConstants.GeneralChatGroup, prefix + message, chatType);
        }

        public static string GetFullRoleplayNameAsDisplayFormat(this IServerPlayer player)
        {
            return PlayerNameUtils.GetFullRoleplayNameAsDisplayFormat(player.Entity);
        }

        public static void ResetExperience(this IServerPlayer player)
        {
            if (player.GetCharClassOrDefault() == null)
                return;

            player.SetPendingExperience(0);

            foreach (AdventureClass adventuringClass in Enum.GetValues(typeof(AdventureClass)))
            {
                if (adventuringClass == AdventureClass.None)
                    continue;

                player.SetExperience(adventuringClass, 0);
            }
        }

        public static void TryUpdateLevel(this IServerPlayer player, AdventureClass adventureClass)
        {
            if (player.Entity.GetLevel(adventureClass) == player.CalculateLevel(adventureClass))
                return;

            player.SetLevel(adventureClass);
        }

        public static void TryUpdateOverallLevel(this IServerPlayer player)
        {
            if (player.GetLevel() == player.CalculateLevel())
                return;

            player.SetLevel();
        }

        public static bool HasLevel(this IServerPlayer player, Tuple<AdventureClass, int> adventureClassLevel)
        {
            if (player == null || player.Entity == null)
                return false;

            if (adventureClassLevel == null)
                return false;

            return player.Entity.GetLevel(adventureClassLevel.Item1) >= adventureClassLevel.Item2;
        }

        public static List<Tuple<AdventureClass,double>> GetExperienceValues(this IPlayer player)
        {
            var result = new List<Tuple<AdventureClass, double>>();

            if (player.GetSelectedClassCode() == null)
                return result;

            foreach (AdventureClass adventuringClass in Enum.GetValues(typeof(AdventureClass)))
            {
                if (adventuringClass == AdventureClass.None)
                    continue;
                result.Add(new Tuple<AdventureClass, double>(adventuringClass, player.GetExperience(adventuringClass)));
            }

            return result;
        }

        public static double GetExperience(this IPlayer player)
        {
            return player.GetExperienceValues().Sum(e => e.Item2);
        }

        public static double GetPendingExperience(this IPlayer player)
        {
            return player.Entity.WatchedAttributes.GetDouble("pendingxp", 0);
        }

        public static double GetExperience(this IPlayer player, AdventureClass experienceType)
        {
            if (experienceType == AdventureClass.None)
                return 0D;

            return player.Entity.WatchedAttributes.GetDouble(experienceType.ToString().ToLower() + "xp", 0);
        }

        public static void SetExperience(this IServerPlayer player, AdventureClass experienceType, double xp)
        {
            if (experienceType == AdventureClass.None)
                return;

            if (xp > WorldLimits.GetMaxExperience())
                xp = WorldLimits.GetMaxExperience();

            if (xp < 0)
                xp = 0;

            player.Entity.WatchedAttributes.SetDouble(experienceType.ToString().ToLower() + "xp", xp);
        }

        public static void SetPendingExperience(this IServerPlayer player, double xp)
        {
            if (xp > WorldLimits.GetMaxPendingExperience())
                xp = WorldLimits.GetMaxPendingExperience();

            if (xp < 0)
                xp = 0;

            player.Entity.WatchedAttributes.SetDouble("pendingxp", xp);
        }

        public static int GetLevel(this IServerPlayer player)
        {
            if (player == null || player.Entity == null)
                return 1;

            return player.Entity.WatchedAttributes.GetInt("level", 1);
        }

        public static Ability GetAbilityInMemoryPosition(this IServerPlayer player, int position)
        {
            if (position < 1 || position > 8)
                return null;

            if (player.InventoryManager.GetOwnInventory("memoriseability") == null)
                return null;

            var itemSlot = player.InventoryManager.GetOwnInventory("memoriseability")[position - 1];
            if (itemSlot == null || itemSlot.Itemstack == null)
                return null;

            if (!(itemSlot.Itemstack.Item is AbilityScrollItem))
                return null;

            return AbilityTools.GetAbility(player.Entity.World, ((AbilityScrollItem)itemSlot.Itemstack.Item).GetScribedAbilityId(itemSlot.Itemstack));
        }

        public static string GetPlayerOverallLevelAsText(this IServerPlayer player)
        {
            return "Overall Level [" + player.GetLevel() + "] progress: " + player.GetExperiencePercentage() + "% into level - XP: " + player.GetExperience() + "/" + PlayerUtils.GetExperienceRequirementForLevel(player.GetLevel()+1) + Environment.NewLine;
        }


        public static string GetPlayerOverviewAsText(this IServerPlayer player)
        {
            var text = player.GetPlayerOverallLevelAsText();
            text += 
                "STR: " + player.Entity.GetStatistic(StatType.Strength) +
                " STA: " + player.Entity.GetStatistic(StatType.Stamina) +
                " AGI: " + player.Entity.GetStatistic(StatType.Agility) +
                " DEX: " + player.Entity.GetStatistic(StatType.Dexterity) +
                " INT: " + player.Entity.GetStatistic(StatType.Intelligence) +
                " WIS: " + player.Entity.GetStatistic(StatType.Wisdom) +
                " CHA: " + player.Entity.GetStatistic(StatType.Charisma) + 
                Environment.NewLine
                ;
            text += "HP: " + player.Entity.GetHealth() + "/" + player.Entity.GetMaxHealth() + 
                " MP: " + player.Entity.GetMana() + "/" + player.Entity.GetMaxMana()
                + Environment.NewLine;

            return text;
        }

        public static int GetExperiencePercentage(this IServerPlayer player)
        {
            return PlayerUtils.GetExperiencePercentage(player.GetLevel(), player.GetExperience());
        }

        public static int GetExperiencePercentage(this IServerPlayer player, AdventureClass adventureClass)
        {
            return PlayerUtils.GetExperiencePercentage(player.Entity.GetLevel(adventureClass), player.GetExperience(adventureClass));
        }

        public static int CalculateLevel(this IServerPlayer player)
        {
            return PlayerUtils.GetLevelFromExperience(player.GetExperience());
        }

        public static int CalculateLevel(this IServerPlayer player, AdventureClass adventureClass)
        {
            return PlayerUtils.GetLevelFromExperience(player.GetExperience(adventureClass));
        }

        public static void SetLevel(this IServerPlayer player)
        {
            player.Entity.WatchedAttributes.SetInt("level", player.CalculateLevel());
        }

        public static void SetLevel(this IServerPlayer player, AdventureClass adventureClass)
        {
            player.Entity.WatchedAttributes.SetInt(adventureClass.ToString().ToLower()+"level", player.CalculateLevel(adventureClass));
        }

        public static void GrantPendingExperience(this IServerPlayer player, double xp)
        {
            // needs to have chosen their profession
            if (player.GetCharClassOrDefault() == null)
                return;

            if ((player.GetPendingExperience() + xp) > WorldLimits.GetMaxPendingExperience())
                xp = WorldLimits.GetMaxPendingExperience() - player.GetPendingExperience();

            if (!player.CanAddPendingExp(xp))
                return;

            player.SetPendingExperience(GetPendingExperience(player) + xp);
        }


        public static void GrantExperience(this IServerPlayer player, AdventureClass experienceType, double xp)
        {
            if (experienceType == AdventureClass.None)
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
            if ((player.GetExperience()+xp) <= WorldLimits.GetMaxExperience())
                return true;

            return false;
        }


        public static bool CanAddPendingExp(this IServerPlayer player, double xp)
        {
            if ((player.GetPendingExperience() + xp) <= WorldLimits.GetMaxPendingExperience())
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


        public static PlayerSpawnPos GetCurrentPositionAsPlayerSpawnPos(this IServerPlayer player)
        {
            return new PlayerSpawnPos()
            {
                x = player.Entity.ServerPos.XYZInt.X,
                y = player.Entity.ServerPos.XYZInt.Y,
                z = player.Entity.ServerPos.XYZInt.Z,
                yaw = player.Entity.ServerPos.Yaw,
                pitch = player.Entity.ServerPos.Pitch,
                RemainingUses = 99999999
            };
        }

        public static PlayerSpawnPos GetPlayerSpawnPos(this IServerPlayer player)
        {
            return new PlayerSpawnPos()
            {
                x = player.GetSpawnPosition(false).XYZInt.X,
                y = player.GetSpawnPosition(false).XYZInt.Y,
                z = player.GetSpawnPosition(false).XYZInt.Z,
                yaw = player.GetSpawnPosition(false).Yaw,
                pitch = player.GetSpawnPosition(false).Pitch,
                RemainingUses = 99999999
            };
        }

        public static bool BindToLocation(this IServerPlayer player)
        {
            var pos = player.GetCurrentPositionAsPlayerSpawnPos();
            player.SetSpawnPosition(pos);
            return true;
        }

        public static bool GateToBind(this IServerPlayer player)
        {
            player.Entity.TeleportTo(player.GetSpawnPosition(false).XYZ.AsBlockPos);
            return true;
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

        public static string GetSelectedClassCode(this IPlayer player)
        {
            string classCode = player.Entity.WatchedAttributes.GetString("characterClass", (string)null);
            if (String.IsNullOrEmpty(classCode))
                return null;

            return classCode;
        }

    }
}
