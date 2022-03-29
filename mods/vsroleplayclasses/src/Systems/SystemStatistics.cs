using vsroleplayclasses.src.Extensions;
using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Common.Entities;
using vsroleplayclasses.src.Behaviors;

namespace vsroleplayclasses.src.Systems
{
    public class SystemStatistics : ModSystem
    {
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
            api.Event.SaveGameLoaded += new System.Action(this.OnSaveGameLoaded);
            api.Event.GameWorldSave += new System.Action(this.OnSaveGameSaving);
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);

            api.RegisterCommand("pstats", "gets player statistical overview", "", GetPlayerOverviewAsText, "root");
            api.RegisterEntityBehaviorClass("EntityBehaviorStatistics", typeof(EntityBehaviorStatistics));
            base.StartServerSide(api);
        }

        private void GetPlayerOverviewAsText(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, player.GetPlayerOverviewAsText(), EnumChatType.CommandSuccess);
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
            player.Entity.ResetStatisticState();
        }

        private void OnSaveGameLoaded()
        {
            
        }


        private void OnSaveGameSaving()
        {
            
        }


        private void OnGameTick(float tick)
        {
            
        }
    }
}
