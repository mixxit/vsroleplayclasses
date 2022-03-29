﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using vsroleplayclasses.src.Behaviors;

namespace vsroleplayclasses.src.Gui.Hud
{
    public class HudCastingBar : HudElement
    {
        private GuiElementStatbar castingbar;
        private GuiElementStaticText castingstatbartxt;
        private bool hideStatBar = true;
        private string currentAbilityName = "";

        public override double InputOrder => 1.0;

        public HudCastingBar(ICoreClientAPI capi)
          : base(capi)
        {
            capi.Event.RegisterGameTickListener(new Action<float>(this.OnGameTick), 100);
            capi.Event.RegisterGameTickListener(new Action<float>(this.OnFlashStatbar), 1000);
        }

        public override string ToggleKeyCombinationCode => (string)null;

        private void OnGameTick(float dt)
        {
            this.UpdateCastingPercent();
        }

        private void OnFlashStatbar(float dt)
        {
            if (this.castingbar == null)
                return;

            if (capi.World.Player.Entity.WatchedAttributes.GetLong("finishCastingUnixTime") < 1)
                return;

            if (capi.World.Player.Entity.WatchedAttributes.GetLong("finishCastingUnixTime") <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
            {
                this.castingbar.ShouldFlash = true;
                currentAbilityName = "Ready to Cast - Right Click";
                if (!castingstatbartxt.GetText().Equals(currentAbilityName))
                {
                    ComposeGuis();
                }
            }
        }

        private void UpdateCastingPercent()
        {
            if (capi.World.Player.Entity.WatchedAttributes.GetLong("finishCastingUnixTime") < 1 && this.castingbar?.GetValue() <= 0.0F)
            {
                return;
            }

            if (hideStatBar == true && this?.castingbar != null)
            {
                ComposeGuis();
                return;
            }

            if ((this?.castingbar?.GetValue() <= 0.0F || this.castingbar?.GetValue() >= 1.0F) && 
                capi.World.Player.Entity.WatchedAttributes.GetLong("finishCastingUnixTime") > DateTimeOffset.Now.ToUnixTimeMilliseconds() ||
                capi.World.Player.Entity.WatchedAttributes.GetLong("startCastingUnixTime") == 0
                )
            {
                if (hideStatBar != true)
                {
                    hideStatBar = true;
                }
                return;
            }

            if (!this.currentAbilityName.Equals(capi.World.Player.Entity.WatchedAttributes.GetString("startCastingAbilityName", "")))
                this.currentAbilityName = capi.World.Player.Entity.WatchedAttributes.GetString("startCastingAbilityName", "");

            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var totalTime = capi.World.Player.Entity.WatchedAttributes.GetLong("finishCastingUnixTime") - capi.World.Player.Entity.WatchedAttributes.GetLong("startCastingUnixTime");
            var progress = now - capi.World.Player.Entity.WatchedAttributes.GetLong("startCastingUnixTime");
            float percentage = (float)((double)progress / (double)totalTime);

            if (hideStatBar == true && this?.castingbar == null && percentage > 0.0f && percentage < 1.0f)
            {
                hideStatBar = false;
                ComposeGuis();
                this.castingbar.SetLineInterval(1f);
                this.castingbar.SetValues(percentage, 0.0f, 1.0f);
                
                return;
            }

            if (this.castingbar == null)
                return;
            this.castingbar.SetLineInterval(1f);
            this.castingbar.SetValues(percentage, 0.0f, 1.0f);
        }

        public override void OnOwnPlayerDataReceived()
        {
            this.ComposeGuis();
            this.UpdateCastingPercent();
        }

        public void ComposeGuis()
        {
            float num = 850f;
            ElementBounds bg = new ElementBounds()
            {
                Alignment = EnumDialogArea.LeftTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = 400,
                fixedHeight = 100
            }.WithFixedAlignmentOffset(40, -20);

            ElementBounds bounds1 = new ElementBounds()
            {
                Alignment = EnumDialogArea.LeftTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = ((double)num),
                fixedHeight = 112.0
            }.WithFixedAlignmentOffset(60.0, 40.0);
            ElementBounds bounds3 = ElementStdBounds.Statbar(EnumDialogArea.LeftTop, (double)num * 0.41).WithFixedAlignmentOffset(-2.0, 5.0);
            bounds3.WithFixedHeight(10.0);
            this.Composers["castingbar"] = this.capi.Gui.CreateCompo("inventory-castingbar", bounds1.FlatCopy().FixedGrow(0.0, 20.0)).
                AddIf(this.hideStatBar == false).
                AddShadedDialogBG(bg, true).
                BeginChildElements(bounds1).
                AddStatbar(bounds3, new double[] { 0, 1, 0, 1 }, "castingstatbar").
                AddStaticText($"{this.currentAbilityName}", CairoFont.WhiteDetailText(), bounds3.BelowCopy(0, -30).WithFixedWidth(num), "castingstatbartxt").
                EndChildElements().
                EndIf().
                Compose();
            this.castingbar = this.Composers["castingbar"].GetStatbar("castingstatbar");
            this.castingstatbartxt = this.Composers["castingbar"].GetStaticText("castingstatbartxt");
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
