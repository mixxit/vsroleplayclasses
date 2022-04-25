using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;

namespace vsroleplayclasses.src.Systems
{
    [HarmonyPatch]
    // original of this after VS updates is at:
    // https://github.com/anegostudios/vsapi/blob/master/Common/Entity/EntityAgent.cs
    // last updated by tyron on 3rd of march 2022
    // the point of this is variable damage for stat items and buffs
    public sealed class EntityAgentOnDamageReplacerMod : ModSystem
    {
        private readonly Harmony harmony;
        public EntityAgentOnDamageReplacerMod()
        {
            harmony = new Harmony("EntityAgentOnDamageReplacerMod");
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
    [HarmonyPatch(typeof(EntityAgent), "OnInteract")]
    public class EntityAgent_OnInteract
    {
        [HarmonyPrefix]
        public static bool Prefix(EntityAgent __instance, EntityAgent byEntity, ItemSlot slot, Vec3d hitPosition, EnumInteractMode mode)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (EntityBehavior behavior in __instance.SidedProperties.Behaviors)
            {
                behavior.OnInteract(byEntity, slot, hitPosition, mode, ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
            }

            if (handled == EnumHandling.PreventDefault || handled == EnumHandling.PreventSubsequent) return false;

            if (mode == EnumInteractMode.Attack)
            {
                float damage = slot.Itemstack == null ? 0.5f : slot.Itemstack.Collectible.GetAttackPower(slot.Itemstack);
                int damagetier = slot.Itemstack == null ? 0 : slot.Itemstack.Collectible.ToolTier;

                damage *= byEntity.Stats.GetBlended("meleeWeaponsDamage");

                if (__instance.Attributes.GetBool("isMechanical", false))
                {
                    damage *= byEntity.Stats.GetBlended("mechanicalsDamage");
                }

                IPlayer byPlayer = null;

                if (byEntity is EntityPlayer && !__instance.IsActivityRunning("invulnerable"))
                {
                    byPlayer = (byEntity as EntityPlayer).Player;

                    __instance.World.PlaySoundAt(new AssetLocation("sounds/player/slap"), __instance.ServerPos.X, __instance.ServerPos.Y, __instance.ServerPos.Z, byPlayer);
                    slot?.Itemstack?.Collectible.OnAttackingWith(byEntity.World, byEntity, __instance, slot);
                }

                if (__instance.Api.Side == EnumAppSide.Client && damage > 1 && !__instance.IsActivityRunning("invulnerable") && __instance.Properties.Attributes?["spawnDamageParticles"].AsBool() == true)
                {
                    Vec3d pos = __instance.SidedPos.XYZ + hitPosition;
                    Vec3d minPos = pos.AddCopy(-0.15, -0.15, -0.15);
                    Vec3d maxPos = pos.AddCopy(0.15, 0.15, 0.15);

                    int textureSubId = __instance.Properties.Client.FirstTexture.Baked.TextureSubId;
                    Vec3f tmp = new Vec3f();

                    for (int i = 0; i < 10; i++)
                    {
                        int color = (__instance.Api as ICoreClientAPI).EntityTextureAtlas.GetRandomColor(textureSubId);

                        tmp.Set(
                            1f - 2 * (float)__instance.World.Rand.NextDouble(),
                            2 * (float)__instance.World.Rand.NextDouble(),
                            1f - 2 * (float)__instance.World.Rand.NextDouble()
                        );

                        __instance.World.SpawnParticles(
                            1, color, minPos, maxPos,
                            tmp, tmp, 1.5f, 1f, 0.25f + (float)__instance.World.Rand.NextDouble() * 0.25f,
                            EnumParticleModel.Cube, byPlayer
                        );
                    }
                }

                // Our harmony patch start here
                /*
                DamageSource dmgSource = new DamageSource()
                {
                    Source = (byEntity as EntityPlayer).Player == null ? EnumDamageSource.Entity : EnumDamageSource.Player,
                    SourceEntity = byEntity,
                    Type = EnumDamageType.BluntAttack,
                    HitPosition = hitPosition,
                    DamageTier = damagetier
                };
                */

                var damageType = EnumDamageType.BluntAttack;


                if (slot.Itemstack?.Item != null && slot.Itemstack.Item is ItemAxe)
                    damageType = EnumDamageType.SlashingAttack;
                // animation appears to go downwards
                if (slot.Itemstack?.Item != null && slot.Itemstack.Item is ItemKnife)
                    damageType = EnumDamageType.SlashingAttack;
                if (slot.Itemstack?.Item != null && slot.Itemstack.Item is ItemScythe)
                    damageType = EnumDamageType.SlashingAttack;
                if (slot.Itemstack?.Item != null && slot.Itemstack.Item is ItemShears)
                    damageType = EnumDamageType.SlashingAttack;
                if (slot.Itemstack?.Item != null && slot.Itemstack.Item is ItemSpear)
                    damageType = EnumDamageType.PiercingAttack;
                if (slot.Itemstack?.Item != null && slot.Itemstack.Item is ItemSword)
                    damageType = EnumDamageType.SlashingAttack;

                DamageSource dmgSource = new DamageSource()
                {
                    Source = (byEntity as EntityPlayer).Player == null ? EnumDamageSource.Entity : EnumDamageSource.Player,
                    SourceEntity = byEntity,
                    Type = damageType,
                    HitPosition = hitPosition,
                    DamageTier = damagetier
                };

                // Our harmony patch ends here

                EntityBehaviorWeapon ebt = byEntity.GetBehavior("EntityBehaviorWeapon") as EntityBehaviorWeapon;
                if (ebt != null)
                {
                    damage = (float)ebt.CalculateWeaponDamage(damageType, (int)damage, __instance);
                }

                if (__instance.ReceiveDamage(dmgSource, damage))
                {
                    byEntity.DidAttack(dmgSource, __instance);
                }

            }

            return false;
        }
    }

}
