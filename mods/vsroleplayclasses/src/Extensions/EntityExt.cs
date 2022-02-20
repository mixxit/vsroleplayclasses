using projectrarahat.src.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace vsroleplayclasses.src.Extensions
{
    public static class EntityExt
    {
        public static void AwardExperience(this Entity me, EnumAdventuringClass experienceType, double experienceAmount)
        {
            if (experienceType == EnumAdventuringClass.None)
                return;

            // Only award to players
            if (!(me is EntityPlayer))
                return;

            if (experienceAmount < 1)
                return;

            if (((EntityPlayer)me).Player == null)
                return;

            if (!(((EntityPlayer)me).Player is IServerPlayer))
                return;

            ((IServerPlayer)((EntityPlayer)me).Player).GrantExperience(experienceType, experienceAmount);
        }

        public static int GetLevel(this Entity me)
        {
            if (me is EntityPlayer && ((EntityPlayer)me).Player is IServerPlayer)
                return ((IServerPlayer)((EntityPlayer)me).Player).GetLevel();

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
