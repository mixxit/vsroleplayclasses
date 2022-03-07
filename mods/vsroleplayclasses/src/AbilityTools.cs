using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public class AbilityTools
    {
        public static float GetTargetTypeManaCostMultiplier(TargetType enumValue)
        {
            switch (enumValue)
            {
                // should never happen
                case TargetType.None:
                    return 100000;
                case TargetType.Self:
                    return 1;
                case TargetType.Target:
                    return 2;
                case TargetType.Group:
                    return 4;
                case TargetType.AETarget:
                    return 6;
                case TargetType.Undead:
                    return 4;
                default:
                    return 10;
            }
        }

        public static float GetTargetTypeDamageAmountMultiplier(TargetType enumValue)
        {
            switch (enumValue)
            {
                // should never happen
                case TargetType.None:
                    return 6;
                case TargetType.Self:
                    return 6;
                case TargetType.Target:
                    return 6;
                case TargetType.Group:
                    return 1;
                case TargetType.AETarget:
                    return 1;
                case TargetType.Undead:
                    return 7;
                default:
                    return 1;
            }
        }
    }
}
