using projectrarahat.src.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;

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
            api.RegisterCommand("linguamagica", "lists information about lingua magica", "", CmdLinguaMagica, null);
            base.StartServerSide(api);
        }

        private void CmdAbilities(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Abilities:", EnumChatType.OwnMessage);
            foreach(var value in abilityList)
                player.SendMessage(groupId, value.Id + ":" + value.Name, EnumChatType.OwnMessage);
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
