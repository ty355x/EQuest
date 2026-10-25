using System;
using Microsoft.Xna.Framework;
using EQuestLib.Octrees;

namespace EQuestLib.Movement
{
	public struct MobMoveResult
	{
		public bool HasMoved { get; private set; }
		public Vector3 Position { get; private set; }
		public CollisionObjectType CollisionObjectType { get; private set; }

		private MobMoveResult(bool hasMoved, Vector3 position, CollisionObjectType collisionObjectType)
		{
			this.HasMoved = hasMoved;
			this.Position = position;
			this.CollisionObjectType = collisionObjectType;
		}

		public static MobMoveResult MakeMoved(Vector3 position)
		{
			return new MobMoveResult(true, position, CollisionObjectType.None);
		}

		public static MobMoveResult MakeHasNotMoved(CollisionObjectType collisionObjectType)
		{
			return new MobMoveResult(false, Vector3.Zero, collisionObjectType);
		}
	}
}
