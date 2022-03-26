using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Gui
{
    public class GuiDialogMemoriseAbility : GuiDialog
    {
        protected IInventory memorisedSlotsInv;
        public override string ToggleKeyCombinationCode => "memoriseability";

        public GuiDialogMemoriseAbility(ICoreClientAPI capi) : base(capi)
        {

        }

        protected virtual void ComposeGuis()
        {
            this.memorisedSlotsInv = this.capi.World.Player.InventoryManager.GetOwnInventory("memoriseability");
            if (this.memorisedSlotsInv == null)
                return;

            ElementBounds bounds1 = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bounds1.BothSizing = ElementSizing.FitToChildren;

            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            double unscaledSlotPadding = GuiElementItemSlotGridBase.unscaledSlotPadding;
            ElementBounds slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 20.0, 20.0 + unscaledSlotPadding, 8, 1).FixedGrow(0.0, unscaledSlotPadding);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(slotBounds);

            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("memoriseability", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Memorised Abilities", OnTitleBarCloseClicked)
                .AddItemSlotGrid(this.memorisedSlotsInv,new Action<object>(this.SendInvPacket), 8, new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 }, slotBounds, "spellslots")
                .Compose()
            ;
        }

        public override void OnGuiOpened()
        {
            this.ComposeGuis();

            if (this.memorisedSlotsInv == null)
                return;

            if (this.capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Guest && this.capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Survival || this.memorisedSlotsInv == null)
                return;

            this.memorisedSlotsInv.Open((IPlayer)this.capi.World.Player);
            UpdateHud(this.capi.World.Player);
        }

        public override void OnGuiClosed()
        {
            if (this.capi.World.Player.InventoryManager.GetOwnInventory("memoriseability") != null)
            {
                UpdateHud(this.capi.World.Player);
                this.memorisedSlotsInv.Close((IPlayer)this.capi.World.Player);
                SingleComposer.GetSlotGrid("spellslots")?.OnGuiClosed(this.capi);
            }
        }

        private void UpdateHud(IPlayer player)
        {
            if (player.Entity.Api.Side != EnumAppSide.Client)
                return;

            SystemAbilities mod = this.capi.World.Api.ModLoader.GetModSystem<SystemAbilities>();
            if (mod == null)
                return;

            mod.SendMemorisedAbilitiesHudUpdatePacketAsClient(player);
        }

        private void OnTitleBarCloseClicked()
        {
            TryClose();
        }

        protected void SendInvPacket(object packet)
        {
            capi.Network.SendPacketClient(packet);
        }
    }
}
