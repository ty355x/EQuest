using System;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestLib.Movement;
using EQuestRender;
using EQuestClient.Network;

namespace EQuestClient.Game
{
	public class Player
	{
		public const float Width = 2.25f;
		public const float HeightCrouching = 2.5f;
		public const float HeightStanding = 6.0f;
		public const float StepSize = 2.75f;

		public bool IsFrozen { get; private set; }
		public bool IsZoning { get; set; }
		public BoundingBox BoundingBox;

		public int MobId = -1;
		public Mob Mob;
		public bool IsRunning = false;
		public float Pitch;
		public float Heading;
		public Matrix Rotation;
		public int RotateDirection = 0;
		public Mob Target;
		public bool IsAttacking = false;
		public float CameraHeight = 5.95f;

		public MobBody Body { get; private set; }
		public PlayerCamera Camera { get; private set; }
		public PlayerCursor Cursor { get; private set; }
		public PlayerInventory Inventory { get; private set; }
		public PlayerLooting Looting { get; private set; }
		public PlayerMovement Movement { get; private set; }
		public PlayerMovementActions MovementActions { get; private set; }
		public PlayerTargetting Targetting { get; private set; }
		
		private GameScene _scene;

		public Player(GameScene scene)
		{
			_scene = scene;

			Body = new MobBody(Vector3.Zero, Vector3.Zero, Width, HeightStanding); 
			Camera = new PlayerCamera(this);
			Cursor = new PlayerCursor(_scene);
			Inventory = new PlayerInventory(_scene);
			Looting = new PlayerLooting(_scene);
			Movement = new PlayerMovement(this, _scene);
			MovementActions = new PlayerMovementActions(this);
			Targetting = new PlayerTargetting(this, _scene);
			
			Movement.StopMovement();
			Movement.UpdatePosition(Vector3.Zero, null);
			IsFrozen = false;
		}

		public void CancelAction()
		{
			if (Looting.IsLooting)
				Looting.EndLooting();
			else if (_scene.UI.IsInventoryVisible)
				_scene.UI.IsInventoryVisible = false;
			else if (Target != null)
				Target = null;
		}

		public void Update(GameTime time, Zone zone, bool noClip)
		{
			MovementActions.Update(time);
			Movement.Update(time, zone, noClip);
			Camera.Update(time);
		}

		public void Freeze()
		{
			Movement.StopMovement(false);
			IsFrozen = true;
		}

		public void Unfreeze()
		{
			IsFrozen = false;
		}

		public void ToggleAttack()
		{
			PacketOut packet = null;

			if (!IsAttacking && Target != null)
				packet = PacketMaker.MakeCombatUpdate(Target, CombatUpdate.StartAttack);
			else if (IsAttacking)
				packet = PacketMaker.MakeCombatUpdate(null, CombatUpdate.EndAttack);

			if (packet != null)
				_scene.Network.SendPacket(packet);
		}

		public void UpdatePositionFromServer(Vector3 position, float heading)
		{
			Movement.StopMovement();
			Movement.UpdatePosition(position, null);
			Heading = heading;
			IsAttacking = false;
			Target = null;
			MovementActions.StartFalling(null);
		}

		public void StopAttack()
		{
			_scene.Network.SendPacket(PacketMaker.MakeCombatUpdate(null, CombatUpdate.EndAttack));
		}

		public void SendZoneRequestForRegion(BspRegion region)
		{
			Freeze();
			IsZoning = true;

			if (region.RequestZoneLineId > 0)
				_scene.Network.SendPacket(PacketMaker.MakeZoneLineRequest(region.RequestZoneLineId, Body.Position));
			else if (region.RequestZoneId > 0)
				_scene.Network.SendPacket(PacketMaker.MakeZoneRequest(region.RequestZoneId, Body.Position));
		}
	}
}
