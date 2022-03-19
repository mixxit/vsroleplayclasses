using System;
using vsroleplayclasses.src.Extensions;

namespace vsroleplayclasses.src
{
    public static class WorldLimits
    {
        public static int MAX_LEVEL = 30;
        public static int MAX_STATISTIC = 255;
        public static int MAX_SKILL_LEVEL = 255;

        public static int MaxActiveEffectSlots = 16;
        public static double MAX_PENDING_EXPERIENCE = 255;

        public static float LocalChatDistance = 700;

        // Max level - 10
        public static double GetMaxPendingExperience()
        {
            return PlayerUtils.GetExperienceRequirementForLevel(MAX_LEVEL-10);
        }

        public static double GetMaxExperience()
        {
            return PlayerUtils.GetExperienceRequirementForLevel(MAX_LEVEL);
        }
    }
}