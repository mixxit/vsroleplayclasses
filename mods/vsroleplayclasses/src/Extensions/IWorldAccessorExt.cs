using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Items;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Extensions
{
    public static class IWorldAccessorExt
    {

        public static IPlayer GetPlayerByName(this IWorldAccessor world, string playerName)
        {
            return world.AllPlayers.FirstOrDefault(e => String.Equals(e.PlayerName, playerName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
