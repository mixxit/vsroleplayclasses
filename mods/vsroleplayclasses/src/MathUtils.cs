using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public class MathUtils
    {
        internal static int RandomBetween(int num1, int num2)
        {
            return (int)Randomise.Instance.Random.Next(num1, num2+1);
        }

        internal static float Clamp(float val, float min, float max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        internal static bool Roll(int chance)
        {
            return MathUtils.RandomBetween(0, 100) < chance;
        }
    }
}
