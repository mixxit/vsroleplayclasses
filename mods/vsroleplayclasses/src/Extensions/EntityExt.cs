using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using vsroleplayclasses.src.Behaviors;

namespace vsroleplayclasses.src.Extensions
{
    public static class EntityExt
    {
        public static bool IsIServerPlayer(this Entity me)
        {
            if (!(me is EntityPlayer))
                return false;

            if (((EntityPlayer)me).Player == null)
                return false;

            if (!(((EntityPlayer)me).Player is IServerPlayer))
                return false;

            return true;
        }

        public static void TryFinishCast(this Entity me, long targetEntityId)
        {
            if (me.Api.Side != EnumAppSide.Server)
                return;
            if (targetEntityId < 1)
                return;

            var targetEntity = me.World.GetEntityById(targetEntityId);
            if (targetEntity == null)
                return;

            EntityBehaviorCasting ebt = me.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return;

            ebt.TryFinishCast(targetEntity);
        }

        public static bool IsWaitingToCast(this Entity me)
        {
            if (me.Api.Side == EnumAppSide.Client)
            {
                if (me.WatchedAttributes.GetFloat("castingpct", 1.0F) == 1.0F)
                    return true;
                else
                    return false;
            }

            EntityBehaviorCasting ebt = me.GetBehavior("EntityBehaviorCasting") as EntityBehaviorCasting;
            if (ebt == null)
                return false;

            return ebt.IsWaitingToCast();
        }

        public static bool IsInvulerable(this Entity me)
        {
            if ((!me.Alive || me.IsActivityRunning("invulnerable")))
                return true;

            return false;
        }

        public static bool ChangeCurrentHp(this Entity me, Entity sourceEntity, float amount, EnumDamageType type)
        {
            return me.ReceiveDamage(
                new DamageSource() { 
                Source = sourceEntity is EntityPlayer ? EnumDamageSource.Player : EnumDamageSource.Entity,
                SourceEntity = sourceEntity, 
                Type = type
                },
                amount
                );
        }

        public static bool BindToLocation(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return false;

            return me.GetAsIServerPlayer().BindToLocation();
        }

        public static IServerPlayer GetAsIServerPlayer(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return null;

            return ((IServerPlayer)((EntityPlayer)me).Player);
        }

        public static void GrantSmallAmountOfAdventureClassXp(this Entity me, Ability ability)
        {
            if (ability.AdventureClass == AdventureClass.None)
                return;

            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().GrantSmallAmountOfAdventureClassXp(ability);
        }

        public static void SkillUp(this Entity me, Ability ability)
        {
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().SkillUp(ability);
        }

        public static bool GateToBind(this Entity me)
        {
            if (!me.IsIServerPlayer())
                return false;

            return me.GetAsIServerPlayer().GateToBind();
        }

        public static void DecreaseMana(this Entity me, float mana)
        {
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().DecreaseMana(mana);
        }

        public static void AwardExperience(this Entity me, AdventureClass experienceType, double experienceAmount)
        {
            if (experienceType == AdventureClass.None)
                return;

            // Only award to players
            if (!me.IsIServerPlayer())
                return;

            me.GetAsIServerPlayer().GrantExperience(experienceType, experienceAmount);
        }

        public static int GetLevel(this Entity me)
        {
            if (me.IsIServerPlayer())
                return me.GetAsIServerPlayer().GetLevel();

            return 1;
        }

        public static double GetExperienceWorth(this Entity killed, IServerPlayer killer)
        {
            // Only award when killing npcs
            if ((killed is EntityPlayer))
                return 0D;

            return (EntityUtils.GetExperienceRewardAverageForLevel(killed.GetLevel(), killer.GetLevel()));
        }
    }
}
