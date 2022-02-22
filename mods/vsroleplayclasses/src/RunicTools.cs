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
        public static MagicaPower? GetWordOfPowerFromLinguaMagicaString(string lingaMagicaString)
        {
            if (String.IsNullOrEmpty(lingaMagicaString))
                return null;

            return LinguaMagica.LinguaMagicaStringToMagicaPowerCaseInsensitive(lingaMagicaString);
        }

        internal static MagicaPower? GetWordOfPowerFromWordOfPowerString(string wordOfPower)
        {
            if (String.IsNullOrEmpty(wordOfPower))
                return null;

            wordOfPower = wordOfPower.ToLower();

            foreach (MagicaPower magicaPower in Enum.GetValues(typeof(MagicaPower)))
            {
                if (magicaPower.ToString().ToLower().Equals(wordOfPower))
                    return magicaPower;
            }

            return null;
        }

        internal static MagicaPower? GetWordOfPowerFromQuillItem(RunicInkwellAndQuillItem item)
        {
            var magicaString = item.Code.ToString().Replace("vsroleplayclasses", "").Replace(":", "").Replace(item.Class, "").Replace("-","");
            if (String.IsNullOrEmpty(magicaString))
                return null;

            return GetWordOfPowerFromLinguaMagicaString(magicaString);
        }

        
    }
}
