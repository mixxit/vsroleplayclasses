using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public static class AdventureClassTools
    {
        public static bool IsWarriorClass(AdventureClass adventureClass)
        {
            switch (adventureClass)
            {
                case AdventureClass.Warrior:
                case AdventureClass.Rogue:
                case AdventureClass.Monk:
                case AdventureClass.Paladin:
                case AdventureClass.Shadowknight:
                case AdventureClass.Ranger:
                case AdventureClass.Beastlord:
                case AdventureClass.Berserker:
                case AdventureClass.Bard:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

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
