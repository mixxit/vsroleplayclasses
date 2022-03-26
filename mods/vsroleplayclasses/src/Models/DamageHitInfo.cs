using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsroleplayclasses.src.Models
{
    public class DamageHitInfo
    {
		public SkillType skill = SkillType.HandtoHand;
		public int damage_done = 0;
		public int min_damage = 0;
		public int base_damage = 0;
		public int offense = 0;
		public int tohit = 0;
		public bool avoided = false;
		public bool dodged = false;
		public bool riposted = false;
		//public int hand = 0; // 0 = primary, 1 = secondary
	}
}
