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
        public static MagicaPower Init = MagicaPower.InitiatePower; // Power Level 0
        public static MagicaPower Adus = MagicaPower.AdeptPower; // Power Level 1
        public static MagicaPower Mas = MagicaPower.MasterPower; // Power Level 2
        public static MagicaPower Myth = MagicaPower.MythicalPower; // Power Level 3
        public static MagicaPower Demi = MagicaPower.AncientPower; // Power Level 4
        public static MagicaPower Eth = MagicaPower.EtherealPower; // Power Level 5
        public static MagicaPower Cela = MagicaPower.CelestialPower; // Power Level 6
        public static MagicaPower Deva = MagicaPower.DivinePower; // Power Level 7
        public static MagicaPower Avul = MagicaPower.AscendentElementalPower;  // Power Level 8
        public static MagicaPower Thul = MagicaPower.DivineElementalPower;  // Power Level 9
        public static MagicaPower Vul = MagicaPower.PrimeElementalPower;  // Power Level 10
        public static MagicaPower Zul = MagicaPower.JudicialPower;  // Power Level 11
        public static MagicaPower Ra = MagicaPower.FoundationalPower;  // Power Level 12
        public static MagicaPower Prima = MagicaPower.PrimordialPower;  // Power Level 13
        public static MagicaPower Omni = MagicaPower.UltimatePower;  // Power Level 15

        public static MagicaPower Nul = MagicaPower.None;
        public static MagicaPower El = MagicaPower.Time;
        public static MagicaPower Poly = MagicaPower.Prismatic;
        public static MagicaPower Duo = MagicaPower.Balance;
        public static MagicaPower Mana = MagicaPower.Magic;
        public static MagicaPower Aer = MagicaPower.Air;
        public static MagicaPower Wena = MagicaPower.Wind;
        public static MagicaPower Cara = MagicaPower.Exploration;
        public static MagicaPower Avon = MagicaPower.Adventure;
        public static MagicaPower Rit = MagicaPower.Trade;
        public static MagicaPower Avi = MagicaPower.Sky;
        public static MagicaPower Aty = MagicaPower.Inspiration;
        public static MagicaPower Denu = MagicaPower.Leadership;
        public static MagicaPower Lum = MagicaPower.Music;
        public static MagicaPower Baal = MagicaPower.Storms;
        public static MagicaPower Rew = MagicaPower.Ruin;
        public static MagicaPower Sin = MagicaPower.Sin;
        public static MagicaPower Tri = MagicaPower.Mischief;
        public static MagicaPower Nifo = MagicaPower.Steam;
        public static MagicaPower Maw = MagicaPower.Spirit;
        public static MagicaPower Cata = MagicaPower.Unlife;
        public static MagicaPower Xul = MagicaPower.Disease;
        public static MagicaPower Jar = MagicaPower.Decay;
        public static MagicaPower Neph = MagicaPower.Clouds;
        public static MagicaPower Mys = MagicaPower.Mystery;
        public static MagicaPower Gno = MagicaPower.Knowledge;
        public static MagicaPower Deus = MagicaPower.Divinity;
        public static MagicaPower Zar = MagicaPower.Heat;
        public static MagicaPower Mar = MagicaPower.Absolution;
        public static MagicaPower Vasu = MagicaPower.Mirth;
        public static MagicaPower Dis = MagicaPower.Purity;
        public static MagicaPower Fyr = MagicaPower.Fire;
        public static MagicaPower Sol = MagicaPower.Radiance;
        public static MagicaPower Dom = MagicaPower.Dominion;
        public static MagicaPower Phob = MagicaPower.Fear;
        public static MagicaPower Gaal = MagicaPower.Power;
        public static MagicaPower Flam = MagicaPower.Flames;
        public static MagicaPower Avok = MagicaPower.Chaos;
        public static MagicaPower Eris = MagicaPower.Strife;
        public static MagicaPower Kada = MagicaPower.Hate;
        public static MagicaPower Asos = MagicaPower.Ash;
        public static MagicaPower Tor = MagicaPower.War;
        public static MagicaPower Hek = MagicaPower.Conquest;
        public static MagicaPower Sang = MagicaPower.Blood;
        public static MagicaPower Mol = MagicaPower.Magma;
        public static MagicaPower Rabi = MagicaPower.Rage;
        public static MagicaPower Delo = MagicaPower.Delerium;
        public static MagicaPower Mora = MagicaPower.Nightmares;
        public static MagicaPower Cal = MagicaPower.Caldera;
        public static MagicaPower Reg = MagicaPower.Authority;
        public static MagicaPower Phar = MagicaPower.Light;
        public static MagicaPower Tru = MagicaPower.Honesty;
        public static MagicaPower Scor = MagicaPower.Scoria;
        public static MagicaPower Bala = MagicaPower.Strength;
        public static MagicaPower Ayas = MagicaPower.Metal;
        public static MagicaPower Saha = MagicaPower.Fortitude;
        public static MagicaPower Dun = MagicaPower.Earth;
        public static MagicaPower Asa = MagicaPower.Dust;
        public static MagicaPower Nec = MagicaPower.Neglect;
        public static MagicaPower Veda = MagicaPower.TheForgotten;
        public static MagicaPower Sadi = MagicaPower.Cruelty;
        public static MagicaPower Apa = MagicaPower.Depths;
        public static MagicaPower Glom = MagicaPower.Gloom;
        public static MagicaPower Sing = MagicaPower.Solitude;
        public static MagicaPower Dol = MagicaPower.Misery;
        public static MagicaPower Karr = MagicaPower.Rock;
        public static MagicaPower Scu = MagicaPower.Salvation;
        public static MagicaPower Pal = MagicaPower.Protection;
        public static MagicaPower Sig = MagicaPower.Heroism;
        public static MagicaPower Kor = MagicaPower.Mud;
        public static MagicaPower Nato = MagicaPower.Nature;
        public static MagicaPower Cita = MagicaPower.Life;
        public static MagicaPower Meth = MagicaPower.Health;
        public static MagicaPower Zep = MagicaPower.Growth;
        public static MagicaPower Klaj = MagicaPower.Clay;
        public static MagicaPower Wei = MagicaPower.Fate;
        public static MagicaPower Zhar = MagicaPower.Entropy;
        public static MagicaPower Esot = MagicaPower.Secrets;
        public static MagicaPower Myr = MagicaPower.Swamps;
        public static MagicaPower Cul = MagicaPower.Peril;
        public static MagicaPower Xon = MagicaPower.Poison;
        public static MagicaPower Xet = MagicaPower.Pain;
        public static MagicaPower Wato = MagicaPower.Water;
        public static MagicaPower Sie = MagicaPower.Oceans;
        public static MagicaPower Azul = MagicaPower.Awe;
        public static MagicaPower Val = MagicaPower.Valor;
        public static MagicaPower Gera = MagicaPower.Reverence;
        public static MagicaPower Isti = MagicaPower.Ice;
        public static MagicaPower Ordo = MagicaPower.Order;
        public static MagicaPower Amor = MagicaPower.Love;
        public static MagicaPower Ono = MagicaPower.Honor;
        public static MagicaPower Tal = MagicaPower.Tides;
        public static MagicaPower Mir = MagicaPower.Peace;
        public static MagicaPower Aral = MagicaPower.Compassion;
        public static MagicaPower Adan = MagicaPower.Diplomacy;
        public static MagicaPower Dhum = MagicaPower.Fog;
        public static MagicaPower Nifl = MagicaPower.Mist;
        public static MagicaPower Leo = MagicaPower.Liberty;
        public static MagicaPower Zal = MagicaPower.Dreams;
        public static MagicaPower Sek = MagicaPower.Tranquility;
        public static MagicaPower Para = MagicaPower.Vapor;
        public static MagicaPower Kela = MagicaPower.Obscuration;
        public static MagicaPower Map = MagicaPower.Deceit;
        public static MagicaPower Set = MagicaPower.Shadow;
        public static MagicaPower Ros = MagicaPower.Rust;
        public static MagicaPower Ex = MagicaPower.Transformation;
        public static MagicaPower Xen = MagicaPower.Translocation;
        public static MagicaPower Gen = MagicaPower.Innovation;

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
