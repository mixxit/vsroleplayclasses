using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src;

namespace projectrarahat.src.Extensions
{
    public static class IServerPlayerExt
    {
        public static bool IsGrantedInitialItems(this IServerPlayer player)
        {
            if (!player.Entity.Attributes.HasAttribute("grantedInitialItems"))
                return false;

            return player.Entity.Attributes.GetBool("grantedInitialItems", false);
        }

        public static void SetGrantedInitialItems(this IServerPlayer player, bool granted)
        {
            player.Entity.Attributes.SetBool("grantedInitialItems", granted);
        }

        public static void GrantInitialClassItems(this IServerPlayer player)
        {
            if (player.GetSelectedClassCode() == null)
                return;

            VSRoleplayClassesMod mod = player.Entity.World.Api.ModLoader.GetModSystem<VSRoleplayClassesMod>();

            var charClass = mod.GetCharacterClassesItems(player.GetSelectedClassCode());
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

                if (itemstack.IsClothing())
                    continue;

                gear.Add(jsonItemStack);
            }

            player.DeliverItems(gear);
            player.SetGrantedInitialItems(true);
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
