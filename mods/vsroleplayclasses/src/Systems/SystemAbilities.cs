using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Items;

namespace vsroleplayclasses.src.Systems
{
    public class SystemAbilities : ModSystem
    {
        List<Ability> abilityList;
        ICoreServerAPI serverApi;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterItemClass("abilitybook", typeof(AbilityBookItem));
            api.RegisterItemClass("abilityscroll", typeof(AbilityScrollItem));
            api.RegisterItemClass("runeofpower", typeof(RuneOfPowerItem));
            api.RegisterItemClass("crushedpower", typeof(CrushedPowerItem));
            api.RegisterItemClass("inkwell", typeof(InkwellItem));
            api.RegisterItemClass("inkwellempty", typeof(InkwellEmptyItem));
            api.RegisterItemClass("inkwellandquill", typeof(InkwellAndQuillItem));
            api.RegisterItemClass("runicinkwellandquill", typeof(RunicInkwellAndQuillItem));
            api.RegisterItemClass("runicinkwell", typeof(RunicInkwellItem));
        }


        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            serverApi = api;
            api.Event.SaveGameLoaded += new System.Action(this.OnSaveGameLoaded);
            api.Event.GameWorldSave += new System.Action(this.OnSaveGameSaving);
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);
            api.RegisterCommand("abilities", "lists information about abilities", "", CmdAbilities, null);
            api.RegisterCommand("forcescrollability", "forces a scroll abillity", "", CmdForceScrollAbility, "root");
            api.RegisterCommand("forceabilitybookability", "forces a abilitybook abillity in a slot", "", CmdForceAbilitybookAbility, "root");
            api.RegisterCommand("linguamagica", "lists information about lingua magica", "", CmdLinguaMagica, null);
            base.StartServerSide(api);
        }

        private void CmdAbilities(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Abilities:", EnumChatType.OwnMessage);
            foreach(var value in abilityList)
                player.SendMessage(groupId, value.Id + ":" + value.Name, EnumChatType.OwnMessage);
        }

        private void CmdForceAbilitybookAbility(IServerPlayer player, int groupId, CmdArgs args)
        {
            if (args.Length < 2)
            {
                player.SendMessage(groupId, $"Missing argument (slotid, abilityid)", EnumChatType.CommandError);
                return;
            }

            try
            {
                int slotId = Convert.ToInt32(args[0]);
                if (slotId < 1 || slotId > 8)
                {
                    player.SendMessage(groupId, $"Invalid slotid", EnumChatType.CommandError);
                    return;
                }

                long abilityId = Convert.ToInt64(args[1]);
                if (!AbilityExists(abilityId))
                {
                    player.SendMessage(groupId, $"Invalid abilityid", EnumChatType.CommandError);
                    return;
                }

                TryForceSpellbookAbilityInSlot(player.Entity.RightHandItemSlot, slotId, abilityId);
                TryForceSpellbookAbilityInSlot(player.Entity.LeftHandItemSlot, slotId, abilityId);

                player.SendMessage(groupId, $"Attempt to force scribe ability completed, please check spellbook", EnumChatType.CommandSuccess);
            }
            catch (Exception)
            {
                player.SendMessage(groupId, $"Invalid argument (slotid, abilityid)", EnumChatType.CommandError);
                return;
            }
        }

        private bool AbilityExists(long abilityId)
        {
            return this.abilityList.Any(e => e.Id == abilityId);
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
                long abilityId = Convert.ToInt64(args[0]);
                if (!AbilityExists(abilityId))
                {
                    player.SendMessage(groupId, $"Invalid abilityid", EnumChatType.CommandError);
                    return;
                }

                TryForceAbilityScrollInSlot(player.Entity.RightHandItemSlot, abilityId);
                TryForceAbilityScrollInSlot(player.Entity.LeftHandItemSlot, abilityId);

                player.SendMessage(groupId, $"Attempt to force scribe ability completed, please check ability scroll", EnumChatType.CommandSuccess);
            }
            catch (Exception)
            {
                player.SendMessage(groupId, $"Invalid argument (abilityid)", EnumChatType.CommandError);
                return;
            }
        }

        private void TryForceSpellbookAbilityInSlot(ItemSlot itemSlot, int slotId, long abilityId)
        {
            if (itemSlot.Itemstack == null || itemSlot.Itemstack.Item == null || (itemSlot.Itemstack.Item as AbilityBookItem) == null)
                return;

            (itemSlot.Itemstack.Item as AbilityBookItem).SetScribedAbility(itemSlot.Itemstack, slotId, abilityId);
            itemSlot.MarkDirty();
        }

        private void TryForceAbilityScrollInSlot(ItemSlot itemSlot, long abilityId)
        {
            if (itemSlot.Itemstack == null || itemSlot.Itemstack.Item == null || (itemSlot.Itemstack.Item as AbilityScrollItem) == null)
                return;

            // already scribed
            if ((itemSlot.Itemstack.Item as AbilityScrollItem).IsAbilityScribed(itemSlot.Itemstack))
                return;

            (itemSlot.Itemstack.Item as AbilityScrollItem).SetScribedAbility(itemSlot.Itemstack, abilityId);
            itemSlot.MarkDirty();
        }


        private void CmdLinguaMagica(IServerPlayer player, int groupId, CmdArgs args)
        {
            var lingua = LinguaMagica.ToCommaSeperatedString();
            player.SendMessage(groupId, "Lingua Magica:", EnumChatType.OwnMessage);
            player.SendMessage(groupId, lingua, EnumChatType.OwnMessage);
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
            player.ResetExperience();
        }

        private void OnSaveGameLoaded()
        {
            byte[] data = serverApi.WorldManager.SaveGame.GetData("vsroleplayclasses_abilities");

            abilityList = data == null || data.Length == 0 ? PreloadSpells() : SerializerUtil.Deserialize<List<Ability>>(data);
        }

        private List<Ability> PreloadSpells()
        {
            return new List<Ability>();
        }

        private void OnSaveGameSaving()
        {
            serverApi.WorldManager.SaveGame.StoreData("vsroleplayclasses_abilities", SerializerUtil.Serialize(abilityList));
        }


        private void OnGameTick(float tick)
        {

        }
    }
}
