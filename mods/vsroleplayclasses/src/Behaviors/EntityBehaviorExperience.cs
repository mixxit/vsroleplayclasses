using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorExperience : EntityBehavior
    {
        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorExperience"; }

        public EntityBehaviorExperience(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            base.OnEntityDeath(damageSourceForDeath);

            if (sapi == null)
                return;

            if (damageSourceForDeath.SourceEntity == null)
                return;

            if (!(damageSourceForDeath.SourceEntity is EntityPlayer))
                return;

            if (!(((EntityPlayer)damageSourceForDeath.SourceEntity).Player is IServerPlayer))
                return;

            damageSourceForDeath.SourceEntity.AwardPendingExperience(this.entity.GetExperienceWorth((IServerPlayer)((EntityPlayer)damageSourceForDeath.SourceEntity).Player));
        }

        public override void OnEntityLoaded()
        {
            base.OnEntityLoaded();
            if (entity is EntityItem)
                return;

            RegisterEntityClassLevelChangedListener(entity);
        }

        private void RegisterEntityClassLevelChangedListener(Entity entity)
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            foreach (AdventureClass adventureClass in Enum.GetValues(typeof(AdventureClass)))
            {
                entity.WatchedAttributes.RegisterModifiedListener(adventureClass.ToString().ToLower() + "level", (System.Action)(() => OnEntityAdventureLevelChanged(entity, adventureClass)));
            }
        }

        public override void OnEntitySpawn()
        {
            base.OnEntitySpawn();
            if (entity is EntityItem)
                return;

            RegisterEntityClassLevelChangedListener(entity);
        }

        private void OnEntityAdventureLevelChanged(Entity entity, AdventureClass type)
        {
            if (entity.IsIServerPlayer())
            {
                entity.GetAsIServerPlayer().SendMessage(GlobalConstants.InfoLogChatGroup, $"Your {type.ToString()} adventure level has changed {entity.GetLevel(type)}!", EnumChatType.CommandSuccess);
            }
            entity.ResetMaxMana();
            entity.ResetMaxHealth();
            entity.ResetMaxSkills();
        }
    }
}
