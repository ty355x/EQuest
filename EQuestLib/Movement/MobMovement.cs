using System;
using Microsoft.Xna.Framework;
using EQuestLib.Octrees;
using EQuestRender;

namespace EQuestLib.Movement
{
	public class MobMovement
	{
		private const float MaxMoveWithoutLerp = 1.5f;

		private float _entityWidth;
		private float _entityHeight;
		private float _entityStepSize;
		private float _entityHalfWidth;

		public MobMovement(float entityWidth, float entityHeight, float entityStepSize)
		{
			_entityWidth = entityWidth;
			_entityHeight = entityHeight;
			_entityStepSize = entityStepSize;
			_entityHalfWidth = _entityWidth / 2;
		}

		public Vector3? MoveWithCollisionTesting(MobBody mob, Octree octree, float elapsedTime)
		{
			Vector3 moveDelta = mob.Velocity * elapsedTime;
			float distanceMoved = moveDelta.Length();

			if (Math.Abs(distanceMoved) <= MaxMoveWithoutLerp)
			{
				Vector3? newPosition = TryMove(mob.Position, moveDelta, octree);

				if (newPosition != null)
					return newPosition.Value;
				else
					return null;
			}
			else
			{
				return AttemptMoveWithLerp(mob.Position, moveDelta, distanceMoved, octree);
			}
		}

		private Vector3? AttemptMoveWithLerp(Vector3 mobPosition, Vector3 moveDelta, float distanceMoved, Octree octree)
		{
			float lerpAmount = 1 / (distanceMoved / MaxMoveWithoutLerp);
			Vector3? newPosition = null;

			for (float i = lerpAmount; i < 1; i += lerpAmount)
			{
				newPosition = TryMove(mobPosition, moveDelta, octree);

				if (newPosition == null)
					break;
			}

			if (newPosition != null)
				return newPosition.Value;
			else
				return null;
		}

		private Vector3? TryMove(Vector3 mobPosition, Vector3 moveDelta, Octree octree)
		{
			// If we are only moving in Z, just try to move in Z.
			if (moveDelta.X == 0 && moveDelta.Y == 0 && moveDelta.Z != 0)
			{
				MobMoveResult result = FindMoveDestination(mobPosition, moveDelta, octree, false);
				
				if (result.HasMoved)
					return result.Position;
				else
					return null;
			}
			
			// Try moving in xy.
			Vector3 moveDeltaXY = new Vector3(moveDelta.X, moveDelta.Y, 0);
			MobMoveResult moveXY = FindMoveDestination(mobPosition, moveDeltaXY, octree, false);

			// If this worked, try moving in Z and return the result.
			if (moveXY.HasMoved)
				return TryMoveInZFromCurrentPos(moveXY.Position, moveDelta, octree);

			// Attempt to step above obstacle.
			MobMoveResult moveStepUp = MobMoveResult.MakeHasNotMoved(CollisionObjectType.None);
			moveStepUp = FindMoveDestination(mobPosition, new Vector3(moveDelta.X, moveDelta.Y, _entityStepSize), octree, false);
			
			// If we stepped above an obstacle, move the mob down to the floor and return.
			if (moveStepUp.HasMoved)
			{
				const float stepDownAmount = 0.5f;
				Vector3 stepDownFinalPos = moveStepUp.Position;

				for (float i = 0; i <= _entityStepSize; i += stepDownAmount)
				{
					MobMoveResult moveStepDown = FindMoveDestination(stepDownFinalPos, new Vector3(0, 0, -stepDownAmount), octree, false);

					if (!moveStepDown.HasMoved)
						break;

					stepDownFinalPos = moveStepDown.Position;
				}

				return stepDownFinalPos;
			}

			// If we couldn't move, try sliding along the polygon.
			MobMoveResult moveSlide = FindMoveDestination(mobPosition, moveDeltaXY, octree, true);

			// If this worked, try moving in Z and return the result.
			if (moveSlide.HasMoved)
				return TryMoveInZFromCurrentPos(moveSlide.Position, moveDelta, octree);
			
			// Otherwise, move the player in Z and return.
			MobMoveResult moveZ = FindMoveDestination(mobPosition, new Vector3(0, 0, moveDelta.Z), octree, false);

			if (moveZ.HasMoved)
				return moveZ.Position;
			else
				return null;
		}
		
		private Vector3 TryMoveInZFromCurrentPos(Vector3 mobPosition, Vector3 moveDelta, Octree octree)
		{
			MobMoveResult moveZ = FindMoveDestination(mobPosition, new Vector3(0, 0, moveDelta.Z), octree, false);

			if (moveZ.HasMoved)
				return moveZ.Position;
			else
				return mobPosition;
		}

		private MobMoveResult FindMoveDestination(Vector3 mobPos, Vector3 moveDelta, Octree octree, bool trySlide)
		{
			// Calculate the destination and check if we collide with any polygons.
			Vector3 mobDest = mobPos + moveDelta;
			PolyCollisionResult collision = FindPolyMobCollidesWith(mobDest, octree, false);

			// If we don't collide, return the destination.
			if (!collision.HasCollided)
				return MobMoveResult.MakeMoved(mobDest);

			// Attempt to slide off the polygon if TrySlide is set and movement has an X or Y component.
			if (!trySlide || (moveDelta.X == 0 && moveDelta.Y == 0))
				return MobMoveResult.MakeHasNotMoved(collision.ObjectType);

			// Normalize the movement delta vector.
			moveDelta.Normalize();
			
			// We need to reverse the polygon normal vector.
			Vector3 polyNormal = collision.Polygon.Normal * -1 ;
			
			// Calculate the slide delta vector.
			Vector3 slideDelta = moveDelta + polyNormal;
			slideDelta.Normalize();

			// Calculate the new destination.
			slideDelta = slideDelta * moveDelta.Length() * 0.1f;
			Vector3 newDest = mobPos + slideDelta;

			// Determine if we can move to the new location.
			collision = FindPolyMobCollidesWith(newDest, octree, false);
			
			if (!collision.HasCollided)
				return MobMoveResult.MakeMoved(newDest);
			else
				return MobMoveResult.MakeHasNotMoved(collision.ObjectType);
		}

		private PolyCollisionResult FindPolyMobCollidesWith(Vector3 mobPos, Octree octree, bool makeSpaceForFloor)
		{
			// float floorGap = makeSpaceForFloor ? 0.1f : 0;
			float floorGap = 0;

			var mobBounds = new BoundingBox(
				new Vector3(mobPos.X - _entityHalfWidth, mobPos.Y - _entityHalfWidth, mobPos.Z + floorGap),
				new Vector3(mobPos.X + _entityHalfWidth, mobPos.Y + _entityHalfWidth, mobPos.Z + _entityHeight));

			return octree.Collision.FindPolyBoxCollidesWith(mobBounds);
		}
	}
}
