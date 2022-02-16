using Foundation.Extensions;
using projectrarahat.src.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace vsroleplayclasses.src
{
    public class VSRoleplayClassesMod : ModSystem
    {
        List<CharacterClass> characterClassesItems;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
        }

        public override void StartPre(ICoreAPI api)
        {
            VSRoleplayClassesModConfigFile.Current = api.LoadOrCreateConfig<VSRoleplayClassesModConfigFile>(typeof(VSRoleplayClassesMod).Name + ".json");
            base.StartPre(api);
        }

        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            if (api.Side.Equals(EnumAppSide.Server))
            {
                var asset = api.Assets.TryGet("vsroleplayclasses:config/characterclassesitems.json");
                characterClassesItems = asset.ToObject<List<CharacterClass>>();
            }

            api.Event.SaveGameLoaded += new System.Action(this.OnSaveGameLoaded);
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);
            // Check every 8 seconds
            api.World.RegisterGameTickListener(OnGameTick, 8000);
            api.RegisterCommand("inventorycodes", "dumps your inventory as internal codes", "", CmdInventoryCodes, null);
            api.RegisterCommand("forcescrollability", "forces a scroll abillity", "", CmdForceScrollAbility, "root");
            api.RegisterCommand("getscrollability", "gets ability code", "", GetScrollAbility, "root");
            base.StartServerSide(api);
        }

        private void GetScrollAbility(IServerPlayer player, int groupId, CmdArgs args)
        {
            if (player.Entity.RightHandItemSlot != null && player.Entity.RightHandItemSlot.Itemstack != null && player.Entity.RightHandItemSlot.Itemstack.Attributes != null)
            {
                player.SendMessage(groupId, $"Right Hand: ", EnumChatType.CommandSuccess);
                foreach (var att in player.Entity.RightHandItemSlot.Itemstack.Attributes)
                {
                    player.SendMessage(groupId, $"{att.Key} {att.Value}: ", EnumChatType.CommandSuccess);

                }
            }

            if (player.Entity.LeftHandItemSlot != null && player.Entity.LeftHandItemSlot.Itemstack != null && player.Entity.LeftHandItemSlot.Itemstack.Attributes != null)
            {
                player.SendMessage(groupId, $"Left Hand: ", EnumChatType.CommandSuccess);
                foreach (var att in player.Entity.LeftHandItemSlot.Itemstack.Attributes)
                {
                    player.SendMessage(groupId, $"{att.Key} {att.Value}: ", EnumChatType.CommandSuccess);
                }
            }

        }

        private void CmdForceScrollAbility(IServerPlayer player, int groupId, CmdArgs args)
        {
            if (args.Length < 1)
            {
                player.SendMessage(groupId, $"Missing argument (abilityid)", EnumChatType.CommandError);
                return;
            }

            try
            {
                long serial = Convert.ToInt64(args[0]);

                TryForceAbilityScrollInSlot(player.Entity.RightHandItemSlot, serial);
                TryForceAbilityScrollInSlot(player.Entity.LeftHandItemSlot, serial);

                player.SendMessage(groupId, $"Attempt to force scribe ability completed, please check ability scroll", EnumChatType.CommandSuccess);
            }
            catch (Exception)
            {
                player.SendMessage(groupId, $"Invalid argument (abilityid)", EnumChatType.CommandError);
                return;
            }
        }

        private void TryForceAbilityScrollInSlot(ItemSlot itemSlot, long serial)
        {
            if (itemSlot.Itemstack == null || itemSlot.Itemstack.Item == null || (itemSlot.Itemstack.Item as AbilityScrollItem) == null)
                return;

            // already fixed
            if ((itemSlot.Itemstack.Item as AbilityScrollItem).IsAbilityScribed(itemSlot.Itemstack))
                return;

            (itemSlot.Itemstack.Item as AbilityScrollItem).SetScribedAbility(itemSlot.Itemstack, serial);
            itemSlot.MarkDirty();
        }

        private bool IsValidClassCode(List<CharacterClass> classes, string classCode)
        {
            return classes.Select(e => e.Code).Contains(classCode);
        }

        private void CmdInventoryCodes(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Wearables:", EnumChatType.OwnMessage);
            foreach (var slot in player.InventoryManager.GetOwnInventory(GlobalConstants.characterInvClassName))
            {
                if (slot.Empty)
                    continue;

                ItemStack stack = slot.Itemstack;
                if (stack == null)
                    continue;

                player.SendMessage(groupId, stack.Collectible.Code.ToString(), EnumChatType.OwnMessage);
            }

            player.SendMessage(groupId, "Backpack:", EnumChatType.OwnMessage);
            foreach (var slot in player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName))
            {
                if (slot.Empty)
                    continue;

                ItemStack stack = slot.Itemstack;
                if (stack == null)
                    continue;

                player.SendMessage(groupId, stack.Collectible.Code.ToString(), EnumChatType.OwnMessage);
            }

            player.SendMessage(groupId, "Hotbar:", EnumChatType.OwnMessage);
            foreach (var slot in player.InventoryManager.GetOwnInventory(GlobalConstants.hotBarInvClassName))
            {
                if (slot.Empty)
                    continue;

                ItemStack stack = slot.Itemstack;
                if (stack == null)
                    continue;

                player.SendMessage(groupId, stack.Collectible.Code.ToString(), EnumChatType.OwnMessage);
            }
        }

        internal CharacterClass GetCharacterClassesItems(string className)
        {
            if (String.IsNullOrEmpty(className))
                return null;

            if (this.characterClassesItems == null)
                return null;

            return this.characterClassesItems.FirstOrDefault(e => e.Code != null && e.Code.Equals(className));
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
            player.GrantInitialClassItems();
        }

        private void OnSaveGameLoaded()
        {

        }

        private void OnGameTick(float tick)
        {

        }
    }

    public class VSRoleplayClassesModConfigFile
    {
        public static VSRoleplayClassesModConfigFile Current { get; set; }
    }
}
