using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

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
            return new ItemSlotSurvival(this);
        }

        public override float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            if (targetSlot == slots[0] && sourceSlot.Itemstack.Collectible.GrindingProps != null) return 4f;

            return base.GetSuitability(sourceSlot, targetSlot, isMerge);
        }

        public override ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot)
        {
            return slots[0];
        }
    }
}
