using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Gui;
using vsroleplayclasses.src.Packets;

namespace vsroleplayclasses.src.Systems
{
    public class SystemExperience : ModSystem
    {
        GuiDialogExperience experienceDialog;
        ICoreClientAPI capi;
        ICoreAPI api;
        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }
        public override void Start(ICoreAPI api)
        {
            this.api = api;
            base.Start(api);

            api.Network.RegisterChannel("spendpendingexperience").RegisterMessageType<SpendPendingExperiencePacket>();
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
            capi.Input.RegisterHotKey("experience", "Opens the Experience Window", GlKeys.X, HotkeyType.GUIOrOtherControls, true);
            capi.Input.SetHotKeyHandler("experience", ToggleExperienceGui);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);
            api.RegisterCommand("xp", "lists information about experience", "", CmdXp, null);
            api.RegisterCommand("givexp", "grants experience", "", CmdGiveXp, "root");
            api.RegisterEntityBehaviorClass("EntityBehaviorExperience", typeof(EntityBehaviorExperience));
            base.StartServerSide(api);
            api.Network.GetChannel("spendpendingexperience").SetMessageHandler<SpendPendingExperiencePacket>(OnSpendPendingExperiencePacket);
        }

        private void OnSpendPendingExperiencePacket(IServerPlayer fromPlayer, SpendPendingExperiencePacket networkMessage)
        {
            fromPlayer.SpendPendingExperience(networkMessage.adventureClass);
        }

        private void CmdGiveXp(IServerPlayer player, int groupId, CmdArgs args)
        {
            if (args.Length < 2)
            {
                player.SendMessage(groupId, "Missing args (playername, xp)", EnumChatType.OwnMessage);
                return;
            }

            long xp = 0;
            long.TryParse(args[1], out xp);

            if (xp < 1)
            {
                player.SendMessage(groupId, "XP must be greater than 0", EnumChatType.OwnMessage);
                return;
            }

            var targetPlayer = player.Entity.World.GetPlayerByName(args[0]);
            if (targetPlayer == null)
            {
                player.SendMessage(groupId, "Could not find player by name, try /who", EnumChatType.OwnMessage);
                return;
            }

            ((IServerPlayer)targetPlayer).GrantPendingExperience(xp);
            player.SendMessage(groupId, $"Granted additional {xp} pending XP to {targetPlayer.PlayerName}", EnumChatType.OwnMessage);
        }

        private void CmdXp(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Experience:", EnumChatType.OwnMessage);
            foreach(var value in player.GetExperienceValues())
            {
                player.SendMessage(groupId, value.Item1.ToString()+":"+value.Item2 + $" (Level: {player.Entity.GetLevel(value.Item1)}) {player.GetExperiencePercentage(value.Item1)} % into level - XP: " + player.GetExperience(value.Item1) + "/" + PlayerUtils.GetExperienceRequirementForLevel(player.Entity.GetLevel(value.Item1) + 1), EnumChatType.OwnMessage);
            }
            player.SendMessage(groupId, "Pending Xp: " + player.GetPendingExperience(), EnumChatType.OwnMessage);
            player.SendMessage(groupId, player.GetPlayerOverallLevelAsText(), EnumChatType.OwnMessage);
        }


        private bool ToggleExperienceGui(KeyCombination comb)
        {
            if (experienceDialog == null)
                experienceDialog = new GuiDialogExperience(capi);

            if (experienceDialog.IsOpened()) experienceDialog.TryClose();
            else experienceDialog.TryOpen();

            return true;
        }

        private void OnPlayerNowPlaying(IServerPlayer player)
        {
            RegisterPlayerClassChangedListener(player);
            RegisterPlayerExperienceChangedListener(player);
            RegisterPlayerLevelChangedListener(player);
        }

        private void RegisterPlayerClassChangedListener(IServerPlayer player)
        {
            player.Entity.WatchedAttributes.RegisterModifiedListener("characterClass", (System.Action)(() => OnPlayerClassChanged(player)));
        }

        private void RegisterPlayerLevelChangedListener(IServerPlayer player)
        {
            player.Entity.WatchedAttributes.RegisterModifiedListener("level", (System.Action)(() => OnPlayerLevelChanged(player)));
        }

        private void RegisterPlayerExperienceChangedListener(IServerPlayer player)
        {
            foreach (AdventureClass experienceType in Enum.GetValues(typeof(AdventureClass)))
            {
                if (experienceType == AdventureClass.None)
                    continue;

                player.Entity.WatchedAttributes.RegisterModifiedListener(experienceType.ToString().ToLower()+"xp", (System.Action)(() => OnPlayerExperienceChanged(player, experienceType)));
                player.Entity.WatchedAttributes.RegisterModifiedListener(experienceType.ToString().ToLower() + "lvl", (System.Action)(() => OnPlayerAdventureLevelChanged(player, experienceType)));
            }
        }

        private void OnPlayerAdventureLevelChanged(IServerPlayer player, AdventureClass experienceType)
        {
            if (player.Entity.GetLevel(experienceType) > 1)
                player.SendMessage(GlobalConstants.InfoLogChatGroup, $"* You have reached {experienceType.ToString().ToLower()} level " + player.Entity.GetLevel(experienceType) + "!", EnumChatType.OwnMessage);
        }

        private void OnPlayerExperienceChanged(IServerPlayer player, AdventureClass experienceType)
        {
            if (player.GetExperience(experienceType) > 0)
                player.SendMessage(GlobalConstants.InfoLogChatGroup, "* You have gained "+ experienceType.ToString().ToLower() + " experience!", EnumChatType.OwnMessage);

            player.TryUpdateLevel(experienceType);
            player.TryUpdateOverallLevel();
        }

        private void OnPlayerLevelChanged(IServerPlayer player)
        {
            if (player.GetLevel() > 1)
                player.SendMessage(GlobalConstants.InfoLogChatGroup, "* You have reached overall level " + player.GetLevel() + "!", EnumChatType.OwnMessage);
        }

        private void OnPlayerClassChanged(IServerPlayer player)
        {
            player.ResetExperience();
        }

        private void OnGameTick(float tick)
        {

        }
    }
}
