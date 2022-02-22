using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
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

            IServerPlayer player = (IServerPlayer)(byEntity as EntityPlayer).Player;

            if (slot?.Itemstack?.Item == null || (!(slot?.Itemstack?.Item is RunicInkwellAndQuillItem)))
            {
                player.SendMessage(GlobalConstants.CurrentChatGroup, $"Missing items to scribe scroll", EnumChatType.CommandError);
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            if (
                player.Entity.RightHandItemSlot == null || player.Entity.RightHandItemSlot.Itemstack == null ||
                player.Entity.LeftHandItemSlot == null || player.Entity.LeftHandItemSlot.Itemstack == null
                )
            {
                player.SendMessage(GlobalConstants.CurrentChatGroup, $"No scroll in offhand", EnumChatType.CommandError);
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            if (
                !(player.Entity.LeftHandItemSlot.Itemstack.Item is AbilityScrollItem)
                )
            {
                player.SendMessage(GlobalConstants.CurrentChatGroup, $"No scroll in offhand", EnumChatType.CommandError);
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            var magicaPower = RunicTools.GetWordOfPowerFromQuillItem((RunicInkwellAndQuillItem)slot.Itemstack.Item);
            if (magicaPower == null)
            {
                player.SendMessage(GlobalConstants.CurrentChatGroup, $"Cannot find runic ink and quill", EnumChatType.CommandError);
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            if (TryApplyRuneToScroll(player, player.Entity.LeftHandItemSlot.Itemstack, (MagicaPower)magicaPower))
            {
                TryTransitionToAbility(player, player.Entity.LeftHandItemSlot.Itemstack);

                api.World.PlaySoundAt(new AssetLocation("sounds/tool/padlock.ogg"), player, player, false, 12);
                slot.TakeOut(1);
                slot.MarkDirty();
                player.Entity.LeftHandItemSlot.MarkDirty();

                player.SendMessage(GlobalConstants.CurrentChatGroup, $"Scribed", EnumChatType.CommandSuccess);
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }

        private bool TryTransitionToAbility(IServerPlayer player, ItemStack itemstack)
        {
            if (itemstack == null)
                return false;

            if (!(itemstack.Item is AbilityScrollItem))
                return false;

            if (((AbilityScrollItem)itemstack.Item).IsAbilityScribed(itemstack))
                return false;

            if (((AbilityScrollItem)itemstack.Item).HasSpareRuneSlot(itemstack))
                return false;

            SystemAbilities mod = player.Entity.World.Api.ModLoader.GetModSystem<SystemAbilities>();
            long abilityId = mod.TryCreateAbility(player, itemstack);
            if (abilityId > 0)
            {
                ((AbilityScrollItem)itemstack.Item).SetScribedAbility(itemstack, abilityId);
                return true;
            }

            return false;
        }

        private bool TryApplyRuneToScroll(IServerPlayer player, ItemStack itemstack, MagicaPower wordOfPower)
        {
            if (player == null || itemstack == null)
                return false;

            if (!(itemstack.Item is AbilityScrollItem))
                return false;

            if (((AbilityScrollItem)itemstack.Item).IsAbilityScribed(itemstack))
                return false;

            if (!((AbilityScrollItem)itemstack.Item).HasSpareRuneSlot(itemstack))
                return false;

            if (((AbilityScrollItem)itemstack.Item).HasRunePower(itemstack, wordOfPower))
                return false;

            if (((AbilityScrollItem)itemstack.Item).GetWordOfPower(itemstack, 1) == null)
            {
                ((AbilityScrollItem)itemstack.Item).SetWordOfPower(itemstack, 1, wordOfPower);
                return true;
            }

            if (((AbilityScrollItem)itemstack.Item).GetWordOfPower(itemstack, 2) == null)
            {
                ((AbilityScrollItem)itemstack.Item).SetWordOfPower(itemstack, 2, wordOfPower);
                return true;
            }

            if (((AbilityScrollItem)itemstack.Item).GetWordOfPower(itemstack, 3) == null)
            {
                ((AbilityScrollItem)itemstack.Item).SetWordOfPower(itemstack, 3, wordOfPower);
                return true;
            }

            if (((AbilityScrollItem)itemstack.Item).GetWordOfPower(itemstack, 4) == null)
            {
                ((AbilityScrollItem)itemstack.Item).SetWordOfPower(itemstack, 4, wordOfPower);
                return true;
            }

            return false;
        }
    }
}