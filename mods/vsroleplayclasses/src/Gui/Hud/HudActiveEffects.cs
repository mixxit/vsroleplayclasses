using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Packets;

namespace vsroleplayclasses.src.Gui.Hud
{
    public class HudActiveEffects : HudElement
    {
        private ConcurrentDictionary<int, ActiveSpellEffectHudEntry> effects = new ConcurrentDictionary<int, ActiveSpellEffectHudEntry>();

        //private GuiElementStatbar castingbar;

        public override double InputOrder => 1.0;

        public HudActiveEffects(ICoreClientAPI capi)
          : base(capi)
        {
            //capi.Event.RegisterGameTickListener(new Action<float>(this.OnGameTick), 100);
            capi.Network.GetChannel("updateactiveeffectshudpacket").SetMessageHandler<UpdateActiveEffectsHudPacket>(OnClientSideReceiveActiveEffectsHudPacket);
        }

        private void OnClientSideReceiveActiveEffectsHudPacket(UpdateActiveEffectsHudPacket networkMessage)
        {
            var activeSpellEffects = JsonConvert.DeserializeObject<List<ActiveSpellEffectHudEntry>>(networkMessage.SerializedActiveSpellEffectHudEntryList).ToArray();

            for (int i = 0; i < WorldLimits.MaxActiveEffectSlots; i++)
            {
                if (activeSpellEffects.ElementAtOrDefault(i) != null)
                    this.effects[i] = activeSpellEffects[i];
                else
                    this.effects.TryRemove(i, out _);
            }

            this.ComposeGuis();
        }

        public override string ToggleKeyCombinationCode => (string)null;

        public override void OnOwnPlayerDataReceived()
        {
            this.ComposeGuis();
            this.UpdateActiveEffects();
        }

        private void UpdateActiveEffects()
        {
            return;
        }

        public ElementBounds GetSpellEffectIconImageBounds(int slotNumber)
        {
            var offsetHeight = 0.0f;
            var offsetWidth = 0.0f + (slotNumber <= 8 ? slotNumber * 40.0f : (slotNumber-8)*40.0f);
            if (slotNumber > 8)
                offsetHeight = 40.0f;
            return new ElementBounds()
            {
                Alignment = EnumDialogArea.LeftTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = 40.0f,
                fixedHeight = 40.0f,
                fixedOffsetX = offsetWidth,
                fixedOffsetY = offsetHeight
            };
        }

        public void ComposeGuis()
        {
            float num = 360f;
            ElementBounds bounds1 = new ElementBounds()
            {
                Alignment = EnumDialogArea.RightTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = ((double)num),
                fixedHeight = 85.0
            }.WithFixedAlignmentOffset(-5.0, 5.0);

            var effect1 = GetActiveSpellEffect(1);
            var effect2 = GetActiveSpellEffect(2);
            var effect3 = GetActiveSpellEffect(3);
            var effect4 = GetActiveSpellEffect(4);
            var effect5 = GetActiveSpellEffect(5);
            var effect6 = GetActiveSpellEffect(6);
            var effect7 = GetActiveSpellEffect(7);
            var effect8 = GetActiveSpellEffect(8);
            var effect9 = GetActiveSpellEffect(9);
            var effect10 = GetActiveSpellEffect(10);
            var effect11 = GetActiveSpellEffect(11);
            var effect12 = GetActiveSpellEffect(12);
            var effect13 = GetActiveSpellEffect(13);
            var effect14 = GetActiveSpellEffect(14);
            var effect15 = GetActiveSpellEffect(15);
            var effect16 = GetActiveSpellEffect(16);

            ElementBounds bounds3 = ElementStdBounds.Statbar(EnumDialogArea.RightTop, (double)num * 0.41).WithFixedAlignmentOffset(-2.0, 5.0);
            ElementBounds line = ElementBounds.Fixed(40, 0, 40, 40);
            bounds3.WithFixedHeight(10.0);
            this.Composers["activeeffects"] = 
                this.capi.Gui.CreateCompo("activeeffects", bounds1.FlatCopy().FixedGrow(0.0, 20.0)).
                BeginChildElements(bounds1).
                AddIf(effect1 != null).AddImage(GetSpellEffectIconImageBounds(1), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect1?.Icon}.png")).AddTranspHoverText(effect1?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect2 != null).AddImage(GetSpellEffectIconImageBounds(2), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect2?.Icon}.png")).AddTranspHoverText(effect2?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect3 != null).AddImage(GetSpellEffectIconImageBounds(3), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect3?.Icon}.png")).AddTranspHoverText(effect3?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect4 != null).AddImage(GetSpellEffectIconImageBounds(4), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect4?.Icon}.png")).AddTranspHoverText(effect4?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect5 != null).AddImage(GetSpellEffectIconImageBounds(5), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect5?.Icon}.png")).AddTranspHoverText(effect5?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect6 != null).AddImage(GetSpellEffectIconImageBounds(6), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect6?.Icon}.png")).AddTranspHoverText(effect6?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect7 != null).AddImage(GetSpellEffectIconImageBounds(7), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect7?.Icon}.png")).AddTranspHoverText(effect7?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect8 != null).AddImage(GetSpellEffectIconImageBounds(8), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect8?.Icon}.png")).AddTranspHoverText(effect8?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect9 != null).AddImage(GetSpellEffectIconImageBounds(9), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect9?.Icon}.png")).AddTranspHoverText(effect9?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect10 != null).AddImage(GetSpellEffectIconImageBounds(10), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect10?.Icon}.png")).AddTranspHoverText(effect10?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect11 != null).AddImage(GetSpellEffectIconImageBounds(11), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect11?.Icon}.png")).AddTranspHoverText(effect11?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect12 != null).AddImage(GetSpellEffectIconImageBounds(12), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect12?.Icon}.png")).AddTranspHoverText(effect12?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect13 != null).AddImage(GetSpellEffectIconImageBounds(13), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect13?.Icon}.png")).AddTranspHoverText(effect13?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect14 != null).AddImage(GetSpellEffectIconImageBounds(14), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect14?.Icon}.png")).AddTranspHoverText(effect14?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect15 != null).AddImage(GetSpellEffectIconImageBounds(15), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect15?.Icon}.png")).AddTranspHoverText(effect15?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                AddIf(effect16 != null).AddImage(GetSpellEffectIconImageBounds(16), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect16?.Icon}.png")).AddTranspHoverText(effect16?.Name, CairoFont.WhiteDetailText(), 200, line).EndIf().
                // AddStatbar(bounds3, new double[] { 0, 1, 0, 1 }, "castingstatbar").EndIf().
                EndChildElements().Compose();
            //this.castingbar = this.Composers["castingbar"].GetStatbar("castingstatbar");
            this.TryOpen();
        }

        private ActiveSpellEffectHudEntry GetActiveSpellEffect(int slotId)
        {
            if (effects == null)
                return null;

            if (!effects.ContainsKey(slotId-1))
                return null;

            return effects[slotId - 1];
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
