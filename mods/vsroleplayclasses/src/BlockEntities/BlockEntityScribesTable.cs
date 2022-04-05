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

namespace vsroleplayclasses.src.BlockEntities
{
    public class BlockEntityScribesTable : BlockEntityOpenableContainer
    {
        public float inputScribeTime;
        static BlockEntityScribesTable()
        {
        }

        internal InventoryScribesTable inventory;


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
            inventory.LateInitialize("scribestable-" + Pos.X + "/" + Pos.Y + "/" + Pos.Z, api);
        }

        public float ScribeSpeed = 1.0f;

        private void Every100ms(float dt)
        {
            float scribeSpeed = ScribeSpeed;

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

        private void FinishScribe()
        {

        }

        public bool CanScribe()
        {
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
                MarkDirty();

                if (clientDialog != null && clientDialog.IsOpened())
                {
                    clientDialog.SingleComposer.ReCompose();
                }
            }
        }

        public virtual float maxScribeingTime()
        {
            return 4;
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

            if (Api?.Side == EnumAppSide.Client && clientDialog != null)
            {
                clientDialog.Update(inputScribeTime, maxScribeingTime());
            }
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invtree = new TreeAttribute();
            Inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;
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
            }
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
