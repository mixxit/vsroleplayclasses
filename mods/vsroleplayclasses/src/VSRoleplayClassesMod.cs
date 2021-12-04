using Foundation.Extensions;
using projectrarahat.src.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace vsroleplayclasses.src
{
    public class VSRoleplayClassesMod : ModSystem
    {
        List<CharacterClass> characterClassesItems;

        public override void StartPre(ICoreAPI api)
        {
            VSRoleplayClassesModConfigFile.Current = api.LoadOrCreateConfig<VSRoleplayClassesModConfigFile>(typeof(VSRoleplayClassesMod).Name+".json");
            

            api.World.Config.SetBool("loadGearNonDress", VSRoleplayClassesModConfigFile.Current.LoadGearNonDress);

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

            api.Event.SaveGameLoaded += new Vintagestory.API.Common.Action(this.OnSaveGameLoaded);
            api.Event.PlayerNowPlaying += new PlayerDelegate(this.OnPlayerNowPlaying);
            // Check every 8 seconds
            api.World.RegisterGameTickListener(OnGameTick, 8000);
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
            if (player.IsGrantedInitialItems())
                return;

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
        public bool LoadGearNonDress = true;
    }
}
