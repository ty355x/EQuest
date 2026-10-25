using System;
using EQuestLib;
using EQuestRender;
using EQuestClient.UI.Controls;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace EQuestClient.Game
{
	public class CombatHandler
	{
		private GameApp _game;
		private GameScene _scene;

		public CombatHandler(GameApp game, GameScene scene)
		{
			_game = game;
			_scene = scene;
		}

		public void HandleCombatUpdatePacket(PacketIn packet)
		{
			ServerCombatUpdate update = (ServerCombatUpdate)packet.ReadInt();

			if (update == ServerCombatUpdate.StartAttack)
				_scene.Player.IsAttacking = true;
			else if (update == ServerCombatUpdate.EndAttack)
				_scene.Player.IsAttacking = false;
			else if (update == ServerCombatUpdate.Hit)
				HandleHit(packet);
			else if (update == ServerCombatUpdate.Missed)
				HandleMiss(packet);
			else if (update == ServerCombatUpdate.Slain)
				HandleSlain(packet);
			else
				Log.Debug("Got unknown combat update packet " + update);
		}

		private void HandleHit(PacketIn packet)
		{
			int attackerId = packet.ReadInt();
			int targetId = packet.ReadInt();
			int damage = packet.ReadInt();

			Mob attacker = _scene.Zone.FindMob(attackerId);
			Mob target = _scene.Zone.FindMob(targetId);

			if ((attacker == null && attackerId != _scene.Player.MobId) || target == null)
				return;

			string attackerName = _scene.Zone.DetermineMobConsoleName(attackerId);
			string targetName = _scene.Zone.DetermineMobConsoleName(targetId);
			string text = attackerName + " hit " + targetName + " for " + damage + " points of damage.";
			_scene.UI.Write(text, ConsoleColors.Red);

			attacker.Animation.ShowCombat(5, 0);
			target.Animation.ShowDamage(0);
		}

		private void HandleMiss(PacketIn packet)
		{
			int attackerId = packet.ReadInt();
			int targetId = packet.ReadInt();

			Mob attacker = _scene.Zone.FindMob(attackerId);
			Mob target = _scene.Zone.FindMob(targetId);

			if ((attacker == null && attackerId != _scene.Player.MobId) || target == null)
				return;

			string attackerName = attackerId == _scene.Player.MobId ? "YOU" : attacker.Name;
			string targetName = targetId == _scene.Player.MobId ? "YOU" : target.Name;
			_scene.UI.Write(attackerName + " missed " + targetName + ".", ConsoleColors.Red);

			attacker.Animation.ShowCombat(5, 0);
		}

		private void HandleSlain(PacketIn packet)
		{
			// Read all the corpse mob data.
			int corpseId = packet.ReadInt();
			string corpseName = packet.ReadString();
			int corpseRace = packet.ReadInt();
			int corpseGender = packet.ReadInt();
			int corpseTexture = packet.ReadInt();
			float corpseSize = packet.ReadFloat();
			Vector3 corpsePosition = packet.ReadVector3();
			float corpseHeading = packet.ReadFloat();

			// Create the corpse and add to the zone.
			var corpseModelInfo = GameData.Races.FindModelInfo(corpseRace, corpseGender);
			MobModel corpseModel = _scene.Assets.FindMobModel(corpseModelInfo.Name);
			
			var mob = new Mob(corpseId, corpseName, MobType.Corpse, corpseTexture, corpseSize, corpseModel.RaceSize, corpseModel, 
				corpsePosition, 0, 0, 0, corpseHeading, _scene);
			
			mob.Race = corpseRace;
			mob.Gender = corpseGender;
			mob.Animation.ShowCorpse();
			
			_scene.Zone.AddMob(mob);

			// Get the slain mob and the attacker mob data.
			int slainId = packet.ReadInt();
			string slainName = packet.ReadString();
			int attackerId = packet.ReadInt();
			string attackerName = packet.ReadString();

			// Player has died.
			if (slainId == _scene.Player.MobId)
			{
				if (attackerName != null)
					_scene.UI.Write("You were slain by " + attackerName + "!");
				else
					_scene.UI.Write("You died.");

				return;
			}

			// Player has killed a mob.
			if (attackerId == _scene.Player.MobId)
			{
				_scene.UI.Write("You have slain " + slainName + "!");
				return;
			}

			// General slain message.
			if (attackerName != null)
				_scene.UI.Write(slainName + " was slain by " + attackerName + "!");
			else
				_scene.UI.Write(slainName + " died.");
		}      
	}
}
