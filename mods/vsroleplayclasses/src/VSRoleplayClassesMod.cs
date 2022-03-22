using Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src
{
    public class VSRoleplayClassesMod : ModSystem
    {
        Random rand;
        List<CharacterClass> characterClassesItems;
        List<String> skillChecks;

        public override void Start(ICoreAPI api)
        {
            rand = new Random();
            base.Start(api);
            skillChecks = new List<string>();
            skillChecks.Add("athletics");
            skillChecks.Add("acrobatics");
            skillChecks.Add("sleightofhand");
            skillChecks.Add("stealth");
            skillChecks.Add("arcana");
            skillChecks.Add("history");
            skillChecks.Add("investigation");
            skillChecks.Add("nature");
            skillChecks.Add("religion");
            skillChecks.Add("animalhandling");
            skillChecks.Add("insight");
            skillChecks.Add("medicine");
            skillChecks.Add("perception");
            skillChecks.Add("survival");
            skillChecks.Add("deception");
            skillChecks.Add("intimidation");
            skillChecks.Add("performance");
            skillChecks.Add("persuasion");

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
            api.RegisterCommand("itemattributes", "views attributes of items in hand", "", CmdItemAttributes, "root");
            api.RegisterCommand("skillcheck", "performs a skill check", "", CmdSkillCheck, null);
            base.StartServerSide(api);
        }

        private void CmdSkillCheck(IServerPlayer player, int groupId, CmdArgs args)
        {
            String skill = "perception";

            if (args.Length == 0)
            {
                player.SendMessage(groupId, $"Insufficient arguments, must provide skill from this list: {String.Join(",", skillChecks)}",EnumChatType.CommandError);
                return;
            }
            else
            {
                skill = args[0].ToLower();
                if (!skillChecks.Contains(skill))
                {
                    player.SendMessage(groupId, $"Invalid argument [" + skill + "], must provide skill from this list: " + String.Join(", ", skillChecks),EnumChatType.CommandError);
                    return;
                }
            }

            int bonus = 0;
            //int bonus = getClassRollBonus(skill, solplayer.getClassObj());


            String message = " makes a skill check for " + skill + ". They roll: " + rand.Next(0, 20+1) + "/20";

            if (bonus != 0)
            {
                int roll = rand.Next(0, 20+1);
                if (bonus > 0)
                    player.SendMessage(groupId, $"Your roll has been assigned an appropriate class modifier [" + bonus + "]!", EnumChatType.CommandError);

                int bonusroll = roll + bonus;
                message = " makes a skill check for " + skill + ". They roll: " + roll + "+" + bonus + "(" + bonusroll + ")" + "/20";
            }
            player.SendEmote(message, true);
        }


        private void CmdItemAttributes(IServerPlayer player, int groupId, CmdArgs args)
        {
            if (player.Entity.RightHandItemSlot != null && player.Entity.RightHandItemSlot.Itemstack != null && player.Entity.RightHandItemSlot.Itemstack.Attributes != null)
            {
                player.SendMessage(groupId, $"Right Hand: ", EnumChatType.CommandSuccess);
                foreach (var att in player.Entity.RightHandItemSlot.Itemstack.Attributes)
                {
                    player.SendMessage(groupId, $"{att.Key} : {att.Value}", EnumChatType.CommandSuccess);

                }
            }

            if (player.Entity.LeftHandItemSlot != null && player.Entity.LeftHandItemSlot.Itemstack != null && player.Entity.LeftHandItemSlot.Itemstack.Attributes != null)
            {
                player.SendMessage(groupId, $"Left Hand: ", EnumChatType.CommandSuccess);
                foreach (var att in player.Entity.LeftHandItemSlot.Itemstack.Attributes)
                {
                    player.SendMessage(groupId, $"{att.Key} : {att.Value}", EnumChatType.CommandSuccess);
                }
            }
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
