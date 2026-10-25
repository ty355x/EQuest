using System;

namespace EQuestLib
{
	public enum ClientCommand
	{
		Login = 0,
		EnteredZone = 1,
		UpdateMovement = 2,
		Message = 3,
		CombatUpdate = 4,
		ZoneLineRequest = 5,
		ZoneRequest = 6,
		StartLootCorpse = 7,
		LootItems = 8,
		PickupInventoryItem = 9,
		DestroyItem = 10,
	}
}
