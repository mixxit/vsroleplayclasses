using System;
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
            if (IsAbilityScribed(inSlot.Itemstack))
            {
                long scribedAbility = GetScribedAbility(inSlot.Itemstack); // when deserialized json item it will default to long over int
                dsc.AppendLine(Lang.Get("Ability: {0}", scribedAbility.ToString()));
            }
            else
            {
                dsc.AppendLine(Lang.Get("Ability: {0}", "Not Scribed"));
            }

            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
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
    }
}