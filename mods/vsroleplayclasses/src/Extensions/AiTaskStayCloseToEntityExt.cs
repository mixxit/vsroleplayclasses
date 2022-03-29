using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.GameContent;

namespace vsroleplayclasses.src.Extensions
{
    public static class AiTaskStayCloseToEntityExt
    {
        public static float GetMoveSpeed(this AiTaskStayCloseToEntity me)
        {
            FieldInfo fieldInfo = typeof(AiTaskStayCloseToEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            return (float)fieldInfo.GetValue(me);
        }

        public static void SetMoveSpeed(this AiTaskStayCloseToEntity me, float speed)
        {
            FieldInfo fieldInfo = typeof(AiTaskStayCloseToEntity).GetField("moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(me, (float)speed);
        }
    }
}
