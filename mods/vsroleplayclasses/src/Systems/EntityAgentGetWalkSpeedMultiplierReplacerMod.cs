using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src.Systems
{
    [HarmonyPatch]
    // original of this after VS updates is at:
    // https://github.com/anegostudios/vsapi/blob/master/Common/Entity/EntityAgent.cs
    // last updated by tyron on 3rd of march 2022
    // the point of this is variable runspeed for snare, sow etc
    public sealed class EntityAgentGetWalkSpeedMultiplierReplacerMod : ModSystem
    {
        private readonly Harmony harmony;
        public EntityAgentGetWalkSpeedMultiplierReplacerMod()
        {
            harmony = new Harmony("EntityAgentGetWalkSpeedMultiplierReplacerMod");
            harmony.PatchAll();
        }

        public override void Start(ICoreAPI api)
        {
            harmony.PatchAll();
            base.Start(api);
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
    }

    // Implements proper damage type handling based on weapon
    [HarmonyPatch(typeof(EntityAgent), "GetWalkSpeedMultiplier")]
    public class EntityAgent_GetWalkSpeedMultiplier
    {
        [HarmonyPrefix]
        public static bool Prefix(EntityAgent __instance, double groundDragFactor, ref double __result)
        {
            int y1 = (int)(__instance.SidedPos.Y - 0.05f);
            int y2 = (int)(__instance.SidedPos.Y + 0.01f);

            Vintagestory.API.Common.Block belowBlock = __instance.World.BlockAccessor.GetBlock((int)__instance.SidedPos.X, y1, (int)__instance.SidedPos.Z);

            __instance.GetInsidePos().Set((int)__instance.SidedPos.X, y2, (int)__instance.SidedPos.Z);
            __instance.SetInsideBlock(__instance.World.BlockAccessor.GetBlock(__instance.GetInsidePos()));

            double multiplier = (__instance.ServerControls.Sneak ? GlobalConstants.SneakSpeedMultiplier : 1.0) * (__instance.ServerControls.Sprint ? GlobalConstants.SprintSpeedMultiplier : 1.0);

            if (__instance.FeetInLiquid) multiplier /= 2.5;

            IPlayer player = (__instance as EntityPlayer)?.Player;
            if (player == null || player.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                multiplier *= belowBlock.WalkSpeedMultiplier * (y1 == y2 ? 1 : __instance.GetInsideBlock().WalkSpeedMultiplier);
            }
            var bonus = __instance.WatchedAttributes.GetFloat("currentWalkSpeedBonus", 0.0f);
            multiplier *= GameMath.Clamp(__instance.Stats.GetBlended("walkspeed")+ bonus, 0, 999);


            if (player != null)
            {
                Console.WriteLine(bonus + " " + multiplier);
            }
            __result = multiplier;


            return false;
        }
    }

}
