using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Packets;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Gui
{
    public class HudMemorisedSpells : HudElement
    {
        private ConcurrentDictionary<int, MemorisedAbilityHudEntry> memorisedAbilities = new ConcurrentDictionary<int, MemorisedAbilityHudEntry>();

        //private GuiElementStatbar castingbar;

        public override double InputOrder => 1.0;

        public HudMemorisedSpells(ICoreClientAPI capi)
          : base(capi)
        {
            
        }
        public override string ToggleKeyCombinationCode => (string)null;

        public override void OnOwnPlayerDataReceived()
        {
            this.ComposeGuis();
        }

        public void UpdateHud(IPlayer player)
        {
            if (player?.Entity?.Api?.Side != EnumAppSide.Client)
                return;

            SystemAbilities mod = this.capi.World.Api.ModLoader.GetModSystem<SystemAbilities>();
            if (mod == null)
                return;

            mod.SendMemorisedAbilitiesHudUpdatePacketAsClient(player);
        }

        public ElementBounds GetSpellEffectIconImageBounds(int slotNumber)
        {
            var offsetHeight = slotNumber * 41.0f;
            var offsetWidth = 0.0f;

            return new ElementBounds()
            {
                Alignment = EnumDialogArea.RightTop,
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
                Alignment = EnumDialogArea.LeftTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = 85.0,
                fixedHeight = ((double)num)
            }.WithFixedAlignmentOffset(-5.0, 5.0);

            var effect1 = GetMemorisedHudAbility(1);
            var effect2 = GetMemorisedHudAbility(2);
            var effect3 = GetMemorisedHudAbility(3);
            var effect4 = GetMemorisedHudAbility(4);
            var effect5 = GetMemorisedHudAbility(5);
            var effect6 = GetMemorisedHudAbility(6);
            var effect7 = GetMemorisedHudAbility(7);
            var effect8 = GetMemorisedHudAbility(8);

            ElementBounds bounds3 = ElementStdBounds.Statbar(EnumDialogArea.LeftTop, (double)num * 0.41).WithFixedAlignmentOffset(-2.0, 5.0);
            ElementBounds line = ElementBounds.Fixed(40, 0, 40, 40);
            bounds3.WithFixedHeight(10.0);
            this.Composers["memorisedabilities"] = 
                this.capi.Gui.CreateCompo("memorisedabilities", bounds1.FlatCopy().FixedGrow(0.0, 20.0)).
                BeginChildElements(bounds1).
                AddIf(effect1 != null).AddImage(GetSpellEffectIconImageBounds(1), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect1?.Icon}.png")).AddStaticText("ALT-1", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(1)).AddTranspHoverText(effect1?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(1)).EndIf().
                AddIf(effect2 != null).AddImage(GetSpellEffectIconImageBounds(2), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect2?.Icon}.png")).AddStaticText("ALT-2", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(2)).AddTranspHoverText(effect2?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(2)).EndIf().
                AddIf(effect3 != null).AddImage(GetSpellEffectIconImageBounds(3), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect3?.Icon}.png")).AddStaticText("ALT-3", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(3)).AddTranspHoverText(effect3?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(3)).EndIf().
                AddIf(effect4 != null).AddImage(GetSpellEffectIconImageBounds(4), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect4?.Icon}.png")).AddStaticText("ALT-4", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(4)).AddTranspHoverText(effect4?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(4)).EndIf().
                AddIf(effect5 != null).AddImage(GetSpellEffectIconImageBounds(5), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect5?.Icon}.png")).AddStaticText("ALT-5", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(5)).AddTranspHoverText(effect5?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(5)).EndIf().
                AddIf(effect6 != null).AddImage(GetSpellEffectIconImageBounds(6), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect6?.Icon}.png")).AddStaticText("ALT-6", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(6)).AddTranspHoverText(effect6?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(6)).EndIf().
                AddIf(effect7 != null).AddImage(GetSpellEffectIconImageBounds(7), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect7?.Icon}.png")).AddStaticText("ALT-7", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(7)).AddTranspHoverText(effect7?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(7)).EndIf().
                AddIf(effect8 != null).AddImage(GetSpellEffectIconImageBounds(8), new AssetLocation("vsroleplayclasses", $"textures/gui/spells/{effect8?.Icon}.png")).AddStaticText("ALT-8", CairoFont.WhiteDetailText(), GetSpellEffectIconImageBounds(8)).AddTranspHoverText(effect8?.Name, CairoFont.WhiteDetailText(), 400, GetSpellEffectIconImageBounds(8)).EndIf().
                // AddStatbar(bounds3, new double[] { 0, 1, 0, 1 }, "castingstatbar").EndIf().
                EndChildElements().Compose();
            //this.castingbar = this.Composers["castingbar"].GetStatbar("castingstatbar");
            this.TryOpen();
        }

        internal void UpdateMemorisedAbilities(UpdateMemorisedSpellsPacket updateMemorisedSpellsPacket)
        {
            foreach (var dictionaryEntry in updateMemorisedSpellsPacket.memorisedAbilities)
            {
                this.memorisedAbilities[dictionaryEntry.Key] = new MemorisedAbilityHudEntry() { Icon = dictionaryEntry.Value.Item1, Name = dictionaryEntry.Value.Item2 };
            }
            this.ComposeGuis();
        }

        private MemorisedAbilityHudEntry GetMemorisedHudAbility(int slotId)
        {
            if (memorisedAbilities == null)
                return null;

            if (!memorisedAbilities.ContainsKey(slotId-1))
                return null;

            return memorisedAbilities[slotId - 1];
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
