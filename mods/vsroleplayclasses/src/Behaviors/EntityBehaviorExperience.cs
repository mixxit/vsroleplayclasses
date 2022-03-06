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
    public class EntityBehaviorExperience : EntityBehavior
    {
        protected double warriorxp;
        protected double clericxp;
        protected double paladinxp;
        protected double rangerxp;
        protected double shadowknightxp;
        protected double druidxp;
        protected double monkxp;
        protected double bardxp;
        protected double roguexp;
        protected double shamanxp;
        protected double necromancerxp;
        protected double wizardxp;
        protected double magicianxp;
        protected double enchanterxp;
        protected double beastlordxp;
        protected double berserkerxp;

        ICoreServerAPI sapi;

        public override string PropertyName() { return "EntityBehaviorExperience"; }

        public EntityBehaviorExperience(Entity entity) : base(entity) { }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            if (entity != null && entity.Api.Side == EnumAppSide.Server)
                sapi = entity.Api as ICoreServerAPI;

            base.Initialize(properties, attributes);

            this.warriorxp = attributes["warriorxp"].AsDouble(0);
            this.clericxp = attributes["clericxp"].AsDouble(0);
            this.paladinxp = attributes["paladinxp"].AsDouble(0);
            this.rangerxp = attributes["rangerxp"].AsDouble(0);
            this.shadowknightxp = attributes["shadowknightxp"].AsDouble(0);
            this.druidxp = attributes["druidxp"].AsDouble(0);
            this.monkxp = attributes["monkxp"].AsDouble(0);
            this.bardxp = attributes["bardxp"].AsDouble(0);
            this.roguexp = attributes["roguexp"].AsDouble(0);
            this.shamanxp = attributes["shamanxp"].AsDouble(0);
            this.necromancerxp = attributes["necromancerxp"].AsDouble(0);
            this.wizardxp = attributes["wizardxp"].AsDouble(0);
            this.magicianxp = attributes["magicianxp"].AsDouble(0);
            this.enchanterxp = attributes["enchanterxp"].AsDouble(0);
            this.beastlordxp = attributes["beastlordxp"].AsDouble(0);
            this.berserkerxp = attributes["berserkerxp"].AsDouble(0);
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
                        
            var expType = EnumAdventuringClass.Warrior;
            if (damageSourceForDeath.Type == EnumDamageType.PiercingAttack)
                expType = EnumAdventuringClass.Ranger;
            if (damageSourceForDeath.Type == EnumDamageType.Poison)
                expType = EnumAdventuringClass.Rogue;

            damageSourceForDeath.SourceEntity.AwardExperience(expType, this.entity.GetExperienceWorth((IServerPlayer)((EntityPlayer)damageSourceForDeath.SourceEntity).Player));
        }
    }
}
