using Vintagestory.API.Common;

namespace vsroleplayclasses.src.ItemSlots
{
    public class ItemSlotScroll : ItemSlot
    {
        public override EnumItemStorageFlags StorageType { get { return EnumItemStorageFlags.Custom1; } }

        public ItemSlotScroll(InventoryBase inventory) : base(inventory)
        {
            this.BackgroundIcon = "scroll";
        }

    }
}