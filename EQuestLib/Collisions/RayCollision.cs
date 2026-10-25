using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EQuestRender;

namespace EQuestLib.Collisions
{
	public class RayCollision
	{
		private enum PlaneSides { PlaneFront, PlaneBack, PlaneCoincide };

		public static float? RayCollidesWithMesh(Vector3 rayStart, Vector3 rayEnd, MeshCollision[] meshes)
		{
			Vector3 direction = rayEnd - rayStart;
			var ray = new Ray(rayStart, direction);

			foreach (var mesh in meshes)
			{
				if (mesh.CollisionMethod == MeshCollisionMethod.BoundingBox && ray.Intersects(mesh.Bounds) == null)
					continue;
				else if (ray.Intersects(mesh.BoundingSphere) == null)
					continue;

				foreach (Polygon poly in mesh.CollisionPolygons)
				{
					if (RayIntersectsTriangle(rayStart, rayEnd, poly))
					{
						return ray.Intersects(poly.Plane);
					}
				}
			}

			return null;
		}

		private static bool RayIntersectsTriangle(Vector3 rayStart, Vector3 rayEnd, Polygon poly)
		{
			if (poly.Flags == 16)
				return false;

			PlaneSides p1 = ClassifyPoint(poly, rayStart);
			PlaneSides p2 = ClassifyPoint(poly, rayEnd);

			if (p1 != p2)
			{
				Vector3 ray = Vector3.Subtract(rayEnd, rayStart);
				float denominator = Vector3.Dot(poly.Normal, ray);

				if (denominator == 0)
					return false;

				float t = -(Vector3.Dot(poly.Normal, rayStart) + poly.Plane.D) /
					denominator;

				Vector3 intersect = rayStart + (ray * t);

				if (PointInTriangle(intersect, poly.V0, poly.V1, poly.V2))
					return true;
			}

			return false;
		}

		private static PlaneSides ClassifyPoint(Polygon poly, Vector3 pos)
		{
			float p = Vector3.Dot(poly.Normal, pos) + poly.Plane.D;

			if (p > 0.0f)
				return PlaneSides.PlaneFront;
			else if (p < 0.0f)
				return PlaneSides.PlaneBack;
			else
				return PlaneSides.PlaneCoincide;
		}

		private static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
		{
			return SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b);
		}

		private static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
		{
			Vector3 cp1 = Vector3.Cross(b - a, p1 - a);
			Vector3 cp2 = Vector3.Cross(b - a, p2 - a);
			return Vector3.Dot(cp1, cp2) >= 0;
		}

		private static bool EdgeIntersectsWithTriangle(Ray edgeRay, float edgeLength, Vector3 triV0, Vector3 triV1, 
			Vector3 triV2, Plane triPlane)
		{
			float? intersectDist = edgeRay.Intersects(triPlane);

			if (intersectDist == null)
				return false;

			Vector3 intersectPoint = edgeRay.Position + (edgeRay.Direction * edgeLength);
			return PointInTriangle(intersectPoint, triV0, triV1, triV2);
		}
	}
}
