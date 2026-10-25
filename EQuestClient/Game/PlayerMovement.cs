using System;
using Microsoft.Xna.Framework;
using EQuestRender;
using EQuestLib.Movement;

namespace EQuestClient.Game
{
	public class PlayerMovement
	{
		private const float WalkSpeed = 35.0f;
		private const float RunSpeed = 150.0f;
		
		public Vector3 Move = Vector3.Zero;
		public float MoveSpeed { get { return _player.IsRunning ? RunSpeed : WalkSpeed; } }

		private Player _player;
		private GameScene _scene;
		private MobMovement _mobMovement;
		
		public PlayerMovement(Player player, GameScene scene)
		{
			_player = player;
			_scene = scene;
			_mobMovement = new MobMovement(Player.Width, Player.HeightStanding, Player.StepSize);
		}

		public void Update(GameTime time, Zone zone, bool noClip)
		{
			// Get the time elapsed since the last frame.
			float elapsedTime = (float)time.ElapsedGameTime.TotalSeconds;

			if (elapsedTime == 0)
				return;

			// Create the rotation matrix.
			_player.Rotation =
				Matrix.CreateRotationX(MathHelper.ToRadians(-_player.Pitch)) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(_player.Heading));

			// Calculate the player's velocity.
			_player.Body.Velocity = Move * MoveSpeed;
			_player.Body.Velocity = Vector3.Transform(_player.Body.Velocity, _player.Rotation);

			// Apply fall velocity/gravity.
			if (!noClip)
			{
				_player.Body.Velocity.Z = 
					_player.MovementActions.FallVelocityCurrent != 0 ?
					_player.MovementActions.FallVelocityCurrent :
					PlayerMovementActions.GravityVelocityInitial;
			}
			
			// Make sure we have some velocity.
			if (_player.Body.Velocity == Vector3.Zero)
				return;

			// If no clip is enabled, move the player without collision testing.
			if (noClip)
			{
				Vector3 distance = _player.Body.Velocity * elapsedTime;
				UpdatePosition(_player.Body.Position += distance, time);
				return;
			}

			// Othwerwise, move with collision testing.
			Vector3? dest = _mobMovement.MoveWithCollisionTesting(_player.Body, zone.Octree, elapsedTime);

			// If the player is falling but has not moved in Z, stop falling.
			if (_player.MovementActions.IsFalling && (dest == null || dest.Value.Z == _player.Body.Position.Z))
				_player.MovementActions.StopFalling();

			// If the player is not falling but has moved in Z, start falling.
			if (!_player.MovementActions.IsFalling && dest != null && dest.Value.Z != _player.Body.Position.Z)
				_player.MovementActions.StartFalling(time);

			// If the player has moved, update the player's position.
			if (dest != null)
				UpdatePosition(dest.Value, time);
		}
				
		public void UpdatePosition(Vector3 newPos, GameTime time)
		{
			// Update the player's position.
			_player.Body.Position = newPos;

			// Update the player's bounding box.
			_player.BoundingBox = new BoundingBox(
				new Vector3(newPos.X - 0.5f, newPos.Y - 0.5f, newPos.Z + 0.1f),
				new Vector3(newPos.X + 0.5f, newPos.Y + 0.5f, newPos.Z + 6f));

			// Check if we have intersected with any BSP tree nodes.
			if (_scene.Zone != null)
				CheckIntersectWithBspTreeNode();
		}

		public void StopMovement(bool resetPitchAndHeading = true)
		{
			_player.IsRunning = false;
			_player.Movement.Move = Vector3.Zero;
			_player.Body.Velocity = Vector3.Zero;
			
			if (resetPitchAndHeading)
			{
				_player.Pitch = 0;
				_player.Heading = 0;
				_player.Rotation = Matrix.Identity;
				_player.RotateDirection = 0;
			}
		}

		public void CheckIntersectWithBspTreeNode()
		{
			_scene.Zone.BspTree.FindIntersectionNodes(_player.BoundingBox);

			for (int i = 0; i < _scene.Zone.BspTree.IntersectionCount; i++)
			{
				BspNode node = _scene.Zone.BspTree.Intersections[i];
				
				if (node.Region != null && (node.Region.RequestZoneLineId > 0 || node.Region.RequestZoneId > 0))
				{
					_player.SendZoneRequestForRegion(node.Region);
				}
			}
		}
	}
}
