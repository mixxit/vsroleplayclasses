using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public static class AdventureClassTools
    {
        public static AdventureClass GetAdventureClassByString(string adventureClassNameCaseInsensitive)
        {
            AdventureClass type = AdventureClass.None;

            try
            {
                type = (AdventureClass)Enum.Parse(typeof(AdventureClass), adventureClassNameCaseInsensitive, true);
            }
            catch (Exception)
            {
                type = AdventureClass.None;
            }

            return type;
        }
    }
}
