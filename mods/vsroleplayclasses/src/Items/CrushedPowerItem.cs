using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src.Items
{
    public class CrushedPowerItem : Item
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


            var entity = byEntity.Api.World.BlockAccessor.GetBlockEntity(blockSel.Position);
            if (!(entity is BlockEntityAnvil))
            {
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            if (!slot.Itemstack.ItemAttributes.KeyExists("runecode"))
            {
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            var runeCode = slot.Itemstack.ItemAttributes["runecode"].ToString();

            // apply to workitemstack
            if (((BlockEntityAnvil)entity).WorkItemStack != null)
            {
                if (!((BlockEntityAnvil)entity).WorkItemStack.Attributes.HasAttribute("rune1"))
                {
                    ((BlockEntityAnvil)entity).WorkItemStack.Attributes.SetString("rune1", runeCode);
                    if (byEntity is EntityPlayer)
                        byEntity.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Rune applied successfully", EnumChatType.CommandSuccess);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }
                else if (!((BlockEntityAnvil)entity).WorkItemStack.Attributes.HasAttribute("rune2"))
                {
                    ((BlockEntityAnvil)entity).WorkItemStack.Attributes.SetString("rune2", runeCode);
                    if (byEntity is EntityPlayer)
                        byEntity.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Rune applied successfully", EnumChatType.CommandSuccess);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }
                else if (!((BlockEntityAnvil)entity).WorkItemStack.Attributes.HasAttribute("rune3"))
                {
                    ((BlockEntityAnvil)entity).WorkItemStack.Attributes.SetString("rune3", runeCode);
                    if (byEntity is EntityPlayer)
                        byEntity.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Rune applied successfully", EnumChatType.CommandSuccess);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }
                else if (!((BlockEntityAnvil)entity).WorkItemStack.Attributes.HasAttribute("rune4"))
                {
                    ((BlockEntityAnvil)entity).WorkItemStack.Attributes.SetString("rune4", runeCode);
                    if (byEntity is EntityPlayer)
                        byEntity.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "Rune applied successfully", EnumChatType.CommandSuccess);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }
                else
                {
                    if (byEntity is EntityPlayer)
                        byEntity.GetAsIServerPlayer().SendMessage(GlobalConstants.CurrentChatGroup, "There are no available rune slots left on this item", EnumChatType.CommandError);

                    handling = EnumHandHandling.PreventDefault;
                    return;
                }
            }

            handling = EnumHandHandling.PreventDefault;
            return;
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
                        var targetType = RunicTools.GetWordOfPowerFromItemStackItemAttributes<TargetType>(inSlot.Itemstack).ToString();
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