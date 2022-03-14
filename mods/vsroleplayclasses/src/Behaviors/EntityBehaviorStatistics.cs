using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src.Behaviors
{
    public class EntityBehaviorStatistics : EntityBehavior
    {
        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorStatistics"; }

        public EntityBehaviorStatistics(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }

        public override void OnEntityLoaded()
        {
            base.OnEntityLoaded();
            if (entity is EntityItem)
                return;

            RegisterEntityStatisticsChangedListener(entity);
            entity.ResetStatisticState();
        }

        private void RegisterEntityStatisticsChangedListener(Entity entity)
        {
            if (this.entity.Api.Side != EnumAppSide.Server)
                return;

            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                entity.WatchedAttributes.RegisterModifiedListener("stat_" + statType.ToString(), (System.Action)(() => OnEntityStatisticsChanged(entity, statType)));
            }
        }

        public override void OnEntitySpawn()
        {
            base.OnEntitySpawn();
            if (entity is EntityItem)
                return;

            RegisterEntityStatisticsChangedListener(entity);
            entity.ResetStatisticState();
        }

        private void OnEntityStatisticsChanged(Entity entity, StatType type)
        {
            if (type == StatType.Agility || type == StatType.Intelligence || type == StatType.Wisdom)
                entity.ResetMaxMana();
            if (type == StatType.Agility)
                entity.ResetMaxHealth();
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }
    }
}
