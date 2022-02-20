using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace vsroleplayclasses.src.Items
{
    public class AbilityBookItem : Item
    {
        internal long GetScribedAbility(ItemStack itemStack, int slotId)
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

        internal void SetScribedAbility(ItemStack itemStack, int slotId, long abilityId)
        {
            if (slotId < 1 || slotId > 8)
                return;

            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetLong("scribedAbility_" + slotId, abilityId);
                if (!itemStack.Attributes.HasAttribute("scribedAbility_" + slotId))
                    throw new Exception("This should not happen");
            }
        }
    }
}