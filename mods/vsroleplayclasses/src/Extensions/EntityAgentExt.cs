
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Extensions
{
    public static class EntityAgentExt
    {
        public static BlockPos GetInsidePos(this EntityAgent me)
        {
            FieldInfo fieldInfo = typeof(EntityAgent).GetField("insidePos", BindingFlags.NonPublic | BindingFlags.Instance);
            return (BlockPos)fieldInfo.GetValue(me);
        }

        public static Block GetInsideBlock(this EntityAgent me)
        {
            FieldInfo fieldInfo = typeof(EntityAgent).GetField("insideBlock", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Block)fieldInfo.GetValue(me);
        }

        public static void SetInsideBlock(this EntityAgent me, Block block)
        {
            FieldInfo fieldInfo = typeof(EntityAgent).GetField("insideBlock", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(me, (Block)block);
        }
    }
}
