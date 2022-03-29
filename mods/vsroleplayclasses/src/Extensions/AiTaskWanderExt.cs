using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.GameContent;

namespace vsroleplayclasses.src.Extensions
{
    public static class AiTaskWanderExt
    {
        public static float GetMoveSpeed(this AiTaskWander me)
        {
            FieldInfo fieldInfo = typeof(AiTaskWander).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            return (float)fieldInfo.GetValue(me);
        }

        public static void SetMoveSpeed(this AiTaskWander me, float speed)
        {
            FieldInfo fieldInfo = typeof(AiTaskWander).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(me, (float)speed);
        }
    }
}
