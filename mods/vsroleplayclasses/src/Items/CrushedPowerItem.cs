using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace vsroleplayclasses.src.Items
{
    public class CrushedPowerItem : Item
    {
        public override void OnLoaded(ICoreAPI api)
        {
            if (api.Side != EnumAppSide.Client)
                return;
        }
        internal MagicaPower GetScribedRune(ItemStack itemStack)
        {
            if (itemStack.Attributes != null)
            {
                
            }
            return MagicaPower.None;
        }

    }
}