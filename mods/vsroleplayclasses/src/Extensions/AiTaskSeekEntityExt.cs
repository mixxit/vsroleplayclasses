using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.GameContent;

namespace vsroleplayclasses.src.Extensions
{
    public static class AiTaskSeekEntityExt
    {
        public static float GetMoveSpeed(this AiTaskSeekEntity me)
        {
            FieldInfo fieldInfo = typeof(AiTaskSeekEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            return (float)fieldInfo.GetValue(me);
        }

        public static void SetMoveSpeed(this AiTaskSeekEntity me, float speed)
        {
            FieldInfo fieldInfo = typeof(AiTaskSeekEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(me, (float)speed);
        }
    }
}
