using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Items
{
    public class RunicInkwellAndQuillItem : Item
    {
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            // We set this up server side and let the attributes sync
            if (byEntity.Api.Side == EnumAppSide.Client)
            {
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }


        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            if (inSlot?.Itemstack?.Item != null && inSlot.Itemstack.ItemAttributes.KeyExists("runetype"))
            {
                var runeTypeText = "Unknown";

                switch (inSlot.Itemstack.ItemAttributes["runetype"].ToString())
                {
                    case "powerlevel":
                        runeTypeText = "Rune of Rank";
                        break;
                    case "targettype":
                        runeTypeText = "Rune of Seeking";
                        break;
                    case "resisttype":
                        runeTypeText = "Rune of Elements";
                        break;
                    case "adventureclass":
                        runeTypeText = "Rune of Adventure";
                        break;
                    case "spelleffectindex":
                        runeTypeText = "Rune of Modification";
                        break;
                    case "spelleffect":
                        runeTypeText = "Rune of Effect";
                        break;
                    default:
                        break;
                }

                dsc.AppendLine(Lang.Get("Rune Type: {0}", runeTypeText));
            }
        }
    }
}