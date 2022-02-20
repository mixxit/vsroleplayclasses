using System;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src
{
    public static class WorldLimits
    {
        public static int MAX_LEVEL = 30;
        public static int MAX_STATISTIC = 255;

        public static double GetMaxExperience()
        {
            return PlayerUtils.GetExperienceRequirementForLevel(MAX_LEVEL);
        }
    }
}