using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Packets;

namespace vsroleplayclasses.src.Gui
{
    public class GuiDialogSkills : GuiDialog
    {

        public override string ToggleKeyCombinationCode => "skills";

        public GuiDialogSkills(ICoreClientAPI capi) : base(capi)
        {
            capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("pendingxp", (System.Action)(() => UpdateSkills()));
            foreach (AdventureClass SkillsType in Enum.GetValues(typeof(AdventureClass)))
            {
                if (SkillsType == AdventureClass.None)
                    continue;

                capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener(SkillsType.ToString().ToLower() + "xp", (System.Action)(() => UpdateSkills()));
            }
        }

        protected virtual void ComposeGuis()
        {
            ElementBounds bounds1 = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bounds1.BothSizing = ElementSizing.FitToChildren;

            // Auto-sized dialog at the center of the screen
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
            double unscaledSlotPadding = GuiElementItemSlotGridBase.unscaledSlotPadding;
            ElementBounds information = ElementBounds.Fixed(0, 30, 280, 20);
            ElementBounds line = ElementBounds.Fixed(0, 60, 280, -10);
            ElementBounds statbarBounds = ElementBounds.Fixed(0, 0, 280, -10);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(bounds1);
            float width = 600;
            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("openSkills", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Skills", OnTitleBarCloseClicked)
                .BeginChildElements(bgBounds)
                    .BeginChildElements(bounds1)
                    .AddDynamicText($"Skills are passive bonuses that affect mechanics", CairoFont.WhiteDetailText(), information.WithFixedSize(width+40, 40))

                    .AddStatbar(statbarBounds = information.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "offense")
                    .AddDynamicText($"Offense", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "offensetxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "defense")
                    .AddDynamicText($"Defense", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "defensetxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "throwing")
                    .AddDynamicText($"Throwing", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "throwingtxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "archery")
                    .AddDynamicText($"Archery", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "archerytxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "handtohand")
                    .AddDynamicText($"Hand to Hand", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "handtohandtxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "slashing")
                    .AddDynamicText($"Slashing", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "slashingtxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "piercing")
                    .AddDynamicText($"Piercing", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "piercingtxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "crushing")
                    .AddDynamicText($"Crushing", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "crushingtxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "twohandslashing")
                    .AddDynamicText($"Two Hand Slashing", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "twohandslashingtxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "twohandpiercing")
                    .AddDynamicText($"Two Hand Piercing", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "twohandpiercingtxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "twohandblunt")
                    .AddDynamicText($"Two Hand Blunt", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "twohandblunttxt")


                    .EndChildElements()
                .EndChildElements()
                .Compose()
            ;
            UpdateSkills();
        }

        public void UpdateSkills()
        {
            if (SingleComposer == null)
                return;

            var skills = capi.World.Player.Entity.GetSkillsValues();

            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                if (!skills.ContainsKey(skillType))
                    continue;

                UpdateSkillBar(skillType, skills[skillType].Item1, skills[skillType].Item2);
            }
        }

        public void UpdateSkillBar(SkillType skillType, double? xp, double maxxp)
        {
            if (xp == null)
                return;

            var bar = SingleComposer.GetStatbar(skillType.ToString().ToLower());
            if (bar == null)
                return;

            var txt = SingleComposer.GetDynamicText(skillType.ToString().ToLower()+"txt");
            if (txt == null)
                return;

            var percentage = PlayerUtils.GetPercent((double)xp, maxxp)/100f;
            bar.SetLineInterval(1f);
            bar.SetValues(percentage, 0.0f, 1.0f);
            txt.SetNewText($"{skillType.ToString()}: {(double)xp}/{(double)maxxp}");
        }

        public override void OnGuiOpened()
        {
            this.ComposeGuis();

            if (this.capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Guest && this.capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Survival)
                return;
        }

        public override void OnGuiClosed()
        {
        }

        private void OnTitleBarCloseClicked()
        {
            TryClose();
        }
    }
}
