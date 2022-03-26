using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace vsroleplayclasses.src.Gui.Hud
{
    public class HudManaBar : HudElement
    {
        private float lastMana;
        private float lastMaxMana;
        private GuiElementStatbar manabar;

        public override double InputOrder => 1.0;

        public HudManaBar(ICoreClientAPI capi)
          : base(capi)
        {
            capi.Event.RegisterGameTickListener(new Action<float>(this.OnGameTick), 20);
        }

        public override string ToggleKeyCombinationCode => (string)null;

        private void OnGameTick(float dt)
        {
            this.UpdateMana();
        }


        private void UpdateMana()
        {
            float? nullable1 = this.capi.World.Player.Entity.WatchedAttributes.TryGetFloat("currentmana");
            float? nullable2 = this.capi.World.Player.Entity.WatchedAttributes.TryGetFloat("maxmana");
            if (!nullable1.HasValue || !nullable2.HasValue)
                return;
            double lastMana = (double)this.lastMana;
            float? nullable3 = nullable1;
            double valueOrDefault1 = (double)nullable3.GetValueOrDefault();
            if (lastMana == valueOrDefault1 & nullable3.HasValue)
            {
                double lastMaxHealth = (double)this.lastMaxMana;
                float? nullable4 = nullable2;
                double valueOrDefault2 = (double)nullable4.GetValueOrDefault();
                if (lastMaxHealth == valueOrDefault2 & nullable4.HasValue)
                    return;
            }
            if (this.manabar == null)
                return;
            this.manabar.SetLineInterval(1f);
            this.manabar.SetValues(nullable1.Value, 0.0f, nullable2.Value);
            this.lastMana = nullable1.Value;
            this.lastMaxMana = nullable2.Value;
        }

        public override void OnOwnPlayerDataReceived()
        {
            this.ComposeGuis();
            this.UpdateMana();
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
            ElementBounds bounds2 = ElementStdBounds.Statbar(EnumDialogArea.LeftTop, (double)num * 0.41).WithFixedAlignmentOffset(1.0, 5.0);
            bounds2.WithFixedHeight(10.0);
            this.Composers["manabar"] = this.capi.Gui.CreateCompo("inventory-manabar", bounds1.FlatCopy().FixedGrow(0.0, 20.0)).BeginChildElements(bounds1).
                AddStatbar(bounds2, new double[] { 0, 0, 1, 1 }, "manastatbar").EndIf().
                EndChildElements().Compose();
            this.manabar = this.Composers["manabar"].GetStatbar("manastatbar");
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
