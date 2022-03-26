using Vintagestory.API.Common;

namespace vsroleplayclasses.src.ItemSlots
{
    public class ItemSlotCompass : ItemSlot
    {
        public override EnumItemStorageFlags StorageType { get { return EnumItemStorageFlags.Custom2; } }

        public ItemSlotCompass(InventoryBase inventory) : base(inventory)
        {
            this.BackgroundIcon = "compass";
        }

    }
}