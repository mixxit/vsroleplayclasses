using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using vsroleplayclasses.src.Items;

namespace vsroleplayclasses.src.Inventories
{
    public class ItemSlotScribesTable : ItemSlot
    {
        InventoryScribesTable inv;
        ItemSlotScribesTableTypeEnum slotType;

        public ItemSlotScribesTable(InventoryScribesTable inventory, ItemSlotScribesTableTypeEnum slotType) : base(inventory)
        {
            this.inv = inventory;
            this.slotType = slotType;
        }

        public override bool CanHold(ItemSlot itemstackFromSourceSlot)
        {
            if (itemstackFromSourceSlot?.Itemstack?.Item is AbilityScrollItem && ((AbilityScrollItem)itemstackFromSourceSlot?.Itemstack.Item).GetScribedAbilityId(itemstackFromSourceSlot?.Itemstack) < 0 && slotType.Equals(ItemSlotScribesTableTypeEnum.InputScroll))
                return true;

            if (itemstackFromSourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && itemstackFromSourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                itemstackFromSourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("resisttype") && slotType.Equals(ItemSlotScribesTableTypeEnum.ResistType))
                return true;

            if (itemstackFromSourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && itemstackFromSourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                itemstackFromSourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("spelleffect") && slotType.Equals(ItemSlotScribesTableTypeEnum.SpellEffect))
                return true;

            if (itemstackFromSourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && itemstackFromSourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                itemstackFromSourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("spelleffectindex") && slotType.Equals(ItemSlotScribesTableTypeEnum.SpellEffectIndex))
                return true;

            if (itemstackFromSourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && itemstackFromSourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                itemstackFromSourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("targettype") && slotType.Equals(ItemSlotScribesTableTypeEnum.TargetType))
                return true;

            if (itemstackFromSourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && itemstackFromSourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                itemstackFromSourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("powerlevel") && slotType.Equals(ItemSlotScribesTableTypeEnum.PowerLevel))
                return true;

            if (itemstackFromSourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && itemstackFromSourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                itemstackFromSourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("adventureclass") && slotType.Equals(ItemSlotScribesTableTypeEnum.AdventureClass))
                return true;

            if (itemstackFromSourceSlot?.Itemstack?.Item is AbilityScrollItem && ((AbilityScrollItem)itemstackFromSourceSlot?.Itemstack.Item).GetScribedAbilityId(itemstackFromSourceSlot?.Itemstack) > 0 && slotType.Equals(ItemSlotScribesTableTypeEnum.OutputScroll))
                return true;

            return false;
        }
    }
}
