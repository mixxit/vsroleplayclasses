using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using vsroleplayclasses.src.Gui;
using vsroleplayclasses.src.Models;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src
{
    public class AbilityTools
    {
        /*public static Dictionary<int, ActiveSpellEffect> GetEffectsFromString(Entity entity)
        {
            var result = new Dictionary<int, ActiveSpellEffect>();
            var effectsstring = entity.WatchedAttributes.GetString("spelleffects");

            if (String.IsNullOrEmpty(effectsstring))
                return result;

            var rows = effectsstring.Split('|');
            for (int i = 0; i < rows.Count(); i++)
            {
                var data = rows[i].Split(',');
                result.Add(i, new ActiveSpellEffect() { AbilityId = Convert.ToInt64(data[0]), SourceEntityId = Convert.ToInt64(data[1]), Duration = Convert.ToInt64(data[2]) });
            }

            return result;
        }

        public static Dictionary<int, ActiveSpellEffectWithNameAndIcon> GetEffectsWithNameAndIconFromString(Entity entity)
        {
            var result = new Dictionary<int, ActiveSpellEffectWithNameAndIcon>();
            var effectsstring = entity.WatchedAttributes.GetString("spelleffects");

            if (String.IsNullOrEmpty(effectsstring))
                return result;

            var rows = effectsstring.Split('|');
            for (int i = 0; i < rows.Count(); i++)
            {
                var data = rows[i].Split(',');
                var entry = new ActiveSpellEffectWithNameAndIcon()
                {
                    AbilityId = Convert.ToInt64(data[0]),
                    SourceEntityId = Convert.ToInt64(data[1]),
                    Duration = Convert.ToInt64(data[2])
                };

                if (data.Length > 3)
                    entry.Icon = Convert.ToInt32(data[3]);
                if (data.Length > 4)
                    entry.Name = data[4];

                result.Add(i, entry);
            }

            return result;
        }*/

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
                    return 7;
                case TargetType.AECaster:
                    return 6;
                case TargetType.Undead:
                    return 4;
                default:
                    return 100000;
            }
        }

        public static int GetCastTimeMultiplier(TargetType enumValue)
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
                    return 7;
                case TargetType.AECaster:
                    return 6;
                case TargetType.Undead:
                    return 4;
                default:
                    return 100000;
            }
        }

        public static float GetTargetTypeAmountMultiplier(TargetType enumValue)
        {
            switch (enumValue)
            {
                // should never happen
                case TargetType.None:
                    return 1;
                case TargetType.Self:
                    return 7;
                case TargetType.Target:
                    return 6;
                case TargetType.Group:
                    return 5;
                case TargetType.AETarget:
                    return 2;
                case TargetType.AECaster:
                    return 3;
                case TargetType.Undead:
                    return 7;
                default:
                    return 1;
            }
        }

        internal static Ability GetAbility(IWorldAccessor world, long abilityId)
        {
            var mod = world.Api.ModLoader.GetModSystem<SystemAbilities>();
            if (mod == null)
                return null;

            return mod.GetAbilityById(abilityId);
        }
    }
}
