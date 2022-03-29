using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using vsroleplayclasses.src.Items;
using vsroleplayclasses.src.Models;

namespace vsroleplayclasses.src
{
    public static class RunicTools
    {
        public static T GetWordOfPowerFromQuillItem<T>(RunicInkwellAndQuillItem item) where T : Enum
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

        public static MagicWord ConvertMagicPowerType<T>(T magicPowerType) where T : Enum
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
                case "PowerLevel":
                    return GetPowerLevelMagicWord((PowerLevel)(object)magicPowerType);
                case "AdventureClass":
                    return GetAdventureClassMagicWord((AdventureClass)(object)magicPowerType);
            }

            return MagicWord.Nul;
        }

        public static MagicWord GetAdventureClassMagicWord(AdventureClass adventureClass)
        {
            switch (adventureClass)
            {
                case AdventureClass.None:
                    return MagicWord.Nul;
                case AdventureClass.Warrior:
                    return MagicWord.Tor;
                case AdventureClass.Cleric:
                    return MagicWord.Deus;
                case AdventureClass.Paladin:
                    return MagicWord.Val;
                case AdventureClass.Ranger:
                    return MagicWord.Cara;
                case AdventureClass.Shadowknight:
                    return MagicWord.Rew;
                case AdventureClass.Druid:
                    return MagicWord.Nato;
                case AdventureClass.Monk:
                    return MagicWord.Sek;
                case AdventureClass.Bard:
                    return MagicWord.Lum;
                case AdventureClass.Rogue:
                    return MagicWord.Tri;
                case AdventureClass.Shaman:
                    return MagicWord.Mys;
                case AdventureClass.Necromancer:
                    return MagicWord.Jar;
                case AdventureClass.Wizard:
                    return MagicWord.Flam;
                case AdventureClass.Magician:
                    return MagicWord.Ex;
                case AdventureClass.Enchanter:
                    return MagicWord.Azul;
                case AdventureClass.Beastlord:
                    return MagicWord.Reg;
                case AdventureClass.Berserker:
                    return MagicWord.Hek;
                default:
                    return MagicWord.Nul;
            }
        }

        public static MagicWord GetSpellEffectIndexMagicWord(SpellEffectIndex enumValue)
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
                case SpellEffectIndex.Calm:
                    return MagicWord.Nifl;
                case SpellEffectIndex.Mana_Regen_Resist_Song:
                    return MagicWord.Aral;
                case SpellEffectIndex.Stat_Buff:
                    return MagicWord.Scor;
                case SpellEffectIndex.Haste_Runspeed:
                    return MagicWord.Aer;
                default:
                    return MagicWord.Nul;
            }
        }

        public static MagicWord GetSpellEffectTypeMagicWord(SpellEffectType enumValue)
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
                case SpellEffectType.CurrentMana:
                    return MagicWord.Aty;
                case SpellEffectType.MovementSpeed:
                    return MagicWord.Wena;
                case SpellEffectType.Root:
                    return MagicWord.Kor;
                case SpellEffectType.STR:
                    return MagicWord.Bala;
                case SpellEffectType.DEX:
                    return MagicWord.Avi;
                case SpellEffectType.AGI:
                    return MagicWord.Tal;
                case SpellEffectType.STA:
                    return MagicWord.Saha;
                case SpellEffectType.INT:
                    return MagicWord.Gno;
                case SpellEffectType.WIS:
                    return MagicWord.Esot;
                case SpellEffectType.CHA:
                    return MagicWord.Adan;

                default:
                    return MagicWord.Nul;
            }
        }

        public static MagicWord GetTargetTypeMagicWord(TargetType enumValue)
        {
            switch (enumValue)
            {
                case TargetType.None:
                    return MagicWord.Nul;
                case TargetType.Self:
                    return MagicWord.Swe;
                case TargetType.AECaster:
                    return MagicWord.Sie;
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

        public static MagicWord GetResistTypeMagicWord(ResistType enumValue)
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

        public static MagicWord GetPowerLevelMagicWord(PowerLevel enumValue)
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
