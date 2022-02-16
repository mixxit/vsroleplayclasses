using projectrarahat.src.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;

namespace vsroleplayclasses.src.Systems
{
    public class SystemExperience : ModSystem
    {
        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);
            api.RegisterCommand("xp", "lists information about experience", "", CmdXp, null);
            api.RegisterEntityBehaviorClass("EntityBehaviorExperience", typeof(EntityBehaviorExperience));
            base.StartServerSide(api);
        }

        private void CmdXp(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Experience:", EnumChatType.OwnMessage);
            foreach(var value in player.GetExperienceValues())
            {
                player.SendMessage(groupId, value.Item1+":"+value.Item2, EnumChatType.OwnMessage);
            }
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
            player.ResetExperience();
        }

        private void OnGameTick(float tick)
        {

        }
    }
}
