using projectrarahat.src.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;

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
            base.StartServerSide(api);
        }

        private void CmdMana(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Mana: " + player.GetMana(), EnumChatType.OwnMessage);
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
            player.ResetMana();
            player.ResetStatisticState();
        }

        private void OnGameTick(float tick)
        {

        }
    }
}
