using System;
using EQuestRender;

namespace EQuestLib.Octrees
{
	public struct PolyCollisionResult
	{
		public bool HasCollided { get; private set; }
		public CollisionPoly Polygon { get; private set; }
		public CollisionObjectType ObjectType { get; private set; }

		private PolyCollisionResult(bool hasCollided, CollisionPoly polygon, CollisionObjectType objectType)
		{
			this.HasCollided = hasCollided;
			this.Polygon = polygon;
			this.ObjectType = objectType;
		}

		public static PolyCollisionResult MakeCollisionResult(CollisionPoly polygon, CollisionObjectType objectType)
		{
			return new PolyCollisionResult(true, polygon, objectType);
		}

		public static PolyCollisionResult MakeNoCollisionResult()
		{
			return new PolyCollisionResult(false, null, CollisionObjectType.None);
		}
	}
}
