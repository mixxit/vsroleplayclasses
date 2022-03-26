using vsroleplayclasses.src.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using vsroleplayclasses.src.Gui;
using System;
using vsroleplayclasses.src.Gui.Hud;

namespace vsroleplayclasses.src.Systems
{
    public class SystemMana : ModSystem
    {
        ICoreServerAPI serverApi;
        
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
        }


        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            serverApi = api;
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);
            api.RegisterCommand("Mana", "lists information about Mana", "", CmdMana, null);
            api.Event.RegisterGameTickListener(OnServerManaTick, 8000);
            base.StartServerSide(api);
        }

        private void OnServerManaTick(float obj)
        {
            foreach (var player in serverApi.World.AllOnlinePlayers)
                ((IServerPlayer)player).Entity.TickInnateMana();
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            capi.Gui.RegisterDialog(new HudManaBar(capi));
        }

        private void CmdMana(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Mana: " + player.Entity.GetMana() + "/" + player.Entity.GetMaxMana(), EnumChatType.OwnMessage);
        }


        private void OnPlayerNowPlaying(IServerPlayer player)
        {
            RegisterPlayerClassChangedListener(player);
        }

        private void RegisterPlayerClassChangedListener(IServerPlayer player)
        {
            player.Entity.WatchedAttributes.RegisterModifiedListener("characterClass", (System.Action)(() => OnPlayerClassChanged(player)));
        }

        private void OnPlayerClassChanged(IServerPlayer player)
        {
            player.Entity.ResetMana();
        }
    }
}
