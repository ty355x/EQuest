using System;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.MobAI
{
	public abstract class AIBase
	{
		protected Mob _mob;
		protected Zone _zone;

		public abstract MobAIType Type { get; }

		public AIBase(Mob mob, Zone zone)
		{
			_mob = mob;
			_zone = zone;
		}

		public virtual void Update(long time)
		{
		}
	}
}
