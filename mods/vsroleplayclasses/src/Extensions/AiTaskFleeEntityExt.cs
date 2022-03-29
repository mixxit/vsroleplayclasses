using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.GameContent;

namespace vsroleplayclasses.src.Extensions
{
    public static class AiTaskFleeEntityExt
    {
        public static float GetMoveSpeed(this AiTaskFleeEntity me)
        {
            FieldInfo fieldInfo = typeof(AiTaskFleeEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            return (float)fieldInfo.GetValue(me);
        }

        public static void SetMoveSpeed(this AiTaskFleeEntity me, float speed)
        {
            FieldInfo fieldInfo = typeof(AiTaskFleeEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(me, (float)speed);
        }
    }
}
