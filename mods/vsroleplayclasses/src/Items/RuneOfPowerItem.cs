using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src.Items
{
    public class RuneOfPowerItem : Item
    {
        public override void OnLoaded(ICoreAPI api)
        {
            if (api.Side != EnumAppSide.Client)
                return;
        }

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

            if (inSlot?.Itemstack?.Item != null && inSlot?.Itemstack?.ItemAttributes != null && inSlot.Itemstack.ItemAttributes.KeyExists("runetype"))
            {
                var runeTypeText = "Unknown";
                var runeEffect = "Unknown";

                switch (inSlot.Itemstack.ItemAttributes["runetype"].ToString())
                {
                    case "powerlevel":
                        runeTypeText = "Rune of Rank";
                        runeEffect = RunicTools.GetWordOfPowerFromItemStackItemAttributes<PowerLevel>(inSlot.Itemstack).ToString();
                        break;
                    case "targettype":
                        runeTypeText = "Rune of Seeking";
                        runeEffect = RunicTools.GetWordOfPowerFromItemStackItemAttributes<TargetType>(inSlot.Itemstack).ToString();
                        break;
                    case "resisttype":
                        runeTypeText = "Rune of Elements";
                        runeEffect = RunicTools.GetWordOfPowerFromItemStackItemAttributes<ResistType>(inSlot.Itemstack).ToString();
                        break;
                    case "adventureclass":
                        runeTypeText = "Rune of Adventure";
                        runeEffect = RunicTools.GetWordOfPowerFromItemStackItemAttributes<AdventureClass>(inSlot.Itemstack).ToString();
                        break;
                    case "spelleffectindex":
                        runeTypeText = "Rune of Modification";
                        runeEffect = RunicTools.GetWordOfPowerFromItemStackItemAttributes<SpellEffectIndex>(inSlot.Itemstack).ToString();
                        break;
                    case "spelleffect":
                        runeTypeText = "Rune of Effect";
                        runeEffect = RunicTools.GetWordOfPowerFromItemStackItemAttributes<SpellEffectType>(inSlot.Itemstack).ToString();
                        break;
                    default:
                        break;
                }

                dsc.AppendLine(Lang.Get("Rune Type: {0}", runeTypeText));
                dsc.AppendLine(Lang.Get("Effect: {0}", runeEffect));
            }
        }
    }
}