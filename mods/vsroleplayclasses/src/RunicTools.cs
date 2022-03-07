using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using vsroleplayclasses.src.Items;

namespace vsroleplayclasses.src
{
    public static class RunicTools
    {
        internal static T GetWordOfPowerFromQuillItem<T>(RunicInkwellAndQuillItem item) where T : Enum
        {
            var magicaString = item.Code.ToString().Replace("vsroleplayclasses", "").Replace(":", "").Replace(item.Class, "").Replace("-", "");
            if (String.IsNullOrEmpty(magicaString))
                return default(T);

            foreach (T magicPowerType in Enum.GetValues(typeof(T)))
            {
                var magicaPowerConverted = ConvertMagicPowerType<T>(magicPowerType);
                if (magicaPowerConverted.ToString().ToLower().Equals(magicaString))
                    return (T)magicPowerType;
            }

            return default(T);
        }

        private static MagicWord ConvertMagicPowerType<T>(T magicPowerType) where T : Enum
        {
            switch (typeof(T).Name)
            {
                case "SpellEffectIndex":
                    return GetSpellEffectIndexMagicWord((SpellEffectIndex)(object)magicPowerType);
                case "SpellEffectType":
                    return GetSpellEffectTypeMagicWord((SpellEffectType)(object)magicPowerType);
                case "TargetType":
                    return GetTargetTypeMagicWord((TargetType)(object)magicPowerType);
                case "ResistType":
                    return GetResistTypeMagicWord((ResistType)(object)magicPowerType);
                case "SpellPolarity":
                    return GetSpellPolarityMagicWord((SpellPolarity)(object)magicPowerType);
                case "PowerLevel":
                    return GetPowerLevelMagicWord((PowerLevel)(object)magicPowerType);
            }

            return MagicWord.Nul;
        }

        private static MagicWord GetSpellEffectIndexMagicWord(SpellEffectIndex enumValue)
        {
            switch (enumValue)
            {
                case SpellEffectIndex.None:
                    return MagicWord.Nul;
                case SpellEffectIndex.Vanish:
                    return MagicWord.Set;
                case SpellEffectIndex.Dispell_Sight:
                    return MagicWord.El;
                case SpellEffectIndex.Direct_Damage:
                    return MagicWord.Cal;
                case SpellEffectIndex.Heal_Cure:
                    return MagicWord.Meth;
                default:
                    return MagicWord.Nul;
            }
        }

        private static MagicWord GetSpellEffectTypeMagicWord(SpellEffectType enumValue)
        {
            switch (enumValue)
            {

                case SpellEffectType.None:
                    return MagicWord.Nul;
                case SpellEffectType.BindAffinity:
                    return MagicWord.Maw;
                case SpellEffectType.Gate:
                    return MagicWord.Xen;
                case SpellEffectType.CurrentHP:
                    return MagicWord.Sang;
                default:
                    return MagicWord.Nul;
            }
        }

        private static MagicWord GetTargetTypeMagicWord(TargetType enumValue)
        {
            switch (enumValue)
            {
                case TargetType.None:
                    return MagicWord.Nul;
                case TargetType.Self:
                    return MagicWord.Swe;
                case TargetType.Target:
                    return MagicWord.Tar;
                case TargetType.Group:
                    return MagicWord.Krup;
                case TargetType.AETarget:
                    return MagicWord.Expa;
                case TargetType.Undead:
                    return MagicWord.Ghu;
                default:
                    return MagicWord.Nul;
            }
        }

        private static MagicWord GetResistTypeMagicWord(ResistType enumValue)
        {
            switch (enumValue)
            {
                case ResistType.None:
                    return MagicWord.Nul;
                case ResistType.Magic:
                    return MagicWord.Mana;
                case ResistType.Fire:
                    return MagicWord.Fyr;
                case ResistType.Cold:
                    return MagicWord.Isti;
                case ResistType.Poison:
                    return MagicWord.Xon;
                case ResistType.Disease:
                    return MagicWord.Xul;
                case ResistType.Chromatic:
                    return MagicWord.Duo;
                case ResistType.Prismatic:
                    return MagicWord.Poly;
                case ResistType.Physical:
                    return MagicWord.Karr;
                case ResistType.Corruption:
                    return MagicWord.Mal;
                default:
                    return MagicWord.Nul;
            }
        }

        private static MagicWord GetSpellPolarityMagicWord(SpellPolarity enumValue)
        {
            switch (enumValue)
            {
                case SpellPolarity.None:
                    return MagicWord.Nul;
                case SpellPolarity.Positive:
                    return MagicWord.Scu;
                case SpellPolarity.Negative:
                    return MagicWord.Rew;
                default:
                    return MagicWord.Nul;
            }
        }

        private static MagicWord GetPowerLevelMagicWord(PowerLevel enumValue)
        {
            switch (enumValue)
            {
                case PowerLevel.XV:
                    return MagicWord.Omni;
                case PowerLevel.XIV:
                    return MagicWord.Sul;
                case PowerLevel.XIII:
                    return MagicWord.Prima;
                case PowerLevel.XII:
                    return MagicWord.Ra;
                case PowerLevel.XI:
                    return MagicWord.Zul;
                case PowerLevel.X:
                    return MagicWord.Vul;
                case PowerLevel.IX:
                    return MagicWord.Thul;
                case PowerLevel.VIII:
                    return MagicWord.Deva;
                case PowerLevel.VII:
                    return MagicWord.Cela;
                case PowerLevel.VI:
                    return MagicWord.Eth;
                case PowerLevel.V:
                    return MagicWord.Demi;
                case PowerLevel.IV:
                    return MagicWord.Myth;
                case PowerLevel.III:
                    return MagicWord.Mas;
                case PowerLevel.II:
                    return MagicWord.Adus;
                case PowerLevel.I:
                    return MagicWord.Init;
                case PowerLevel.None:
                    return MagicWord.Nul;
                default:
                    return MagicWord.Nul;
            }
        }
    }
}
