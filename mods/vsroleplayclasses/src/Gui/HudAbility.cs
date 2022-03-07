using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using vsroleplayclasses.src.Behaviors;

namespace vsroleplayclasses.src.Gui
{
    public class HudAbility : HudElement
    {
        private float lastCastingpct;
        private GuiElementStatbar castingbar;

        public override double InputOrder => 1.0;

        public HudAbility(ICoreClientAPI capi)
          : base(capi)
        {
            capi.Event.RegisterGameTickListener(new Action<float>(this.OnGameTick), 500);
        }

        public override string ToggleKeyCombinationCode => (string)null;

        private void OnGameTick(float dt)
        {
            this.UpdateCastingPercent();
        }

        private void UpdateCastingPercent()
        {
            float? nullable1 = capi.World.Player.Entity.WatchedAttributes.GetFloat("castingpct", 0.0f); ;
            if (!nullable1.HasValue)
                return;
            if (this.castingbar == null)
                return;
            this.castingbar.SetLineInterval(1f);
            this.castingbar.SetValues(nullable1.Value, 0.0f, 1.0f);
            this.lastCastingpct = nullable1.Value;
        }

        public override void OnOwnPlayerDataReceived()
        {
            this.ComposeGuis();
            this.UpdateCastingPercent();
        }

        public void ComposeGuis()
        {
            float num = 850f;
            ElementBounds bounds1 = new ElementBounds()
            {
                Alignment = EnumDialogArea.CenterBottom,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = ((double)num),
                fixedHeight = 112.0
            }.WithFixedAlignmentOffset(0.0, 5.0);
            ElementBounds bounds3 = ElementStdBounds.Statbar(EnumDialogArea.RightTop, (double)num * 0.41).WithFixedAlignmentOffset(-2.0, 5.0);
            bounds3.WithFixedHeight(10.0);
            this.Composers["castingbar"] = this.capi.Gui.CreateCompo("inventory-castingbar", bounds1.FlatCopy().FixedGrow(0.0, 20.0)).BeginChildElements(bounds1).
                AddStatbar(bounds3, new double[] { 0, 1, 0, 1 }, "castingstatbar").EndIf().
                EndChildElements().Compose();
            this.castingbar = this.Composers["castingbar"].GetStatbar("castingstatbar");
            this.TryOpen();
        }

        public override bool TryClose() => false;

        public override bool ShouldReceiveKeyboardEvents() => false;

        public override void OnRenderGUI(float deltaTime)
        {
            if (this.capi.World.Player.WorldData.CurrentGameMode == EnumGameMode.Spectator)
                return;
            base.OnRenderGUI(deltaTime);
        }

        public override bool Focusable => false;

        protected override void OnFocusChanged(bool on)
        {
        }

        public override void OnMouseDown(MouseEvent args)
        {
        }
    }
}
