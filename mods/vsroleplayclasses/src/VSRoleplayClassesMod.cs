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
            base.StartServerSide(api);
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
