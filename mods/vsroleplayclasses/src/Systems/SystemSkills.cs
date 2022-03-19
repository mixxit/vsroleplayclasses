using System;
using System.Collections.Concurrent;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Gui;


namespace vsroleplayclasses.src.Systems
{
    public class SystemSkills : ModSystem
    {
        ICoreServerAPI serverApi;
        GuiDialogSkills skillsDialog;
        ICoreClientAPI capi;
        ICoreAPI api;

        public override void Start(ICoreAPI api)
        {
            this.api = api;
            base.Start(api);
        }

        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }
        public override void StartServerSide(ICoreServerAPI api)
        {
            serverApi = api;
            base.StartServerSide(api);
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);
            api.RegisterEntityBehaviorClass("EntityBehaviorSkillable", typeof(EntityBehaviorSkillable));

        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
            capi.Input.RegisterHotKey("skills", "Shows your skill levels", GlKeys.S, HotkeyType.GUIOrOtherControls, true);
            capi.Input.SetHotKeyHandler("skills", ToggleSkillsGui);
        }

        private bool ToggleSkillsGui(KeyCombination comb)
        {
            if (skillsDialog == null)
                skillsDialog = new GuiDialogSkills(capi);

            if (skillsDialog.IsOpened()) skillsDialog.TryClose();
            else skillsDialog.TryOpen();

            return true;
        }

        private void OnPlayerNowPlaying(IServerPlayer player)
        {
            RegisterPlayerClassChangedListener(player);
            RegisterPlayerSkillChangedListener(player);
        }

        private void RegisterPlayerSkillChangedListener(IServerPlayer player)
        {
            foreach (SkillType skill in Enum.GetValues(typeof(SkillType)))
                player.Entity.WatchedAttributes.RegisterModifiedListener("skill_"+skill.ToString(), (System.Action)(() => OnPlayerSkillChanged(player, skill)));
        }

        private void OnPlayerSkillChanged(IServerPlayer player, SkillType type)
        {
            if (player.Entity.GetSkill(type) > 1)
                player.SendMessage(GlobalConstants.CurrentChatGroup, $"* Your {type.ToString().ToLower()} skill has increased " + player.Entity.GetSkill(type) + "/" + player.Entity.GetMaxSkill(type) + "!", EnumChatType.OwnMessage);
        }

        private void RegisterPlayerClassChangedListener(IServerPlayer player)
        {
            player.Entity.WatchedAttributes.RegisterModifiedListener("characterClass", (System.Action)(() => OnPlayerClassChanged(player)));
        }

        private void OnPlayerClassChanged(IServerPlayer player)
        {
            player.Entity.ResetSkillsToZeroAndRecalculateMaxSkills();
        }
    }
}
