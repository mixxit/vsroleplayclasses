using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using vsroleplayclasses.src.Items;

namespace vsroleplayclasses.src.Inventories
{
    /// <summary>
    /// Inventory with one normal slot and one output slot
    /// </summary>
    public class InventoryScribesTable : InventoryBase, ISlotProvider
    {
        ItemSlot[] slots;
        public ItemSlot[] Slots { get { return slots; } }


        public InventoryScribesTable(string inventoryID, ICoreAPI api) : base(inventoryID, api)
        {
            // slot 0 = input - scroll
            // slot 1 = input - resisttype
            // slot 2 = input - spelleffect
            // slot 3 = input - spelleffectindex
            // slot 4 = input - targettype
            // slot 5 = input - powerlevel
            // slot 6 = input - adventureclass
            // slot 7 = output
            slots = GenEmptySlots(8);
        }

        public InventoryScribesTable(string className, string instanceID, ICoreAPI api) : base(className, instanceID, api)
        {
            slots = GenEmptySlots(8);
        }


        public override int Count
        {
            get { return 8; }
        }

        public override ItemSlot this[int slotId]
        {
            get
            {
                if (slotId < 0 || slotId >= Count) return null;
                return slots[slotId];
            }
            set
            {
                if (slotId < 0 || slotId >= Count) throw new ArgumentOutOfRangeException(nameof(slotId));
                if (value == null) throw new ArgumentNullException(nameof(value));
                slots[slotId] = value;
            }
        }


        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            slots = SlotsFromTreeAttributes(tree, slots);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            SlotsToTreeAttributes(slots, tree);
        }

        protected override ItemSlot NewSlot(int i)
        {
            // slot 0 = input - scroll
            // slot 1 = input - resisttype
            // slot 2 = input - spelleffect
            // slot 3 = input - spelleffectindex
            // slot 4 = input - targettype
            // slot 5 = input - powerlevel
            // slot 6 = input - adventureclass
            // slot 7 = output

            ItemSlotScribesTableTypeEnum? slotType = null;
            switch(i)
            {
                case 0:
                    slotType = ItemSlotScribesTableTypeEnum.InputScroll;
                    break;
                case 1:
                    slotType = ItemSlotScribesTableTypeEnum.ResistType;
                    break;
                case 2:
                    slotType = ItemSlotScribesTableTypeEnum.SpellEffect;
                    break;
                case 3:
                    slotType = ItemSlotScribesTableTypeEnum.SpellEffectIndex;
                    break;
                case 4:
                    slotType = ItemSlotScribesTableTypeEnum.TargetType;
                    break;
                case 5:
                    slotType = ItemSlotScribesTableTypeEnum.PowerLevel;
                    break;
                case 6:
                    slotType = ItemSlotScribesTableTypeEnum.AdventureClass;
                    break;
                case 7:
                    slotType = ItemSlotScribesTableTypeEnum.OutputScroll;
                    break;
            }

            if (slotType == null)
                throw new Exception("Wrong index for Scribes Slot Type");



            return new ItemSlotScribesTable(this, (ItemSlotScribesTableTypeEnum)slotType);
        }

        public override float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            // slot 0 = input - scroll
            // slot 1 = input - resisttype
            // slot 2 = input - spelleffect
            // slot 3 = input - spelleffectindex
            // slot 4 = input - targettype
            // slot 5 = input - powerlevel
            // slot 6 = input - adventureclass
            // slot 7 = output

            if (targetSlot == slots[0] && sourceSlot?.Itemstack?.Item is AbilityScrollItem && ((AbilityScrollItem)sourceSlot?.Itemstack.Item).GetScribedAbilityId(sourceSlot?.Itemstack) > 0)
                return 4f;

            if (targetSlot == slots[1] && sourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && sourceSlot?.Itemstack?.ItemAttributes != null && sourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                sourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("resisttype")) 
                return 4f;
            if (targetSlot == slots[2] && sourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && sourceSlot?.Itemstack?.ItemAttributes != null && sourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                sourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("spelleffect"))
                return 4f;
            if (targetSlot == slots[3] && sourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && sourceSlot?.Itemstack?.ItemAttributes != null && sourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                sourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("spelleffectindex"))
                return 4f;
            if (targetSlot == slots[4] && sourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && sourceSlot?.Itemstack?.ItemAttributes != null && sourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                sourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("targettype"))
                return 4f;
            if (targetSlot == slots[5] && sourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && sourceSlot?.Itemstack?.ItemAttributes != null && sourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                sourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("powerlevel"))
                return 4f;
            if (targetSlot == slots[6] && sourceSlot?.Itemstack?.Item is RunicInkwellAndQuillItem && sourceSlot?.Itemstack?.ItemAttributes != null && sourceSlot.Itemstack.ItemAttributes.KeyExists("runetype") &&
                sourceSlot.Itemstack.ItemAttributes["runetype"].ToString().Equals("adventureclass"))
                return 4f;
            if (targetSlot == slots[7] && sourceSlot?.Itemstack?.Item is AbilityScrollItem && ((AbilityScrollItem)sourceSlot?.Itemstack.Item).GetScribedAbilityId(sourceSlot?.Itemstack) > 0)
                return 4f;

            return 0f;
        }
    }
}
