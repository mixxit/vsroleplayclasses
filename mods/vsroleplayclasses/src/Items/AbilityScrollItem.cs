using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace vsroleplayclasses.src.Items
{
    public class AbilityScrollItem : Item
    {
        public override void OnLoaded(ICoreAPI api)
        {
            if (api.Side != EnumAppSide.Client)
                return;

            ICoreClientAPI capi = api as ICoreClientAPI;
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            if (IsAbilityScribed(inSlot.Itemstack))
            {
                long scribedAbility = GetScribedAbility(inSlot.Itemstack); // when deserialized json item it will default to long over int
                dsc.AppendLine(Lang.Get("Ability: {0}", scribedAbility.ToString()));
                return;
            }
            else
            {
                if (HasSpareRuneSlot(inSlot.Itemstack))
                {
                    dsc.AppendLine(Lang.Get("Ability: {0}", "This scroll is incomplete"));
                }
                else
                {
                    dsc.AppendLine(Lang.Get("Ability: {0}", "This scroll appears useless"));
                    return;
                }
            }

            var wordOfPower1 = GetWordOfPower(inSlot.Itemstack, 1);
            var wordOfPower2 = GetWordOfPower(inSlot.Itemstack, 2);
            var wordOfPower3 = GetWordOfPower(inSlot.Itemstack, 3);
            var wordOfPower4 = GetWordOfPower(inSlot.Itemstack, 4);

            if (wordOfPower1 != null)
                dsc.AppendLine(Lang.Get("Words of Power: {0}", LinguaMagica.WordOfPowerToLingaMagicaCaseInsensitive((MagicaPower)wordOfPower1)));
            if (wordOfPower2 != null)
                dsc.AppendLine(Lang.Get("Words of Power: {0}", LinguaMagica.WordOfPowerToLingaMagicaCaseInsensitive((MagicaPower)wordOfPower2)));
            if (wordOfPower3 != null)
                dsc.AppendLine(Lang.Get("Words of Power: {0}", LinguaMagica.WordOfPowerToLingaMagicaCaseInsensitive((MagicaPower)wordOfPower3)));
            if (wordOfPower4 != null)
                dsc.AppendLine(Lang.Get("Words of Power: {0}", LinguaMagica.WordOfPowerToLingaMagicaCaseInsensitive((MagicaPower)wordOfPower4)));
        }


        public bool IsAbilityScribed(ItemStack itemStack)
        {
            return (GetScribedAbility(itemStack) > 0);
        }

        // Seed so client and server can match
        public void SetScribedAbility(ItemStack itemStack, long scribedAbility)
        {
            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetLong("scribedAbility", scribedAbility); // when deserialized json item it will default to long over int
                if (!itemStack.Attributes.HasAttribute("scribedAbility"))
                    throw new Exception("This should not happen");
            }
        }

        public void SetWordOfPower(ItemStack itemStack, int slot, MagicaPower magicPower)
        {
            if (slot < 1 || slot > 4)
                throw new Exception("Invalid slot");

            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetString("wordOfPower_"+slot, magicPower.ToString());
                if (!itemStack.Attributes.HasAttribute("wordOfPower_"+slot))
                    throw new Exception("This should not happen");
            }
        }

        internal MagicaPower? GetWordOfPower(ItemStack itemStack, int slot)
        {
            if (slot < 1 || slot > 4)
                throw new Exception("Invalid slot");

            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("wordOfPower_"+slot))
                        return null;
                    var wordOfPower = itemStack.Attributes.GetString("wordOfPower_" + slot, null);
                    return RunicTools.GetWordOfPowerFromWordOfPowerString(wordOfPower);
                }
                catch (InvalidCastException)
                {

                    return null;
                }
            }
            return null;
        }
        
        internal long GetScribedAbility(ItemStack itemStack)
        {
            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("scribedAbility"))
                        return -1;

                    return itemStack.Attributes.GetLong("scribedAbility", -1); // when deserialized json item it will default to long over int
                }
                catch (InvalidCastException)
                {

                    return -1;
                }
            }
            return -1;
        }

        public List<MagicaPower> GetWordOfPowers(ItemStack itemStack)
        {
            List<MagicaPower> magicPowers = new List<MagicaPower>();
            for (int i = 1; i <= 4; i++)
            {
                if (GetWordOfPower(itemStack,i) != null)
                    magicPowers.Add((MagicaPower)GetWordOfPower(itemStack,i));
            }

            return magicPowers;
        }

        internal bool HasRunePower(ItemStack itemstack, MagicaPower wordOfPower)
        {
            return GetWordOfPowers(itemstack).Contains(wordOfPower);
        }

        public bool HasSpareRuneSlot(ItemStack itemstack)
        {
            return (GetWordOfPower(itemstack, 4) == null);
        }
    }
}