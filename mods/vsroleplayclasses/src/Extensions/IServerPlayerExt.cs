using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src;
using vsroleplayclasses.src.Extensions;

namespace projectrarahat.src.Extensions
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

            player.Entity.WatchedAttributes.SetDouble(experienceType.ToString().ToLower() + "xp", xp);
        }

        public static void GrantExperience(this IServerPlayer player, EnumAdventuringClass experienceType, double xp)
        {
            if (experienceType == EnumAdventuringClass.None)
                return;

            if (player.GetCharClassOrDefault() == null)
                return;

            player.SetExperience(experienceType, GetExperience(player, experienceType) + xp);
        }

        public static CharacterClass GetCharClassOrDefault(this IServerPlayer player)
        {
            if (player.GetSelectedClassCode() == null)
                return null;

            VSRoleplayClassesMod mod = player.Entity.World.Api.ModLoader.GetModSystem<VSRoleplayClassesMod>();

            var charClass = mod.GetCharacterClassesItems(player.GetSelectedClassCode());
            return charClass;
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
