using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using vsroleplayclasses.src.Behaviors;
using vsroleplayclasses.src.Entities;
using vsroleplayclasses.src.Extensions;
using vsroleplayclasses.src.Gui;
using vsroleplayclasses.src.Items;
using vsroleplayclasses.src.Packets;

namespace vsroleplayclasses.src.Systems
{
    public class SystemSpellCrafting : ModSystem
    {
        ICoreAPI api;

        public override void Start(ICoreAPI api)
        {
            this.api = api;
            base.Start(api);

            api.RegisterItemClass("abilitybook", typeof(AbilityBookItem));
            api.RegisterItemClass("abilityscroll", typeof(AbilityScrollItem));
            api.RegisterItemClass("runeofpower", typeof(RuneOfPowerItem));
            api.RegisterItemClass("crushedpower", typeof(CrushedPowerItem));
            api.RegisterItemClass("inkwell", typeof(InkwellItem));
            api.RegisterItemClass("inkwellempty", typeof(InkwellEmptyItem));
            api.RegisterItemClass("inkwellandquill", typeof(InkwellAndQuillItem));
            api.RegisterItemClass("inkwellandquillempty", typeof(InkwellAndQuillEmptyItem));
            api.RegisterItemClass("runicinkwellandquill", typeof(RunicInkwellAndQuillItem));
            api.RegisterItemClass("runicinkwell", typeof(RunicInkwellItem));
        }
    }
}
