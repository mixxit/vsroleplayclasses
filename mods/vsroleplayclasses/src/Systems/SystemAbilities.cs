using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.Common;
using Vintagestory.Server;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Gui;
using vsroleplayclasses.src.Items;
using vsroleplayclasses.src.Packets;

namespace vsroleplayclasses.src.Systems
{
    public class SystemAbilities : ModSystem
    {
        ConcurrentDictionary<long,Ability> abilityList;
        ICoreServerAPI serverApi;
        GuiDialogMemoriseAbility memorisateAbilityDialog;
        ICoreClientAPI capi;
        ICoreAPI api;

        public override void Start(ICoreAPI api)
        {
            this.api = api;
            base.Start(api);

            api.Network
                .RegisterChannel("castabilityinmemoryposition")
                .RegisterMessageType<CastAbilityInMemoryPositionPacket>();

            api.RegisterItemClass("abilitybook", typeof(AbilityBookItem));
            api.RegisterItemClass("abilityscroll", typeof(AbilityScrollItem));
            api.RegisterItemClass("runeofpower", typeof(RuneOfPowerItem));
            api.RegisterItemClass("crushedpower", typeof(CrushedPowerItem));
            api.RegisterItemClass("inkwell", typeof(InkwellItem));
            api.RegisterItemClass("inkwellempty", typeof(InkwellEmptyItem));
            api.RegisterItemClass("inkwellandquill", typeof(InkwellAndQuillItem));
            api.RegisterItemClass("inkwellandquillempty", typeof(InkwellAndQuillEmptyItem));
            api.RegisterItemClass("runicinkwellandquill", typeof(RunicInkwellAndQuillItem));
            api.RegisterItemClass("runicinkwell", typeof(RunicInkwellItem));
        }


        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
            capi.Input.RegisterHotKey("memoriseability", "Allows memorisation of abilities", GlKeys.L, HotkeyType.GUIOrOtherControls);
            capi.Input.SetHotKeyHandler("memoriseability", ToggleMemorisationGui);
            capi.Input.RegisterHotKey("useability1", "Uses memorised ability #1", GlKeys.Number1, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.RegisterHotKey("useability2", "Uses memorised ability #2", GlKeys.Number2, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.RegisterHotKey("useability3", "Uses memorised ability #3", GlKeys.Number3, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.RegisterHotKey("useability4", "Uses memorised ability #4", GlKeys.Number4, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.RegisterHotKey("useability5", "Uses memorised ability #5", GlKeys.Number5, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.RegisterHotKey("useability6", "Uses memorised ability #6", GlKeys.Number6, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.RegisterHotKey("useability7", "Uses memorised ability #7", GlKeys.Number7, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.RegisterHotKey("useability8", "Uses memorised ability #8", GlKeys.Number8, HotkeyType.GUIOrOtherControls, true,false,false);
            capi.Input.SetHotKeyHandler("useability1", UseAbilityKey1);
            capi.Input.SetHotKeyHandler("useability2", UseAbilityKey2);
            capi.Input.SetHotKeyHandler("useability3", UseAbilityKey3);
            capi.Input.SetHotKeyHandler("useability4", UseAbilityKey4);
            capi.Input.SetHotKeyHandler("useability5", UseAbilityKey5);
            capi.Input.SetHotKeyHandler("useability6", UseAbilityKey6);
            capi.Input.SetHotKeyHandler("useability7", UseAbilityKey7);
            capi.Input.SetHotKeyHandler("useability8", UseAbilityKey8);
        }

        private bool UseAbilityKey1(KeyCombination keyCombination)
        {
            return UseAbility(1);
        }

        private bool UseAbility(int position)
        {
            capi.Network.GetChannel("castabilityinmemoryposition").SendPacket(new CastAbilityInMemoryPositionPacket()
            {
                Position = position
            });
            return true;
        }

        private bool UseAbilityKey2(KeyCombination keyCombination)
        {
            return UseAbility(2);
        }
        private bool UseAbilityKey3(KeyCombination keyCombination)
        {
            return UseAbility(3);
        }
        private bool UseAbilityKey4(KeyCombination keyCombination)
        {
            return UseAbility(4);
        }
        private bool UseAbilityKey5(KeyCombination keyCombination)
        {
            return UseAbility(5);
        }
        private bool UseAbilityKey6(KeyCombination keyCombination)
        {
            return UseAbility(6);
        }
        private bool UseAbilityKey7(KeyCombination keyCombination)
        {
            return UseAbility(7);
        }
        private bool UseAbilityKey8(KeyCombination keyCombination)
        {
            return UseAbility(8);
        }

        private bool ToggleMemorisationGui(KeyCombination comb)
        {
            if (memorisateAbilityDialog == null)
                memorisateAbilityDialog = new GuiDialogMemoriseAbility(capi);

            if (memorisateAbilityDialog.IsOpened()) memorisateAbilityDialog.TryClose();
            else memorisateAbilityDialog.TryOpen();

            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            serverApi = api;
            api.Event.SaveGameLoaded += new System.Action(this.OnSaveGameLoaded);
            api.Event.GameWorldSave += new System.Action(this.OnSaveGameSaving);
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlayingServer);
            api.Event.RegisterGameTickListener(new Action<float>(this.OnCastingTimerTick), 500);

            base.StartServerSide(api);
            api.RegisterCommand("forcecast", "force casts an ability", "", CmdForceCast, "root");
            api.RegisterCommand("abilities", "lists information about abilities", "", CmdAbilities, null);
            api.RegisterCommand("forcescrollability", "forces a scroll abillity", "", CmdForceScrollAbility, "root");
            api.RegisterCommand("forceabilitybookability", "forces a abilitybook abillity in a slot", "", CmdForceAbilitybookAbility, "root");
            //api.RegisterCommand("linguamagica", "lists information about lingua magica", "", CmdLinguaMagica, null);
            api.Network.GetChannel("castabilityinmemoryposition")
                .SetMessageHandler<CastAbilityInMemoryPositionPacket>(OnCastAbilityInMemoryPosition)
            ;
        }

        private void OnCastingTimerTick(float obj)
        {
            
        }

        internal Ability GetAbilityById(long id)
        {
            if (!this.abilityList.ContainsKey(id))
                return null;

            return this.abilityList[id];
        }

        private void OnCastAbilityInMemoryPosition(IServerPlayer castingPlayer, CastAbilityInMemoryPositionPacket networkMessage)
        {
            var ability = castingPlayer.GetAbilityInMemoryPosition(networkMessage.Position);
            if (ability == null)
            {
                castingPlayer.SendMessage(GlobalConstants.CurrentChatGroup, $"Ability not found", EnumChatType.CommandError);
                return;
            }

            if (castingPlayer.GetMana() < ability.GetManaCost())
            {
                castingPlayer.SendMessage(GlobalConstants.CurrentChatGroup, $"Insufficient Mana", EnumChatType.CommandError);
                return;
            }

            castingPlayer.SendMessage(GlobalConstants.CurrentChatGroup, $"Casting {ability.Name}", EnumChatType.CommandSuccess);
            ability.StartCast(castingPlayer.Entity);
            
        }

        private void CmdForceCast(IServerPlayer player, int groupId, CmdArgs args)
        {
            if (args.Length < 1)
            {
                player.SendMessage(groupId, $"Missing argument (abilityid)", EnumChatType.CommandError);
                return;
            }

            if (!abilityList.ContainsKey(Convert.ToInt64(args[0])))
            {
                player.SendMessage(groupId, $"Ability ID not found", EnumChatType.CommandError);
                return;
            }

            var ability = abilityList[Convert.ToInt64(args[0])];
            if (ability == null)
            {
                player.SendMessage(groupId, $"Ability ID not found", EnumChatType.CommandError);
                return;
            }

            ability.StartCast(player.Entity);
        }

        private void CmdAbilities(IServerPlayer player, int groupId, CmdArgs args)
        {
            player.SendMessage(groupId, "Abilities:", EnumChatType.OwnMessage);
            foreach(var value in abilityList.Values)
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

        internal long TryCreateAbility(IServerPlayer player, ItemStack itemstack)
        {
            if (itemstack == null)
                return 0;

            if (!(itemstack.Item is AbilityScrollItem))
                return 0;

            if (((AbilityScrollItem)itemstack.Item).IsAbilityScribed(itemstack))
                return ((AbilityScrollItem)itemstack.Item).GetScribedAbilityId(itemstack);

            if (((AbilityScrollItem)itemstack.Item).HasSpareRuneSlot(itemstack))
                return 0;

            var spellEffectIndex = ((AbilityScrollItem)itemstack.Item).GetWordOfPower<SpellEffectIndex>(itemstack);
            var spellEffectType = ((AbilityScrollItem)itemstack.Item).GetWordOfPower<SpellEffectType>(itemstack);
            var targetType = ((AbilityScrollItem)itemstack.Item).GetWordOfPower<TargetType>(itemstack);
            var resistType = ((AbilityScrollItem)itemstack.Item).GetWordOfPower<ResistType>(itemstack);
            var spellPolarity = ((AbilityScrollItem)itemstack.Item).GetWordOfPower<SpellPolarity>(itemstack);
            var powerLevel = ((AbilityScrollItem)itemstack.Item).GetWordOfPower<PowerLevel>(itemstack);

            return TryCreateAbilityByRunes(
                player,
                spellEffectIndex,
                spellEffectType,
                resistType,
                targetType,
                spellPolarity,
                powerLevel
                
                );
        }

        private long TryCreateAbilityByRunes(IServerPlayer player,
            SpellEffectIndex spellEffectIndex,
            SpellEffectType spellEffect,
            ResistType resistType,
            TargetType targetType,
            SpellPolarity spellPolarity,
            PowerLevel powerLevel
            )
        {
            if (player == null)
                return 0;

            long returnAbilityId = GetAbilityByRunes(spellEffectIndex,
            spellEffect,
            resistType,
            targetType,
            spellPolarity,
            powerLevel);

            if (returnAbilityId > 0)
                return returnAbilityId;

            /*if (!IsValidMagicaPowerCombination(spellEffectIndex,
            spellEffect,
            resistType,
            targetType,
            spellPolarity,
            powerLevel))
                return 0;*/

            return CreateAbility(player, spellEffectIndex,
            spellEffect,
            resistType,
            targetType,
            spellPolarity,
            powerLevel);
        }

        private long CreateAbility(IServerPlayer player, SpellEffectIndex spellEffectIndex,
            SpellEffectType spellEffect,
            ResistType resistType,
            TargetType targetType,
            SpellPolarity spellPolarity,
            PowerLevel powerLevel)
        {
            long nextKey = 1;
            if (this.abilityList.Keys.Count > 0)
                nextKey = (this.abilityList.Keys.Max() + 1);

            var ability = this.abilityList.GetOrAdd(nextKey, Ability.Create(nextKey,PlayerNameUtils.GetFullRoleplayNameAsDisplayFormat(player.Entity), player.PlayerUID, spellEffectIndex, spellEffect, resistType,targetType, spellPolarity, powerLevel));
            if (ability == null)
                return 0;

            return ability.Id;
        }

        private long GetAbilityByRunes(
            SpellEffectIndex spellEffectIndex,
            SpellEffectType spellEffect,
            ResistType resistType,
            TargetType targetType,
            SpellPolarity spellPolarity,
            PowerLevel powerLevel
            )
        {
            var ability = this.abilityList.Values.FirstOrDefault(
                e => 
                e.SpellEffectIndex == spellEffectIndex &&
                e.SpellEffect == spellEffect &&
                e.ResistType == resistType &&
                e.TargetType == targetType &&
                e.SpellPolarity == spellPolarity &&
                e.PowerLevel == powerLevel
                );

            if (ability == null)
                return 0;

            return ability.Id;
        }

        private bool AbilityExists(long abilityId)
        {
            return this.abilityList[abilityId] != null;
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


        /*private void CmdLinguaMagica(IServerPlayer player, int groupId, CmdArgs args)
        {
            var lingua = LinguaMagica.ToCommaSeperatedString();
            player.SendMessage(groupId, "Lingua Magica:", EnumChatType.OwnMessage);
            player.SendMessage(groupId, lingua, EnumChatType.OwnMessage);
        }*/

        private void OnPlayerNowPlayingServer(IServerPlayer player)
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

            abilityList = new ConcurrentDictionary<long, Ability>();
            var temporaryAbilityList = data == null || data.Length == 0 ? PreloadSpells() : SerializerUtil.Deserialize<List<Ability>>(data);
            foreach (var ability in temporaryAbilityList)
                abilityList.GetOrAdd(ability.Id, ability);

        }

        private List<Ability> PreloadSpells()
        {
            return new List<Ability>();
        }

        private void OnSaveGameSaving()
        {
            serverApi.WorldManager.SaveGame.StoreData("vsroleplayclasses_abilities", SerializerUtil.Serialize(abilityList.Values.ToList()));
        }


        private void OnGameTick(float tick)
        {

        }
    }
}
