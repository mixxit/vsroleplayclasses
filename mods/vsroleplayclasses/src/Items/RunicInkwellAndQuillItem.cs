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

            var spellEffectIndex = RunicTools.GetWordOfPowerFromQuillItem<SpellEffectIndex>((RunicInkwellAndQuillItem)slot.Itemstack.Item);
            var spellEffectType = RunicTools.GetWordOfPowerFromQuillItem<SpellEffectType>((RunicInkwellAndQuillItem)slot.Itemstack.Item);
            var targetType = RunicTools.GetWordOfPowerFromQuillItem<TargetType>((RunicInkwellAndQuillItem)slot.Itemstack.Item);
            var resistType = RunicTools.GetWordOfPowerFromQuillItem<ResistType>((RunicInkwellAndQuillItem)slot.Itemstack.Item);
            var spellPolarity = RunicTools.GetWordOfPowerFromQuillItem<SpellPolarity>((RunicInkwellAndQuillItem)slot.Itemstack.Item);
            var powerLevel = RunicTools.GetWordOfPowerFromQuillItem<PowerLevel>((RunicInkwellAndQuillItem)slot.Itemstack.Item);

            if (
                spellEffectIndex == SpellEffectIndex.None &&
                spellEffectType == SpellEffectType.None &&
                targetType == TargetType.None &&
                resistType == ResistType.None &&
                spellPolarity == SpellPolarity.None &&
                powerLevel == PowerLevel.None
                )
            {
                player.SendMessage(GlobalConstants.CurrentChatGroup, $"Cannot find runic ink and quill", EnumChatType.CommandError);
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            if (spellEffectIndex != SpellEffectIndex.None)
                if (TryApplyRuneToScroll(player, player.Entity.LeftHandItemSlot.Itemstack, spellEffectIndex))
                {
                    Consume(player, slot);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }

            if (spellEffectType != SpellEffectType.None)
                if (TryApplyRuneToScroll(player, player.Entity.LeftHandItemSlot.Itemstack, spellEffectType))
                {
                    Consume(player, slot);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }

            if (targetType != TargetType.None)
                if (TryApplyRuneToScroll(player, player.Entity.LeftHandItemSlot.Itemstack, targetType))
                {
                    Consume(player, slot);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }

            if (resistType != ResistType.None)
                if (TryApplyRuneToScroll(player, player.Entity.LeftHandItemSlot.Itemstack, resistType))
                {
                    Consume(player, slot);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }

            if (spellPolarity != SpellPolarity.None)
                if (TryApplyRuneToScroll(player, player.Entity.LeftHandItemSlot.Itemstack, spellPolarity))
                {
                    Consume(player, slot);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }

            if (powerLevel != PowerLevel.None)
                if (TryApplyRuneToScroll(player, player.Entity.LeftHandItemSlot.Itemstack, powerLevel))
                {
                    Consume(player, slot);
                    handling = EnumHandHandling.PreventDefault;
                    return;
                }

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }

        private void Consume(IServerPlayer player, ItemSlot slot)
        {
            TryTransitionToAbility(player, player.Entity.LeftHandItemSlot.Itemstack);

            api.World.PlaySoundAt(new AssetLocation("sounds/tool/padlock.ogg"), player, player, false, 12);
            slot.TakeOut(1);
            slot.MarkDirty();
            player.Entity.LeftHandItemSlot.MarkDirty();

            player.SendMessage(GlobalConstants.CurrentChatGroup, $"Scribed", EnumChatType.CommandSuccess);
            
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

        private bool TryApplyRuneToScroll<T>(IServerPlayer player, ItemStack itemstack, T wordOfPower) where T : Enum
        {
            if (player == null || itemstack == null)
                return false;

            if (!(itemstack.Item is AbilityScrollItem))
                return false;

            if (((AbilityScrollItem)itemstack.Item).IsAbilityScribed(itemstack))
                return false;

            if (!((AbilityScrollItem)itemstack.Item).HasSpareRuneSlot(itemstack))
                return false;

            if (((AbilityScrollItem)itemstack.Item).HasRunePower<T>(itemstack))
                return false;

            if (Object.Equals(((AbilityScrollItem)itemstack.Item).GetWordOfPower<T>(itemstack), default(T)))
            {
                ((AbilityScrollItem)itemstack.Item).SetWordOfPower<T>(itemstack, wordOfPower);
                return true;
            }
            return false;
        }
    }
}