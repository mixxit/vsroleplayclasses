using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public class DamageTypeClass
    {
        public AdventureClass AdventureClass { get; set; }
        public ResistType ResistType { get; set; }
        internal static DamageTypeClass Convert(ExtendedEnumDamageType damageType)
        {
            if ((int)damageType < 99 || !damageType.ToString().Contains("_"))
            {
                // fall back on war if cant find
                var expType = AdventureClass.Warrior;
                if (damageType == ExtendedEnumDamageType.PiercingAttack)
                    expType = AdventureClass.Ranger;
                if (damageType == ExtendedEnumDamageType.Poison)
                    expType = AdventureClass.Rogue;

                return new DamageTypeClass()
                {
                    AdventureClass = expType,
                    ResistType = ResistType.None
                };
            }

            try
            {
                var data = damageType.ToString().Split('_');
                var adventureClass = (AdventureClass)Enum.Parse(typeof(AdventureClass), data[0]);
                var resistType = (ResistType)Enum.Parse(typeof(ResistType), data[1]);

                return new DamageTypeClass()
                {
                    AdventureClass = adventureClass,
                    ResistType = resistType
                };
            } catch (Exception)
            {
                return new DamageTypeClass()
                {
                    AdventureClass = AdventureClass.Warrior,
                    ResistType = ResistType.None
                };
            }
        }
    }
}
