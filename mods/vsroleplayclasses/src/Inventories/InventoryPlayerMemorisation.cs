﻿using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using vsroleplayclasses.src.ItemSlots;

namespace vsroleplayclasses.src.Inventories
{
    public class InventoryPlayerMemorisation : InventoryBasePlayer
    {
        private ItemSlot[] slots;

        public InventoryPlayerMemorisation(string className, string playerUID, ICoreAPI api)
          : base(className, playerUID, api)
        {
            this.slots = this.GenEmptySlots(8);
            this.baseWeight = 2.5f;
        }

        public InventoryPlayerMemorisation(string inventoryId, ICoreAPI api)
          : base(inventoryId, api)
        {
            this.slots = this.GenEmptySlots(8);
            this.baseWeight = 2.5f;
        }

        public override void OnItemSlotModified(ItemSlot slot) => base.OnItemSlotModified(slot);

        public override int Count => this.slots.Length;

        public override ItemSlot this[int slotId]
        {
            get => slotId < 0 || slotId >= this.Count ? (ItemSlot)null : this.slots[slotId];
            set
            {
                if (slotId < 0 || slotId >= this.Count)
                    throw new ArgumentOutOfRangeException(nameof(slotId));
                this.slots[slotId] = value != null ? value : throw new ArgumentNullException(nameof(value));
            }
        }

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            this.slots = this.SlotsFromTreeAttributes(tree);
            if (this.slots.Length != 8)
                return;

            if (this.slots.Length == 8)
            {
                ItemSlot[] slots = this.slots;
                this.slots = this.GenEmptySlots(8);
                for (int index = 0; index < slots.Length; ++index)
                    this.slots[index] = slots[index];
            }
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            this.SlotsToTreeAttributes(this.slots, tree);
            this.ResolveBlocksOrItems();
        }

        protected override ItemSlot NewSlot(int slotId)
        {
            return (ItemSlot)new ItemSlotScroll((InventoryBase)this);
        }

        public override void DiscardAll()
        {
        }

        public override void OnOwningEntityDeath(Vec3d pos)
        {
        }

        public override WeightedSlot GetBestSuitedSlot(
          ItemSlot sourceSlot,
          List<ItemSlot> skipSlots = null)
        {
            return new WeightedSlot();
        }
    }
}
