using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace vsroleplayclasses.src.Gui.Dialog
{
    public class GuiDialogBlockEntityScribesTable : GuiDialogBlockEntity
    {
        long lastRedrawMs;
        float inputScribeTime;
        float maxScribeTime;

        protected override double FloatyDialogPosition => 0.75;

        public GuiDialogBlockEntityScribesTable(string DialogTitle, InventoryBase Inventory, BlockPos BlockEntityPosition, ICoreClientAPI capi)
            : base(DialogTitle, Inventory, BlockEntityPosition, capi)
        {
            if (IsDuplicate) return;

            capi.World.Player.InventoryManager.OpenInventory(Inventory);

            SetupDialog();
        }

        private void OnInventorySlotModified(int slotid)
        {
            // Direct call can cause InvalidOperationException
            capi.Event.EnqueueMainThreadTask(SetupDialog, "setupscribestabledlg");
        }

        void SetupDialog()
        {
            ItemSlot hoveredSlot = capi.World.Player.InventoryManager.CurrentHoveredSlot;
            if (hoveredSlot != null && hoveredSlot.Inventory == Inventory)
            {
                capi.Input.TriggerOnMouseLeaveSlot(hoveredSlot);
            }
            else
            {
                hoveredSlot = null;
            }

            ElementBounds scribestableBounds = ElementBounds.Fixed(0, 0, 200, 500);

            ElementBounds inputSlotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 10, 50, 1, 1);
            ElementBounds line = ElementBounds.Fixed(0, 0, 100, 30);
            ElementBounds outputLine = ElementBounds.Fixed(180, 180, 50, 30);
            ElementBounds outputSlotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 180, 200, 1, 1);

            ElementBounds helptop = ElementBounds.Fixed(100, 30, 150, 30);
            ElementBounds helpbottom = ElementBounds.Fixed(100, 260, 150, 30);


            // 2. Around all that is 10 pixel padding
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(scribestableBounds);

            // 3. Finally Dialog
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle)
                .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0);

            ClearComposers();
            SingleComposer = capi.Gui
                .CreateCompo("blockentityscribestable" + BlockEntityPosition, dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar(DialogTitle, OnTitleBarClose)
                .BeginChildElements(bgBounds)
                    // slot 0 = input - scroll
                    // slot 1 = input - resisttype
                    // slot 2 = input - spelleffect
                    // slot 3 = input - spelleffectindex
                    // slot 4 = input - targettype
                    // slot 5 = input - powerlevel
                    // slot 6 = input - adventureclass
                    // slot 7 = output
                    .AddStaticText("By crushing magical runes and soaking them in ink, a scribe can create new and unusual magic and combat abilities. A scribe must be focused on the table to finish the scroll", CairoFont.WhiteDetailText(), helptop.WithFixedWidth(150))
                    .AddStaticText("Scroll:", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 0).WithFixedWidth(50))
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 0 }, inputSlotBounds, "inputSlot0")
                    .AddStaticText("Elements:", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 45).WithFixedWidth(50))
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 1 }, inputSlotBounds = inputSlotBounds.BelowCopy(0, 25), "inputSlot1")
                    .AddStaticText("Effect:", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 45).WithFixedWidth(50))
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 2 }, inputSlotBounds = inputSlotBounds.BelowCopy(0, 25), "inputSlot2")
                    .AddStaticText("Modification:", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 45).WithFixedWidth(50))
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 3 }, inputSlotBounds = inputSlotBounds.BelowCopy(0, 25), "inputSlot3")
                    .AddStaticText("Seeking:", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 45).WithFixedWidth(50))
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 4 }, inputSlotBounds = inputSlotBounds.BelowCopy(0, 25), "inputSlot4")
                    .AddStaticText("Rank:", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 47).WithFixedWidth(50))
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 5 }, inputSlotBounds = inputSlotBounds.BelowCopy(0, 25), "inputSlot5")
                    .AddStaticText("Adventure:", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 47).WithFixedWidth(50))
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 6 }, inputSlotBounds = inputSlotBounds.BelowCopy(0, 25), "inputSlot6")
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 7 }, outputSlotBounds, "outputslot")
                    .AddStaticText("Output:", CairoFont.WhiteDetailText(), outputLine.WithFixedWidth(50))
                    .AddStaticText("The first scribe to discover a new ability will have the ability named after them. Abilities can be shared amongst players and can be memorised in the Memorisation Panel (L) and used with ALT-1 through ALT-8. Rune stones can be found via panning and on creatures", CairoFont.WhiteDetailText(), helpbottom.WithFixedWidth(150))
                    .AddDynamicCustomDraw(scribestableBounds, OnBgDraw, "symbolDrawer")
                .EndChildElements()
                .Compose()
            ;

            lastRedrawMs = capi.ElapsedMilliseconds;

            if (hoveredSlot != null)
            {
                SingleComposer.OnMouseMove(new MouseEvent(capi.Input.MouseX, capi.Input.MouseY));
            }
        }

        public void Update(float inputScribeTime, float maxScribeTime)
        {
            this.inputScribeTime = inputScribeTime;
            this.maxScribeTime = maxScribeTime;

            if (!IsOpened()) 
                return;

            if (capi.ElapsedMilliseconds - lastRedrawMs > 500)
            {
                if (SingleComposer != null) SingleComposer.GetCustomDraw("symbolDrawer").Redraw();
                lastRedrawMs = capi.ElapsedMilliseconds;
            }
        }



        private void OnBgDraw(Context ctx, ImageSurface surface, ElementBounds currentBounds)
        {
            double top = 200;

            // Arrow Right
            ctx.Save();
            Matrix m = ctx.Matrix;
            m.Translate(GuiElement.scaled(80), GuiElement.scaled(top + 2));
            m.Scale(GuiElement.scaled(0.6), GuiElement.scaled(0.6));
            ctx.Matrix = m;
            capi.Gui.Icons.DrawArrowRight(ctx, 2);

            double dx = inputScribeTime / maxScribeTime;


            ctx.Rectangle(GuiElement.scaled(5), 0, GuiElement.scaled(125 * dx), GuiElement.scaled(100));
            ctx.Clip();
            LinearGradient gradient = new LinearGradient(0, 0, GuiElement.scaled(200), 0);
            gradient.AddColorStop(0, new Color(0, 0.4, 0, 1));
            gradient.AddColorStop(1, new Color(0.2, 0.6, 0.2, 1));
            ctx.SetSource(gradient);
            capi.Gui.Icons.DrawArrowRight(ctx, 0, false, false);
            gradient.Dispose();
            ctx.Restore();
        }




        private void SendInvPacket(object p)
        {
            capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.Y, BlockEntityPosition.Z, p);
        }


        private void OnTitleBarClose()
        {
            TryClose();
        }


        public override void OnGuiOpened()
        {
            base.OnGuiOpened();
            Inventory.SlotModified += OnInventorySlotModified;

            
        }

        public override void OnGuiClosed()
        {
            Inventory.SlotModified -= OnInventorySlotModified;

            SingleComposer.GetSlotGrid("inputSlot0").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlot1").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlot2").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlot3").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlot4").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlot5").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("inputSlot6").OnGuiClosed(capi);
            SingleComposer.GetSlotGrid("outputslot").OnGuiClosed(capi);

            base.OnGuiClosed();
        }
    }
}
