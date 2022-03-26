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
    public class GuiDialogExperience : GuiDialog
    {

        public override string ToggleKeyCombinationCode => "experience";

        public GuiDialogExperience(ICoreClientAPI capi) : base(capi)
        {
            capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("pendingxp", (System.Action)(() => UpdateExperience()));
            foreach (AdventureClass experienceType in Enum.GetValues(typeof(AdventureClass)))
            {
                if (experienceType == AdventureClass.None)
                    continue;

                capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener(experienceType.ToString().ToLower() + "xp", (System.Action)(() => UpdateExperience()));
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
            ElementBounds line = ElementBounds.Fixed(0, 0, 280, -10);
            ElementBounds statbarBounds = ElementBounds.Fixed(0, 0, 280, -10);

            // Background boundaries. Again, just make it fit it's child elements, then add the text as a child element
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(bounds1);
            float width = 600;
            // Lastly, create the dialog
            SingleComposer = capi.Gui.CreateCompo("openExperience", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Experience", OnTitleBarCloseClicked)
                .BeginChildElements(bgBounds)
                    .BeginChildElements(bounds1)
                    .AddDynamicText($"Spend your unassigned experience to raise your adventure level", CairoFont.WhiteDetailText(), information.WithFixedSize(width+40, 40))

                    .AddStatbar(statbarBounds = information.BelowCopy(0, 0).WithFixedSize(width, 20), new double[] { 0, 1, 1, 0.5 }, "pendingxpbar")
                    .AddDynamicText($"Unassigned Experience XP out of Max XP", CairoFont.WhiteDetailText(), line = information.BelowCopy(0, -16).WithFixedSize(width, 20), "pendingxpbartxt")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarwarrior")
                    .AddDynamicText($"Level X Warrior: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarwarriortxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Warrior); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarwarriorbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarcleric")
                    .AddDynamicText($"Level X Cleric: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarclerictxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Cleric); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarclericbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarpaladin")
                    .AddDynamicText($"Level X Paladin: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarpaladintxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Paladin); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarpaladinbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarranger")
                    .AddDynamicText($"Level X Ranger: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarrangertxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Ranger); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarrangerbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarshadowknight")
                    .AddDynamicText($"Level X Shadowknight: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarshadowknighttxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Shadowknight); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarshadowknightbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbardruid")
                    .AddDynamicText($"Level X Druid: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbardruidtxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Druid); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbardruidbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarmonk")
                    .AddDynamicText($"Level X Monk: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarmonktxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Monk); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarmonkbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarbard")
                    .AddDynamicText($"Level X Bard: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarbardtxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Bard); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarbardbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarrogue")
                    .AddDynamicText($"Level X Rogue: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarroguetxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Rogue); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarroguebtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarshaman")
                    .AddDynamicText($"Level X Shaman: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarshamantxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Shaman); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarshamanbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarnecromancer")
                    .AddDynamicText($"Level X Necromancer: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarnecromancertxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Necromancer); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarnecromancerbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarwizard")
                    .AddDynamicText($"Level X Wizard: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarwizardtxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Wizard); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarwizardbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarmagician")
                    .AddDynamicText($"Level X Magician: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarmagiciantxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Magician); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarmagicianbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarenchanter")
                    .AddDynamicText($"Level X Enchanter: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarenchantertxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Enchanter); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarenchanterbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarbeastlord")
                    .AddDynamicText($"Level X Beastlord: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarbeastlordtxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Beastlord); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarbeastlordbtn")

                    .AddStatbar(statbarBounds = statbarBounds.BelowCopy(0, 25).WithFixedSize(width, 20), new double[] { 1, 1, 0, 0.5 }, "classxpbarberserker")
                    .AddDynamicText($"Level X Berserker: XP/MAXXP", CairoFont.WhiteDetailText(), line = line.BelowCopy(0, 25).WithFixedSize(width, 20), "classxpbarberserkertxt")
                    .AddButton("+", () => { return OnSpendPoints(AdventureClass.Berserker); }, statbarBounds.RightCopy(0, 0).WithFixedSize(20, 20), CairoFont.WhiteSmallText(), EnumButtonStyle.Small, EnumTextOrientation.Center, "classxpbarberserkerbtn")

                    .EndChildElements()
                .EndChildElements()
                .Compose()
            ;
            UpdateExperience();
        }

        public void UpdateExperience()
        {
            if (SingleComposer == null)
                return;

            var experience = capi.World.Player.GetExperienceValues();
            var pendingexperience = capi.World.Player.GetPendingExperience();

            var txt = SingleComposer.GetDynamicText("pendingxpbartxt");
            if (txt == null)
                return;

            var bar = SingleComposer.GetStatbar("pendingxpbar");
            if (bar == null)
                return;

            double xpneededforcurrentlevel = 0;
            double xpneededfornextlevel = WorldLimits.GetMaxPendingExperience();
            double totalxpneeded = xpneededfornextlevel - xpneededforcurrentlevel;
            double currentxpprogress = pendingexperience - xpneededforcurrentlevel;
            double percenttolevel = Math.Floor((currentxpprogress / totalxpneeded) * 100);

            bar.SetLineInterval(1f);
            bar.SetValues(((int)percenttolevel)/100f, 0.0f, 1.0f);
            txt.SetNewText($"Unassigned Experience: {pendingexperience} out of MAX {xpneededfornextlevel}");

            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
                UpdateAdventureClassBar(adventureClass, experience.FirstOrDefault(e => e.Item1 == adventureClass)?.Item2, pendingexperience);
        }

        public void UpdateAdventureClassBar(AdventureClass adventureClass, double? xp, double pendingexperience)
        {
            if (xp == null)
                return;

            var bar = SingleComposer.GetStatbar("classxpbar" + adventureClass.ToString().ToLower());
            if (bar == null)
                return;

            var txt = SingleComposer.GetDynamicText("classxpbar" + adventureClass.ToString().ToLower()+"txt");
            if (txt == null)
                return;

            var btn = SingleComposer.GetButton("classxpbar" + adventureClass.ToString().ToLower() + "btn");
            if (btn == null)
                return;

            var lvl = PlayerUtils.GetLevelFromExperience((double)xp);
            var nextXp = PlayerUtils.GetExperienceRequirementForLevel(lvl+1);
            var percentage = PlayerUtils.GetExperiencePercentage(lvl, (double)xp)/100f;
            bar.SetLineInterval(1f);
            bar.SetValues(percentage, 0.0f, 1.0f);
            txt.SetNewText($"Level {lvl} {adventureClass.ToString()}: {(double)xp}/{(double)nextXp}");

            if (nextXp <= (xp+pendingexperience))
                btn.Enabled = true;
            else
                btn.Enabled = false;
        }


        private bool OnSpendPoints(AdventureClass classToSpend)
        {
            capi.Network.GetChannel("spendpendingexperience").SendPacket(new SpendPendingExperiencePacket()
            {
                adventureClass = classToSpend
            });

            return true;
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
