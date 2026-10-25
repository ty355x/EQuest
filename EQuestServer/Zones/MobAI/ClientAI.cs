using System;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.MobAI
{
	public class ClientAI : AIBase
	{
		public override MobAIType Type { get { return MobAIType.Client; } }

		public ClientAI(Mob mob, Zone zone) : base (mob, zone)
		{
		}
	}
}
