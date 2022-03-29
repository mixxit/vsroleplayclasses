using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.GameContent;

namespace vsroleplayclasses.src.Extensions
{
    public static class AiTaskGotoEntityExt
    {
        public static float GetMoveSpeed(this AiTaskGotoEntity me)
        {
            FieldInfo fieldInfo = typeof(AiTaskGotoEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            return (float)fieldInfo.GetValue(me);
        }

        public static void SetMoveSpeed(this AiTaskGotoEntity me, float speed)
        {
            FieldInfo fieldInfo = typeof(AiTaskGotoEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(me, (float)speed);
        }
    }
}
