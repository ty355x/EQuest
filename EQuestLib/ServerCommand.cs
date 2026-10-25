using System;

namespace EQuestLib
{
	public enum ServerCommand
	{
		RequestLogin = 0,
		EnterZone = 1,
		UpdateKnownItems = 2,
		Message = 3,
		SpawnMob = 4,
		RemoveMob = 5,
		UpdateMobStatus = 6,
		UpdateMobMovement = 7,
		CombatUpdate = 8,
		LootList = 9,
		PickupLootItem = 10,
	}
}
