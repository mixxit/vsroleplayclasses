using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src
{
    public static class EntityUtils
    {
		public static double GetExperienceRewardAverageForLevel(int killedLevel, int killersLevel)
		{
			double experience = (Math.Pow(killedLevel, 2) * 10) * WorldLimits.MAX_LEVEL - 1;
			experience = experience / 2;
			if (experience < 1)
				experience = 1d;
			if (killersLevel < 10)
				return experience * 6d;
			if (killersLevel < 20)
				return experience * 5d;
			if (killersLevel < 30)
				return experience * 5d;
			if (killersLevel < 40)
				return experience * 4d;
			if (killersLevel < 50)
				return experience * 3d;
			if (killersLevel < 60)
				return experience * 2d;
			return experience;
		}


		public static float GetStatMaxHP(string className, int level, int stamina)
		{
			// level multiplier
			double multiplier = 1;

			String profession = "UNSKILLED";
			if (!String.IsNullOrEmpty(className))
				profession = className.ToUpper();

			if (profession != null)
			{
				switch (profession)
				{
					case "WARRIOR":
						if (level < 20)
							multiplier = 22;
						else if (level < 30)
							multiplier = 23;
						else if (level < 40)
							multiplier = 25;
						else if (level < 53)
							multiplier = 27;
						else if (level < 57)
							multiplier = 28;
						else
							multiplier = 30;
						break;

					case "DRUID":
					case "CLERIC":
					case "SHAMAN":
						multiplier = 15;
						break;

					case "PALADIN":
					case "SHADOWKNIGHT":
						if (level < 35)
							multiplier = 21;
						else if (level < 45)
							multiplier = 22;
						else if (level < 51)
							multiplier = 23;
						else if (level < 56)
							multiplier = 24;
						else if (level < 60)
							multiplier = 25;
						else
							multiplier = 26;
						break;

					case "MONK":
					case "BARD":
					case "ROGUE":
						// case BEASTLORD:
						if (level < 51)
							multiplier = 18;
						else if (level < 58)
							multiplier = 19;
						else
							multiplier = 20;
						break;

					case "RANGER":
						if (level < 58)
							multiplier = 20;
						else
							multiplier = 21;
						break;

					case "MAGICIAN":
					case "WIZARD":
					case "NECROMANCER":
					case "ENCHANTER":
						multiplier = 12;
						break;
					default:
						if (level < 35)
							multiplier = 21;
						else if (level < 45)
							multiplier = 22;
						else if (level < 51)
							multiplier = 23;
						else if (level < 56)
							multiplier = 24;
						else if (level < 60)
							multiplier = 25;
						else
							multiplier = 26;
						break;
				}
			}

			double hp = level * multiplier;
			double hpmain = (stamina / 12) * level;

			double calculatedhp = hp + hpmain;
			if (calculatedhp > float.MaxValue)
				calculatedhp = float.MaxValue;

			int preTierFilteringHp = (int)Math.Floor(calculatedhp);
			return (float)preTierFilteringHp;
		}

	}
}
