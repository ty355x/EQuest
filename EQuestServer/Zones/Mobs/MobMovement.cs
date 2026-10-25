using System;
using Microsoft.Xna.Framework;

using EQuestLib;

namespace EQuestServer.Zones.Mobs
{
	public class MobMovement
	{
		public int RotateDirection = 0;
		public float WalkSpeed = 20;
		public float RunSpeed = 35;

		public bool IsMoving = false;
		
		private Mob _mob;
		private Vector3 _destination = Vector3.Zero;
		private Vector3 _startMovePosition = Vector3.Zero;
		private long _startMoveTime = -1;
		private float _distanceToMove = 0;

		public MobMovement(Mob mob)
		{
			_mob = mob;
		}

		/// <summary>
		/// Sets a new current position, evaluating the best Z coordinate on the map.
		/// </summary>
		public void SetPosition(Vector3 pos)
		{
			float z = _mob.Zone.Map.FindBestZ(pos);
			_mob.Position = new Vector3(pos.X, pos.Y, z);
		}

		public void Warp(Vector3 pos)
		{
			_mob.Position = pos;
		}

		public void CheckUpdateMovePosition(long time)
		{
			if (_mob.Position == _destination)
				StopMoveInterpolation();

			Vector3 pos = _mob.Position;

			// Determine how far the mob has moved.
			float secondsMoved = _startMoveTime / 1000;
			float distanceMoved = _mob.MoveSpeed * secondsMoved;

			if ((distanceMoved - 0.25) >= _distanceToMove)
			{
				// We have arrived at our destination.
				pos = _destination;
				StopMoveInterpolation();
			}
			else
			{
				// Interpolate movement.
				Vector3 moveTo = _destination - _startMovePosition;
				moveTo.Normalize();
				pos = _startMovePosition + (distanceMoved * moveTo);
			}

			SetPosition(pos);
			_mob.Zone.Server.SendMobMovementUpdate(_mob);
		}

		public void MoveNearTarget()
		{
			if (_mob.IsDisabled || _mob.Target == null || _mob.Target.Position == _mob.LastTargetPosition)
				return;

			// Make a vector from the mob to the target.
			Vector3 toTarget = _mob.Target.Position - _mob.Position;

			// Determine the distance to the target.
			float distance = toTarget.Length();

			// Determine the position to move to.
			toTarget.Normalize();

			float deltaDistance = 8;
			Vector3 newPosition = _mob.Position + ((distance - deltaDistance) * toTarget);
			float degreesRotate = (float)((ZoneServer.Random.NextDouble() * 2.25) - 1.5);

			Matrix rotate =
				Matrix.CreateRotationZ(MathHelper.ToRadians(degreesRotate));

			newPosition = Vector3.Transform(newPosition, rotate);

			// Determine the new heading.
			Vector3 directionToTarget = _mob.Target.Position - newPosition;
			float newHeading = MathHelper.ToDegrees((float)Math.Atan2(directionToTarget.Y, directionToTarget.X));

			// Move to new position.
			MoveTo(newPosition, newHeading);

			// Store the last position of the target.
			_mob.LastTargetPosition = _mob.Target.Position;
		}

		public void MoveTo(Vector3 newPosition)
		{
			if (newPosition == _destination)
				return;

			float heading = _mob.CalculateHeadingToPosition(newPosition);
			MoveTo(newPosition, heading);
		}

		public void MoveTo(Vector3 newPosition, float newHeading)
		{
			if (newPosition == _destination)
				return;

			// Determine the move distance.
			Vector3 toFinal = _destination - _mob.Position;
			_distanceToMove = toFinal.Length();

			// Determine the shorteset rotate direction to the new heading.
			float rotateDistance = EQuestMath.FindShortestRotateDistance(_mob.Heading, newHeading);
			RotateDirection = rotateDistance > 0 ? 1 : -1;

			// Update final position and heading.
			_destination = newPosition;
			_mob.Heading = newHeading;

			// Determine our movement speed.
			_mob.MoveSpeed = _mob.IsAttacking ? RunSpeed : WalkSpeed;

			// Save the current move position and the current time and start movement interpolation.
			_startMovePosition = _mob.Position;
			_startMoveTime = _mob.Zone.Server.CurrentTime;
			IsMoving = true;
		}

		private void StopMoveInterpolation()
		{
			IsMoving = false;
		}
	}
}
