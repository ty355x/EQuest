using System;
using Microsoft.Xna.Framework;
using EQuestRender;

namespace EQuestLib.Collisions
{
	public class MeshCollision
	{
		private Polygon[] _collisionPolygons;
		private BoundingBox _bounds;
		private BoundingSphere _boundingSphere;
		private Vector3 _center;
		private MeshCollisionMethod _collisionMethod;

		public Polygon[] CollisionPolygons { get { return _collisionPolygons; } }
		public BoundingBox Bounds { get { return _bounds; } }
		public BoundingSphere BoundingSphere { get { return _boundingSphere; } }
		public Vector3 Center { get { return _center; } }
		public MeshCollisionMethod CollisionMethod { get { return _collisionMethod; } }

		public MeshCollision(Polygon[] collisionPolygons, BoundingBox bounds, Vector3 center, float maxDist)
		{
			_collisionPolygons = collisionPolygons;
			_bounds = bounds;
			_center = center;

			_bounds = bounds;
			_boundingSphere = new BoundingSphere(center, maxDist);

			if (bounds.Min == Vector3.Zero && bounds.Max == Vector3.Zero)
				_collisionMethod = MeshCollisionMethod.Sphere;
			else
				_collisionMethod = MeshCollisionMethod.BoundingBox;
		}
	}
}
