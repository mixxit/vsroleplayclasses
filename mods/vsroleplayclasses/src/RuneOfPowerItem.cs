using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace vsroleplayclasses.src
{
    public class RuneOfPowerItem : Item
    {
        public override void OnLoaded(ICoreAPI api)
        {
            if (api.Side != EnumAppSide.Client)
                return;

            ICoreClientAPI capi = api as ICoreClientAPI;
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            if (IsRuneScribed(inSlot.Itemstack))
            {
                string rune = GetScribedRune(inSlot.Itemstack);
                dsc.AppendLine(Lang.Get("Lingua Magica: {0}", rune.ToString()));
            }
            else
            {
                dsc.AppendLine(Lang.Get("Lingua Magica: {0}", "This rune has lost its power"));
            }

            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        }


        public bool IsRuneScribed(ItemStack itemStack)
        {
            return (!String.IsNullOrEmpty(GetScribedRune(itemStack)));
        }

        // Seed so client and server can match
        public void SetScribedRune(ItemStack itemStack, string scribedRune)
        {
            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetString("scribedRune", scribedRune);
                if (!itemStack.Attributes.HasAttribute("scribedRune"))
                    throw new Exception("This should not happen");
            }
        }

        internal string GetScribedRune(ItemStack itemStack)
        {
            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("scribedRune"))
                        return "";

                    return itemStack.Attributes.GetString("scribedRune", "");
                }
                catch (InvalidCastException)
                {

                    return "";
                }
            }
            return "";
        }

    }
}