using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace vsroleplayclasses.src.Gui
{
    public class GuiDialogCompass : GuiDialog
    {
        protected IInventory compassSlotInv;
        public override string ToggleKeyCombinationCode => "compass";

        public GuiDialogCompass(ICoreClientAPI capi) : base(capi)
        {

        }

        protected virtual void ComposeGuis()
        {
            this.compassSlotInv = this.capi.World.Player.InventoryManager.GetOwnInventory("compass");
            if (this.compassSlotInv == null)
                return;

            ElementBounds bounds1 = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bounds1.BothSizing = ElementSizing.FitToChildren;

            var direction = BlockFacing.HorizontalFromAngle(GameMath.Mod(((EntityPos)this.capi.World.Player.Entity.Pos).Yaw, 6.283185f)).ToString();

            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            double unscaledSlotPadding = GuiElementItemSlotGridBase.unscaledSlotPadding;
            ElementBounds slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 0.00, 20.0 + unscaledSlotPadding, 8, 1).FixedGrow(0.0, unscaledSlotPadding);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(slotBounds);
            ElementBounds line = ElementBounds.Fixed(60, 0, 350, 20);
            float width = 350;
            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("opencompass", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Compass", OnTitleBarCloseClicked)
                .BeginChildElements(bgBounds)
                    .AddStaticText($"Based on the sun you think you are facing {direction.ToUpper()}", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 0).WithFixedWidth(width))
                    .AddStaticText($"A compass in the compass slot will make this more accurate", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 0).WithFixedWidth(width))
                    .AddItemSlotGrid(this.compassSlotInv, new Action<object>(this.SendInvPacket), 1, new int[1] { 0 }, slotBounds, "compass")
                .EndChildElements()
                .Compose()
            ;
        }

        public override void OnGuiOpened()
        {
            this.ComposeGuis();

            if (this.compassSlotInv == null)
                return;

            if (this.capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Guest && this.capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Survival || this.compassSlotInv == null)
                return;

            this.compassSlotInv.Open((IPlayer)this.capi.World.Player);
        }

        public override void OnGuiClosed()
        {
            if (this.capi.World.Player.InventoryManager.GetOwnInventory("compass") != null)
            {
                this.compassSlotInv.Close((IPlayer)this.capi.World.Player);
                SingleComposer.GetSlotGrid("compass")?.OnGuiClosed(this.capi);
            }
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
