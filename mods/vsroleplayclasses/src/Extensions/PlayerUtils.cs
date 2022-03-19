using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src.Extensions
{
    class PlayerUtils
    {
        internal static int GetLevelFromExperience(double experience)
        {
            double classmodifier = 10d;
            double racemodifier = 100d;
            double levelfactor = 1d;

            double level = experience / levelfactor / racemodifier / classmodifier;
            level = Math.Pow(level, 0.25) + 1;
            return (int)Math.Floor(level);
        }

        public static double GetExperienceRequirementForLevel(int level)
        {
            double classmodifier = 10d;
            double racemodifier = 100d;
            double levelfactor = 1d;

            double experiencerequired = (Math.Pow(level - 1, 4)) * classmodifier * racemodifier * levelfactor;
            return experiencerequired;
        }

        public static int GetExperiencePercentage(int currentlevel, double experience)
        {
            double xpneededforcurrentlevel = PlayerUtils.GetExperienceRequirementForLevel((int)(currentlevel));
            double xpneededfornextlevel = PlayerUtils.GetExperienceRequirementForLevel((int)(currentlevel + 1));
            double totalxpneeded = xpneededfornextlevel - xpneededforcurrentlevel;
            double currentxpprogress = experience - xpneededforcurrentlevel;

            double percenttolevel = Math.Floor((currentxpprogress / totalxpneeded) * 100);
            return (int)percenttolevel;
        }

        internal static int GetPercent(double xp, double maxXp)
        {
            double percenttolevel = Math.Floor((xp / maxXp) * 100);
            return (int)percenttolevel;
        }

    }
}
