using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent.Mechanics;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Inventories;
using vsroleplayclasses.src.Gui.Dialog;
using vsroleplayclasses.src.Items;
using vsroleplayclasses.src.Systems;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src.BlockEntities
{
    public class BlockEntityScribesTable : BlockEntityOpenableContainer
    {
        public float inputScribeTime;
        bool beforeScribing;
        public float prevInputScribeTime;
        // Server side only
        Dictionary<string, long> playersScribing = new Dictionary<string, long>();
        // Client and serverside
        int quantityPlayersScribing;

        static BlockEntityScribesTable()
        {
        }

        internal InventoryScribesTable inventory;

        public void IsScribing(IPlayer byPlayer)
        {
            SetPlayerScribing(byPlayer, true);
        }

        GuiDialogBlockEntityScribesTable clientDialog;

        #region Getters

        public string Material
        {
            get { return Block.LastCodePart(); }
        }


        #endregion

        #region Config

        public override string InventoryClassName
        {
            get { return "scribestable"; }
        }

        public virtual string DialogTitle
        {
            get { return Lang.Get("ScribesTable"); }
        }

        public override InventoryBase Inventory
        {
            get { return inventory; }
        }

        #endregion


        public BlockEntityScribesTable()
        {
            inventory = new InventoryScribesTable(null, null);
            inventory.SlotModified += OnSlotModifid;
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            RegisterGameTickListener(Every100ms, 100);
            RegisterGameTickListener(Every500ms, 500);
            inventory.LateInitialize("scribestable-" + Pos.X + "/" + Pos.Y + "/" + Pos.Z, api);
        }

        public float ScribeSpeed
        {
            get
            {
                if (quantityPlayersScribing > 0) 
                    return 1f;
                
                return 0;
            }
        }

        private void Every100ms(float dt)
        {
            float scribeSpeed = ScribeSpeed;

            if (Api.Side == EnumAppSide.Client)
            {
                return;
            }
            // Only tick on the server and merely sync to client

                // Use up fuel
            if (CanScribe() && scribeSpeed > 0)
            {
                inputScribeTime += dt * scribeSpeed;

                if (inputScribeTime >= maxScribeingTime())
                {
                    FinishScribe();
                    inputScribeTime = 0;
                }

                MarkDirty();
            }
        }

        // Sync to client every 500ms
        private void Every500ms(float dt)
        {
            if (Api.Side == EnumAppSide.Server 
                && (ScribeSpeed > 0 || prevInputScribeTime != inputScribeTime) 
                && CanScribe()
                )  //don't spam update packets when empty, as inputScribeTime is irrelevant when empty
            {
                MarkDirty();
            }

        }

        private void FinishScribe()
        {
            if (Api.Side != EnumAppSide.Server)
                return;

            if (inventory == null || inventory[0]?.Itemstack == null || inventory[7]?.Itemstack != null)
                return;

            var resistType = RunicTools.GetWordOfPowerFromQuillItem<ResistType>((RunicInkwellAndQuillItem)inventory[1].Itemstack.Item);
            var spellEffectType = RunicTools.GetWordOfPowerFromQuillItem<SpellEffectType>((RunicInkwellAndQuillItem)inventory[2].Itemstack.Item);
            var spellEffectIndex = RunicTools.GetWordOfPowerFromQuillItem<SpellEffectIndex>((RunicInkwellAndQuillItem)inventory[3].Itemstack.Item);
            var targetType = RunicTools.GetWordOfPowerFromQuillItem<TargetType>((RunicInkwellAndQuillItem)inventory[4].Itemstack.Item);
            var powerLevel = RunicTools.GetWordOfPowerFromQuillItem<PowerLevel>((RunicInkwellAndQuillItem)inventory[5].Itemstack.Item);
            var adventureClass = RunicTools.GetWordOfPowerFromQuillItem<AdventureClass>((RunicInkwellAndQuillItem)inventory[6].Itemstack.Item);

            if (resistType == ResistType.None)
                return;
            if (spellEffectType == SpellEffectType.None)
                return;
            if (spellEffectIndex == SpellEffectIndex.None)
                return;
            if (targetType == TargetType.None)
                return;
            if (powerLevel == PowerLevel.None)
                return;
            if (adventureClass == AdventureClass.None)
                return;

            var itemStack = inventory[0].Itemstack.Clone();
            if (!TryApplyRuneToScroll<ResistType>(itemStack, resistType))
                return;
            if (!TryApplyRuneToScroll<SpellEffectType>(itemStack, spellEffectType))
                return;
            if (!TryApplyRuneToScroll<SpellEffectIndex>(itemStack, spellEffectIndex))
                return;
            if (!TryApplyRuneToScroll<TargetType>(itemStack, targetType))
                return;
            if (!TryApplyRuneToScroll<PowerLevel>(itemStack, powerLevel))
                return;
            if (!TryApplyRuneToScroll<AdventureClass>(itemStack, adventureClass))
                return;

            if (!TryTransitionToAbility(itemStack))
                return;

            inventory[7].TakeOut(1);
            inventory[7].Itemstack = itemStack;


            inventory[0].TakeOut(1);
            inventory[1].TakeOut(1);
            inventory[2].TakeOut(1);
            inventory[3].TakeOut(1);
            inventory[4].TakeOut(1);
            inventory[5].TakeOut(1);
            inventory[6].TakeOut(1);

            inventory[0].MarkDirty();
            inventory[1].MarkDirty();
            inventory[2].MarkDirty();
            inventory[3].MarkDirty();
            inventory[4].MarkDirty();
            inventory[5].MarkDirty();
            inventory[6].MarkDirty();

            inventory[7].MarkDirty();
        }

        private bool TryTransitionToAbility(ItemStack itemstack)
        {
            // need a player
            if (playersScribing.Count < 1)
                return false;

            var lastScribingPlayerUid = playersScribing.LastOrDefault().Key;
            if (lastScribingPlayerUid == null)
                return false;

            IPlayer lastScribingPlayer = Api.World.PlayerByUid(lastScribingPlayerUid);
            if (lastScribingPlayer == null)
                return false;

            if (!(lastScribingPlayer is IServerPlayer))
                return false;

            if (itemstack == null)
                return false;

            if (!(itemstack.Item is AbilityScrollItem))
                return false;

            if (((AbilityScrollItem)itemstack.Item).IsAbilityScribed(itemstack))
                return false;

            if (((AbilityScrollItem)itemstack.Item).HasSpareRuneSlot(itemstack))
                return false;

            SystemAbilities mod = Api.ModLoader.GetModSystem<SystemAbilities>();
            long abilityId = mod.TryCreateAbility((IServerPlayer)lastScribingPlayer, itemstack);
            if (abilityId > 0)
            {
                ((AbilityScrollItem)itemstack.Item).SetScribedAbility(itemstack, mod.GetAbilityById(abilityId));
                return true;
            }

            return false;
        }

        private bool TryApplyRuneToScroll<T>(ItemStack itemstack, T wordOfPower) where T : Enum
        {
            if (itemstack == null)
                return false;

            if (!(itemstack.Item is AbilityScrollItem))
                return false;

            if (((AbilityScrollItem)itemstack.Item).IsAbilityScribed(itemstack))
                return false;

            if (!((AbilityScrollItem)itemstack.Item).HasSpareRuneSlot(itemstack))
                return false;

            if (((AbilityScrollItem)itemstack.Item).HasRunePower<T>(itemstack))
                return false;

            if (Object.Equals(((AbilityScrollItem)itemstack.Item).GetWordOfPower<T>(itemstack), default(T)))
            {
                ((AbilityScrollItem)itemstack.Item).SetWordOfPower<T>(itemstack, wordOfPower);
                return true;
            }
            return false;
        }

        public bool CanScribe()
        {
            if (!(inventory[0]?.Itemstack?.Item is AbilityScrollItem && ((AbilityScrollItem)inventory[0]?.Itemstack.Item).GetScribedAbilityId(inventory[0]?.Itemstack) < 1))
                return false;

            if (!(inventory[1]?.Itemstack?.Item is RunicInkwellAndQuillItem && 
                inventory[1].Itemstack.ItemAttributes.KeyExists("runetype") &&
                inventory[1].Itemstack.ItemAttributes["runetype"].ToString().Equals("resisttype")
                )
               )
                return false;
            if (!(inventory[2]?.Itemstack?.Item is RunicInkwellAndQuillItem && 
                inventory[2].Itemstack.ItemAttributes.KeyExists("runetype") &&
                inventory[2].Itemstack.ItemAttributes["runetype"].ToString().Equals("spelleffect")
                )
               )
                return false;
            if (!(inventory[3]?.Itemstack?.Item is RunicInkwellAndQuillItem && 
                inventory[3].Itemstack.ItemAttributes.KeyExists("runetype") &&
                inventory[3].Itemstack.ItemAttributes["runetype"].ToString().Equals("spelleffectindex")
                )
               )
                return false;
            if (!(inventory[4]?.Itemstack?.Item is RunicInkwellAndQuillItem && 
                inventory[4].Itemstack.ItemAttributes.KeyExists("runetype") &&
                inventory[4].Itemstack.ItemAttributes["runetype"].ToString().Equals("targettype")
                )
               )
                return false;
            if (!(inventory[5]?.Itemstack?.Item is RunicInkwellAndQuillItem && 
                inventory[5].Itemstack.ItemAttributes.KeyExists("runetype") &&
                inventory[5].Itemstack.ItemAttributes["runetype"].ToString().Equals("powerlevel")
                )
               )
                return false;
            if (!(inventory[6]?.Itemstack?.Item is RunicInkwellAndQuillItem && 
                inventory[6].Itemstack.ItemAttributes.KeyExists("runetype") &&
                inventory[6].Itemstack.ItemAttributes["runetype"].ToString().Equals("adventureclass")
                )
               )
                return false;
            if (inventory[7]?.Itemstack?.Item != null)
                return false;

            return true;
        }

        private void OnSlotModifid(int slotid)
        {
            if (Api is ICoreClientAPI)
            {
                clientDialog.Update(inputScribeTime, maxScribeingTime());
            }

            if (slotid != 7)
            {
                inputScribeTime = 0.0f; //reset the progress to 0 if the item is removed.
                MarkDirty();

                if (clientDialog != null && clientDialog.IsOpened())
                {
                    clientDialog.SingleComposer.ReCompose();
                }
            }
        }

        public virtual float maxScribeingTime()
        {
            return 100;
        }

        #region Events

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            if (blockSel.SelectionBoxIndex == 1) return false;

            if (Api.World is IServerWorldAccessor)
            {
                ((ICoreServerAPI)Api).Network.SendBlockEntityPacket(
                    (IServerPlayer)byPlayer,
                    Pos.X, Pos.Y, Pos.Z,
                    (int)EnumBlockStovePacket.OpenGUI,
                    null
                );
                
                // Start - For recording who is scribing
                if (!Inventory.openedByPlayerGUIds.Contains(byPlayer.PlayerUID))
                {
                    SetPlayerScribing(byPlayer, true);
                }
                // End - For recording who is scribing

                byPlayer.InventoryManager.OpenInventory(inventory);
                MarkDirty();
            }

            return true;
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            Inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));

            if (Api != null)
            {
                Inventory.AfterBlocksLoaded(Api.World);
            }

            inputScribeTime = tree.GetFloat("inputScribeTime");

            if (worldForResolving.Side == EnumAppSide.Client)
            {
                List<int> clientIds = new List<int>((tree["clientIdsScribing"] as IntArrayAttribute).value);

                quantityPlayersScribing = clientIds.Count;

                string[] playeruids = playersScribing.Keys.ToArray();

                foreach (var uid in playeruids)
                {
                    IPlayer plr = Api.World.PlayerByUid(uid);

                    if (!clientIds.Contains(plr.ClientId))
                    {
                        playersScribing.Remove(uid);
                    }
                    else
                    {
                        clientIds.Remove(plr.ClientId);
                    }
                }

                for (int i = 0; i < clientIds.Count; i++)
                {
                    IPlayer plr = worldForResolving.AllPlayers.FirstOrDefault(p => p.ClientId == clientIds[i]);
                    if (plr != null) playersScribing.Add(plr.PlayerUID, worldForResolving.ElapsedMilliseconds);
                }

                updateScribingState();
            }

            if (Api?.Side == EnumAppSide.Client && clientDialog != null)
            {
                clientDialog.Update(inputScribeTime, maxScribeingTime());
            }
        }

        private void updateScribingState()
        {
            if (Api?.World == null) 
                return;

            bool nowScribing = quantityPlayersScribing > 0;

            if (nowScribing != beforeScribing)
            {

                if (Api.Side == EnumAppSide.Server)
                {
                    MarkDirty();
                }
            }

            beforeScribing = nowScribing;
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invtree = new TreeAttribute();
            Inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;
            tree.SetFloat("inputScribeTime", inputScribeTime);

            List<int> vals = new List<int>();
            foreach (var val in playersScribing)
            {
                IPlayer plr = Api.World.PlayerByUid(val.Key);
                if (plr == null) continue;
                vals.Add(plr.ClientId);
            }


            tree["clientIdsScribing"] = new IntArrayAttribute(vals.ToArray());
        }

        public override void OnReceivedClientPacket(IPlayer player, int packetid, byte[] data)
        {
            if (packetid < 1000)
            {
                Inventory.InvNetworkUtil.HandleClientPacket(player, packetid, data);

                // Tell server to save this chunk to disk again
                Api.World.BlockAccessor.GetChunkAtBlockPos(Pos.X, Pos.Y, Pos.Z).MarkModified();

                return;
            }

            if (packetid == (int)EnumBlockStovePacket.CloseGUI && player.InventoryManager != null)
            {
                player.InventoryManager.CloseInventory(Inventory);

                // Start - For recording who is scribing
                if (Inventory.openedByPlayerGUIds.Contains(player.PlayerUID))
                {
                    SetPlayerScribing(player, false);
                }
                // End - For recording who is scribing
            }
        }

        public void SetPlayerScribing(IPlayer player, bool playerScribing)
        {
            if (playerScribing)
            {
                playersScribing[player.PlayerUID] = Api.World.ElapsedMilliseconds;
            }
            else
            {
                playersScribing.Remove(player.PlayerUID);
            }

            quantityPlayersScribing = playersScribing.Count;

            updateScribingState();
        }

        public override void OnReceivedServerPacket(int packetid, byte[] data)
        {
            if (packetid == (int)EnumBlockStovePacket.OpenGUI && (clientDialog == null || !clientDialog.IsOpened()))
            {
                clientDialog = new GuiDialogBlockEntityScribesTable(DialogTitle, Inventory, Pos, Api as ICoreClientAPI);
                clientDialog.TryOpen();
                clientDialog.OnClosed += () => clientDialog = null;

                clientDialog.Update(inputScribeTime, maxScribeingTime());
            }

            if (packetid == (int)EnumBlockEntityPacketId.Close)
            {
                IClientWorldAccessor clientWorld = (IClientWorldAccessor)Api.World;
                clientWorld.Player.InventoryManager.CloseInventory(Inventory);
            }
        }

        #endregion

        public override void OnStoreCollectibleMappings(Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            foreach (var slot in Inventory)
            {
                if (slot.Itemstack == null) continue;

                if (slot.Itemstack.Class == EnumItemClass.Item)
                {
                    itemIdMapping[slot.Itemstack.Item.Id] = slot.Itemstack.Item.Code;
                }
                else
                {
                    blockIdMapping[slot.Itemstack.Block.BlockId] = slot.Itemstack.Block.Code;
                }
            }
        }

        public override void OnLoadCollectibleMappings(IWorldAccessor worldForResolve, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed)
        {
            foreach (var slot in Inventory)
            {
                if (slot.Itemstack == null) continue;
                if (!slot.Itemstack.FixMapping(oldBlockIdMapping, oldItemIdMapping, worldForResolve))
                {
                    slot.Itemstack = null;
                }
            }
        }

        public override void OnBlockUnloaded()
        {
            base.OnBlockUnloaded();
       }
    }
}
