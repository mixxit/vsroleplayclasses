using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Entities;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Gui;
using vsroleplayclasses.src.Items;
using vsroleplayclasses.src.Packets;

namespace vsroleplayclasses.src.Systems
{
    public class SystemWho : ModSystem
    {
        ICoreServerAPI serverApi;
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
            api.RegisterCommand("who", "gets a list of players online", "", CmdWho, "root");
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
        }

        private void CmdWho(IServerPlayer player, int groupId, CmdArgs args)
        {
            foreach (var onlinePlayer in player.Entity.World.AllOnlinePlayers)
            {
                var classes = onlinePlayer.Entity.GetHighestLevelOrNone();
                player.SendMessage(groupId, $"{onlinePlayer.PlayerName} - {onlinePlayer.Entity.GetBehavior<EntityBehaviorNameTag>().DisplayName} ({AddOrdinal(classes.Item2)} Season {PlayerNameUtils.FirstCharToUpper(player.Entity.GetRaceName())} {classes.Item1}) Overall: {onlinePlayer.Entity.GetLevel()}", EnumChatType.CommandError);
            }
        }

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}
