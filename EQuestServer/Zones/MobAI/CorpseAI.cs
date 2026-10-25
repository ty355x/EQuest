using System;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.MobAI
{
	public class CorpseAI : AIBase
	{
		private const long DecayIntervalMS = 60 * 1000;
		private long _decayTimeMS = -1;

		public override MobAIType Type { get { return MobAIType.Corpse; } }

		public CorpseAI(Mob mob, Zone zone, long time) : base(mob, zone)
		{
			_decayTimeMS = time + DecayIntervalMS;
		}

		public override void Update(long time)
		{
			if (time >= _decayTimeMS)
			{
				_zone.Server.QueueRemoveMob(_mob);
				_decayTimeMS = -1;
			}
		}
	}
}
