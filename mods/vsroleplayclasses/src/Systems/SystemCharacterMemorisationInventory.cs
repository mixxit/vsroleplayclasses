using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.Common;
using Vintagestory.GameContent;
using Vintagestory.Server;
using vsroleplayclasses.src.Inventories;

namespace vsroleplayclasses.src.Systems
{
    [HarmonyPatch]
    public sealed class SystemCharacterMemorisationInventory : ModSystem
    {
        public override void StartClientSide(ICoreClientAPI api)
        {
            ClientMain.ClassRegistry.RegisterInventoryClass("memoriseability", typeof(InventoryPlayerMemorisation));
            base.StartClientSide(api);
        }
        public override double ExecuteOrder()
        {
            /// Worldgen:
            /// - GenTerra: 0 
            /// - RockStrata: 0.1
            /// - Deposits: 0.2
            /// - Caves: 0.3
            /// - Blocklayers: 0.4
            /// Asset Loading
            /// - Json Overrides loader: 0.05
            /// - Load hardcoded mantle block: 0.1
            /// - Block and Item Loader: 0.2
            /// - Recipes (Smithing, Knapping, Clayforming, Grid recipes, Alloys) Loader: 1
            /// 
            return 1.1;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            ServerMain.ClassRegistry.RegisterInventoryClass("memoriseability", typeof(InventoryPlayerMemorisation));
            api.Event.PlayerJoin += new PlayerDelegate(this.OnPlayerJoinServer);
            base.StartServerSide(api);
        }

        private void OnPlayerJoinServer(IServerPlayer player)
        {
            string key = "memoriseability" + "-" + player.WorldData.PlayerUID;
            if (!player.InventoryManager.Inventories.ContainsKey(key))
            {
                InventoryBasePlayer inventory = (InventoryBasePlayer)ServerMain.ClassRegistry.CreateInventory("memoriseability", key, player.Entity.Api);
                ((ServerPlayer)player).SetInventory(inventory);
                player.InventoryManager.Inventories[key].Open((IPlayer)player);
            }
        }

    }
}
