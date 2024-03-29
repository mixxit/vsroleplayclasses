﻿using System;
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
    public class EntityBehaviorSkillable : EntityBehavior
    {
        protected long abilityId;

        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorSkillable"; }

        public EntityBehaviorSkillable(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);
        }
        
        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {
            base.OnEntityReceiveDamage(damageSource, ref damage);

            // Allow receiving skill xp for defense if being damaged
            if (this.entity is EntityPlayer)
            {
                if (GetMeleeDamageSourceTypes().Contains((ExtendedEnumDamageType)damageSource.Type))
                    this.entity.TryIncreaseSkill(SkillType.Defense, 1);
            }

            // Allow receiving skill xp for players attacking and source is the actual player not a ranged entity
            if (damageSource.Source == EnumDamageSource.Player && damageSource.SourceEntity != null && damageSource.SourceEntity is EntityPlayer && damageSource.SourceEntity.Alive)
            {
                if (GetMeleeDamageSourceTypes().Contains((ExtendedEnumDamageType)damageSource.Type))
                    damageSource.SourceEntity.TryIncreaseSkill(SkillUtils.GetSkillTypeFromDamageType((ExtendedEnumDamageType)damageSource.Type,false), 1);
            }

            // Allow receiving skill xp for players attacking and source is an entity like an arrow
            if (damageSource.Source == EnumDamageSource.Entity && damageSource.SourceEntity != null && damageSource.SourceEntity is EntityPlayer && damageSource.SourceEntity.Alive)
            {
                if (GetRangedDamageSourceTypes().Contains((ExtendedEnumDamageType)damageSource.Type))
                {
                    if (GetMeleeDamageSourceTypes().Contains((ExtendedEnumDamageType)damageSource.Type))
                        damageSource.SourceEntity.TryIncreaseSkill(SkillUtils.GetSkillTypeFromDamageType((ExtendedEnumDamageType)damageSource.Type, true), 1);
                }
            }

            if (damageSource?.SourceEntity is EntityPlayer)
            {
                damageSource?.SourceEntity.GrantSmallAmountOfPendingExperience();
            }
            if (damageSource.Source == EnumDamageSource.Internal && damageSource.Type == EnumDamageType.Heal && entity is EntityPlayer)
            {
                entity.GrantSmallAmountOfPendingExperience();
            }
        }

        private List<ExtendedEnumDamageType> GetMeleeDamageSourceTypes()
        {
            return new List<ExtendedEnumDamageType>()
            {
                ExtendedEnumDamageType.SlashingAttack,
                ExtendedEnumDamageType.BluntAttack,
                ExtendedEnumDamageType.PiercingAttack
            };
        }

        private List<ExtendedEnumDamageType> GetRangedDamageSourceTypes()
        {
            return new List<ExtendedEnumDamageType>()
            {
                ExtendedEnumDamageType.BluntAttack,
                ExtendedEnumDamageType.PiercingAttack
            };
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
        }
    }
}
