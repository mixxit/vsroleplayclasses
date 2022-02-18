using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public static class LinguaMagica
    {
        public static string Time = "El";
        public static string Prismatic = "Poly";
        public static string Balance = "Duo";
        public static string Magic = "Mana";
        public static string Air = "Aer";
        public static string Wind = "Wena";
        public static string Exploration = "";
        public static string Adventure = "";
        public static string Trade = "Rit";
        public static string Sky = "Avi";
        public static string Inspiration = "";
        public static string Leadership = "Denu";
        public static string Music = "Lum";
        public static string Storms = "Baal";
        public static string Ruin = "Rew";
        public static string Sin = "Sin";
        public static string Mischief = "Tri";
        public static string Steam = "";
        public static string Spirit = "Maw";
        public static string Unlife = "Cata";
        public static string Disease = "Xul";
        public static string Decay = "";
        public static string Clouds = "Neph";
        public static string Mystery = "Mys";
        public static string Knowledge = "Gno";
        public static string Divinity = "Deus";
        public static string Heat = "";
        public static string Absolution = "";
        public static string Mirth = "";
        public static string Purity = "Dis";
        public static string Fire = "Fyr";
        public static string Radiance = "Sol";
        public static string Dominion = "Dom";
        public static string Fear = "";
        public static string Power = "Gaal";
        public static string Flames = "Scor";
        public static string Chaos = "";
        public static string Strife = "";
        public static string Hate = "";
        public static string Ash = "Asos";
        public static string War = "Tor";
        public static string Conquest = "";
        public static string Blood = "";
        public static string Magma = "";
        public static string Rage = "";
        public static string Delerium = "";
        public static string Nightmares = "Mora";
        public static string Caldera = "";
        public static string Authority = "Reg";
        public static string Light = "Phar";
        public static string Honesty = "Tru";
        public static string Scoria = "";
        public static string Strength = "";
        public static string Metal = "";
        public static string Fortitude = "";
        public static string Earth = "Dun";
        public static string Dust = "";
        public static string Neglect = "";
        public static string TheForgotten = "";
        public static string Cruelty = "";
        public static string Depths = "";
        public static string Gloom = "";
        public static string Solitude = "";
        public static string Misery = "";
        public static string Rock = "Karr";
        public static string Salvation = "";
        public static string Protection = "Pal";
        public static string Heroism = "";
        public static string Mud = "Kor";
        public static string Nature = "";
        public static string Life = "Cita";
        public static string Health = "Meth";
        public static string Growth = "Zep";
        public static string Clay = "";
        public static string Fate = "";
        public static string Entropy = "Zhar";
        public static string Secrets = "";
        public static string Swamps = "";
        public static string Peril = "";
        public static string Poison = "Xon";
        public static string Pain = "Xet";
        public static string Water = "";
        public static string Oceans = "";
        public static string Awe = "Azul";
        public static string Valor = "Val";
        public static string Reverence = "";
        public static string Ice = "";
        public static string Order = "";
        public static string Love = "";
        public static string Honor = "";
        public static string Tides = "";
        public static string Peace = "";
        public static string Compassion = "Aral";
        public static string Diplomacy = "Adan";
        public static string Fog = "";
        public static string Mist = "";
        public static string Liberty = "";
        public static string Dreams = "Zal";
        public static string Tranquility = "";
        public static string Vapor = "";
        public static string Obscuration = "Kela";
        public static string Deceit = "Map";
        public static string Shadow = "Set";
        public static string Rust = "";
        public static string Transformation = "Maya";
        public static string Translocation = "Xen";
        public static string Innovation = "Gen";

        internal static string ToCommaSeperatedString()
        {
            return string.Join(", ", ToStringList().ToArray());
        }

        internal static List<String> ToStringList()
        {
            var result = new List<String>();
            foreach (var info in typeof(LinguaMagica).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                result.Add(info.Name + ": " + info.GetValue(null).ToString());

            return result;
        }
    }
}
