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

        public static double GetExperienceWorth(this Entity me)
        {
            // Only award to players
            if ((me is EntityPlayer))
                return 0D;

            return 1D;
        }
    }
}
