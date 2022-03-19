using vsroleplayclasses.src.Extensions;
using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Common.Entities;
using vsroleplayclasses.src.Behaviors;
using Vintagestory.API.Client;
using vsroleplayclasses.src.Gui;
using Vintagestory.API.Config;
using vsroleplayclasses.src.Packets;
using System.Collections.Concurrent;

namespace vsroleplayclasses.src.Systems
{
    public class SystemActiveEffects : ModSystem
    {
        ICoreClientAPI capi;
        ICoreServerAPI sapi;
        ICoreAPI api;
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            this.api = api;
            api.Network.RegisterChannel("updateactiveeffectshudpacket").RegisterMessageType<UpdateActiveEffectsHudPacket>();
        }


        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;
            capi.Gui.RegisterDialog(new HudActiveEffects(capi));
            
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;
            api.Event.SaveGameLoaded += new System.Action(this.OnSaveGameLoaded);
            api.Event.GameWorldSave += new System.Action(this.OnSaveGameSaving);
            api.RegisterEntityBehaviorClass("EntityBehaviorSpellEffects", typeof(EntityBehaviorSpellEffects));
            base.StartServerSide(api);
        }

        internal void SendHudUpdatePacket(ConcurrentDictionary<long, ActiveSpellEffect> activeSpellEffects, IServerPlayer player)
        {
            try
            {
                sapi.Network.GetChannel("updateactiveeffectshudpacket").SendPacket(UpdateActiveEffectsHudPacket.From(sapi.World, activeSpellEffects), new IServerPlayer[] { player });
            } catch (Exception)
            {
                // Not registered yet
            }
        }

        /*
        private void CmdEffects(IServerPlayer player, int groupId, CmdArgs args)
        {
            var effects = AbilityTools.GetEffectsFromString(player.Entity);
            foreach (var effect in effects)
            {
                var ability = AbilityTools.GetAbility(player.Entity.World, effect.Value.AbilityId);

                if (ability == null)
                    continue;

                player.SendMessage(GlobalConstants.CurrentChatGroup, ability.Id + ": " + ability.Name + " Duration Left: " + effect.Value.Duration, EnumChatType.CommandSuccess);
            }
        }*/

        private void OnSaveGameLoaded()
        {
            
        }

        private void OnSaveGameSaving()
        {
            
        }


        private void OnGameTick(float tick)
        {

        }
    }
}
