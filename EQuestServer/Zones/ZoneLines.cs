using System;
using System.Collections.Generic;
using EQuestServer.Repositories;

namespace EQuestServer.Zones
{
	public class ZoneLines
	{
		private List<DbZoneLine> _zoneLines;
		
		public void LoadZoneLines(string zone)
		{
			using (var zoneLineRepository = new ZoneLineRepository())
				_zoneLines = zoneLineRepository.FindByZoneName(zone);
		}

		public DbZoneLine FindZoneLineForZone(int zoneId)
		{
			foreach (var zoneLine in _zoneLines)
			{
				if (zoneLine.target_zone_id == zoneId)
					return zoneLine;
			}

			return null;
		}

		public DbZoneLine FindZoneLineForZoneLineId(int zoneLineId)
		{
			foreach (var zoneLine in _zoneLines)
			{
				if (zoneLine.number == zoneLineId)
					return zoneLine;
			}
			
			return null;
		}
	}
}
