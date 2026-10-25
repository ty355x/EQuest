using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EQuestRender;

namespace EQuestLib.Collisions
{
	public static class Collision
	{
		public static bool BoxCollidesWithMeshBounds(BoundingBox box, Mesh mesh)
		{
			if (mesh.CollisionMethod == MeshCollisionMethod.BoundingBox)
				return box.Contains(mesh.Bounds) != ContainmentType.Disjoint;
			else
				return box.Contains(mesh.BoundingSphere) != ContainmentType.Disjoint;
		}

		public static bool BoxCollidesWithMesh(BoundingBox box, Mesh mesh)
		{
			if (!BoxCollidesWithMeshBounds(box, mesh))
				return false;

			if (!BoxCollidesWithMeshPolygon(box, mesh))
				return false;
			
			return true;
		}

		public static bool BoxCollidesWithZoneObject(BoundingBox box, ZoneObject obj)
		{
			if (box.Contains(obj.Bounds) == ContainmentType.Disjoint)
				return false;

			Vector3 boxCenter = FindBoxCenter(box);
			Vector3 boxHalfSize = FindBoxHalfSize(box);

			foreach (var poly in obj.CollisionPolys)
			{
				if (BoxCollidesWithPolygon(box.Min, box.Max, boxCenter, boxHalfSize, poly.V0, poly.V1, poly.V2, poly.Normal))
					return true;    
			}

			return false;
		}

		public static CollisionPoly FindZoneObjectPolyBoxCollidesWith(BoundingBox box, ZoneObject obj)
		{
			if (box.Contains(obj.Bounds) == ContainmentType.Disjoint)
				return null;

			Vector3 boxCenter = FindBoxCenter(box);
			Vector3 boxHalfSize = FindBoxHalfSize(box);

			foreach (var poly in obj.CollisionPolys)
			{
				if (BoxCollidesWithPolygon(box.Min, box.Max, boxCenter, boxHalfSize, poly.V0, poly.V1, poly.V2, poly.Normal))
					return poly;
			}

			return null;
		}

		public static bool BoxCollidesWithMeshPolygon(BoundingBox box, Vector3 boxCenter, Vector3 boxHalfSize, Mesh mesh)
		{
			foreach (CollisionPoly poly in mesh.CollisionPolys)
			{
				if (BoxCollidesWithPolygon(box.Min, box.Max, boxCenter, boxHalfSize, poly.V0, poly.V1, poly.V2, poly.Normal))
					return true;
			}

			return false;
		}

		public static bool BoxCollidesWithMeshPolygon(BoundingBox box, Mesh mesh)
		{
			Vector3 boxCenter = FindBoxCenter(box);
			Vector3 boxHalfSize = FindBoxHalfSize(box);

			return BoxCollidesWithMeshPolygon(box, boxCenter, boxHalfSize, mesh);
		}

		public static CollisionPoly FindPolyBoxCollidesWith(BoundingBox box, Vector3 boxCenter, Vector3 boxHalfSize, Mesh mesh)
		{
			foreach (CollisionPoly poly in mesh.CollisionPolys)
			{
				if (BoxCollidesWithPolygon(box.Min, box.Max, boxCenter, boxHalfSize, poly.V0, poly.V1, poly.V2, poly.Normal))
					return poly;
			}

			return null;
		}

		public static CollisionPoly FindPolyBoxCollidesWith(BoundingBox box, Mesh mesh)
		{
			Vector3 boxCenter = FindBoxCenter(box);
			Vector3 boxHalfSize = FindBoxHalfSize(box);

			return FindPolyBoxCollidesWith(box, boxCenter, boxHalfSize, mesh);
		}

		public static bool BoxCollidesWithMeshList(BoundingBox box, List<Mesh> meshes)
		{
			foreach (Mesh mesh in meshes)
			{
				if (BoxCollidesWithMesh(box, mesh))
					return true;

			}

			return false;
		}

		public static bool BoxCollidesWithPolygon(Vector3 bmin, Vector3 bmax, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal)
		{
			Vector3 boxCenter = FindBoxCenter(bmin, bmax);
			Vector3 boxHalfSize = FindBoxHalfSize(bmin, bmax);
			return BoxCollidesWithPolygon(bmin, bmax, boxCenter, boxHalfSize, v0, v1, v2, normal);
		}

		public static bool BoxCollidesWithPolygon(Vector3 bmin, Vector3 bmax, Vector3 boxcenter, Vector3 boxhalfsize, 
			Vector3 trivert0, Vector3 trivert1, Vector3 trivert2, Vector3 normal)
		{

			// if triangle is entirely inside the bounding box, then exit
			if ((trivert0.X >= bmin.X && trivert0.X <= bmax.X) && (trivert0.Y >= bmin.Y && trivert0.Y <= bmax.Y) && (trivert0.Z >= bmin.Z && trivert0.Z <= bmax.Z))
				if ((trivert1.X >= bmin.X && trivert1.X <= bmax.X) && (trivert1.Y >= bmin.Y && trivert1.Y <= bmax.Y) && (trivert1.Z >= bmin.Z && trivert1.Z <= bmax.Z))
					if ((trivert2.X >= bmin.X && trivert2.X <= bmax.X) && (trivert2.Y >= bmin.Y && trivert2.Y <= bmax.Y) && (trivert2.Z >= bmin.Z && trivert2.Z <= bmax.Z))
						return true;

			// if we get here triangle maybe intersecting the cube or could be totally out
			float d, min, max;
			float p0, p1, p2;
			Vector3 fe, vmin, vmax;
			Vector3 v0, v1, v2;
			Vector3 e0, e1, e2;

			// center at 0,0,0 
			v0 = trivert0 - boxcenter;
			v1 = trivert1 - boxcenter;
			v2 = trivert2 - boxcenter;

			// compute triangle edges 
			e0 = v1 - v0;
			e1 = v2 - v1;
			e2 = v0 - v2;

			//  test the 9 tests for axis overlapping 

			// axis test x01
			fe = VectorAbs(e0);

			p0 = e0.Z* v0.Y - e0.Y* v0.Z;
			p2 = e0.Z* v2.Y - e0.Y* v2.Z;

			min = p2;
			max = p0;

			if (p0<p2)
			{
				min = p0;
				max = p2;
			}

			d = fe.Z * boxhalfsize.Y + fe.Y * boxhalfsize.Z;

			if (min>d || max<-d) return false;

			// axis test y02

			p0 = -e0.Z * v0.X + e0.X * v0.Z;
			p2 = -e0.Z * v2.X + e0.X * v2.Z;

			min = p2;
			max = p0;

			if (p0<p2) { min = p0; max = p2; }

			d = fe.Z * boxhalfsize.X + fe.X * boxhalfsize.Z;

			if (min > d || max < -d) return false;

			// axis test z12

			p1 = e0.Y * v1.X - e0.X * v1.Y;
			p2 = e0.Y * v2.X - e0.X * v2.Y;

			min = p1;
			max = p2;

			if (p2<p1)
			{
				min = p2;
				max = p1;
			}

			d = fe.Y * boxhalfsize.X + fe.X * boxhalfsize.Y;

			if (min>d || max<-d) return false;

			fe = VectorAbs(e1);

			// axis test x01

			p0 = e1.Z* v0.Y - e1.Y* v0.Z;
			p2 = e1.Z* v2.Y - e1.Y* v2.Z;

			min = p2;
			max = p0;

			if (p0<p2)
			{
				min = p0;
				max = p2;
			}

			d = fe.Z* boxhalfsize.Y + fe.Y* boxhalfsize.Z;

			if (min>d || max<-d) return false;

			// axis test y02

			p0 = -e1.Z* v0.X + e1.X* v0.Z;
			p2 = -e1.Z* v2.X + e1.X* v2.Z;

			min = p2;
			max = p0;

			if (p0<p2)
			{
				min = p0;
				max = p2;
			}

			d = fe.Z* boxhalfsize.X + fe.X* boxhalfsize.Z;

			if (min>d || max<-d) return false;

			// axis test z0

			p0 = e1.Y* v0.X - e1.X* v0.Y;
			p1 = e1.Y* v1.X - e1.X* v1.Y;

			min = p1;
			max = p0;

			if (p0<p1)
			{
				min = p0;
				max = p1;
			}

			d = fe.Y* boxhalfsize.X + fe.X* boxhalfsize.Y;

			if (min>d || max<-d) return false;

			fe = VectorAbs(e2);

			// axis test x02

			p0 = e2.Z* v0.Y - e2.Y* v0.Z;
			p1 = e2.Z* v1.Y - e2.Y* v1.Z;

			min = p1;
			max = p0;

			if (p0<p1)
			{
				min = p0;
				max = p1;
			}

			d = fe.Z* boxhalfsize.Y + fe.Y* boxhalfsize.Z;

			if (min>d || max<-d) return false;

			// axis test y01

			p0 = -e2.Z* v0.X + e2.X* v0.Z;
			p1 = -e2.Z* v1.X + e2.X* v1.Z;

			min = p1;
			max = p0;

			if (p0<p1)
			{
				min = p0;
				max = p1;
			}

			d = fe.Z* boxhalfsize.X + fe.X* boxhalfsize.Z;

			if (min>d || max<-d) return false;

			// axis test z12 

			p1 = e2.Y* v1.X - e2.X* v1.Y;
			p2 = e2.Y* v2.X - e2.X* v2.Y;

			min = p1;
			max = p2;

			if (p2<p1)
			{
				min = p2;
				max = p1;
			}

			d = fe.Y* boxhalfsize.X + fe.X* boxhalfsize.Y;

			if (min>d || max<-d) return false;

			//  first test overlap in the {x,y,z}-directions 
			//  find min, max of the triangle each direction, 
			//  and test for overlap in 
			//  that direction -- this is equivalent to testing 
			//  a minimal AABB around 
			//  the triangle against the AABB 

			// test in X-direction 

			min = v0.X;
			max = v0.X;

			if (v1.X<min) min = v1.X;
			if (v1.X>max) max = v1.X;
			if (v2.X<min) min = v2.X;
			if (v2.X>max) max = v2.X;

			if (min>boxhalfsize.X || max<-boxhalfsize.X) return false;

			// test in Y-direction 

			min = v0.Y;
			max = v0.Y;

			if (v1.Y<min) min = v1.Y;
			if (v1.Y>max) max = v1.Y;
			if (v2.Y<min) min = v2.Y;
			if (v2.Y>max) max = v2.Y;

			if (min>boxhalfsize.Y || max<-boxhalfsize.Y) return false;

			// test in Z-direction 

			min = v0.Z;
			max = v0.Z;

			if (v1.Z<min) min = v1.Z;
			if (v1.Z>max) max = v1.Z;
			if (v2.Z<min) min = v2.Z;
			if (v2.Z>max) max = v2.Z;

			if (min>boxhalfsize.Z || max<-boxhalfsize.Z) return false;

			//  test if the box intersects the plane of the triangle 

			// test x

			if (normal.X>0.0f)
			{
				vmin.X = -boxhalfsize.X;
				vmax.X =  boxhalfsize.X;
			}
			else
			{
				vmin.X =  boxhalfsize.X;
				vmax.X = -boxhalfsize.X;
			}

			// test y

			if (normal.Y>0.0f)
			{
				vmin.Y = -boxhalfsize.Y;
				vmax.Y =  boxhalfsize.Y;
			}
			else
			{
				vmin.Y =  boxhalfsize.Y;
				vmax.Y = -boxhalfsize.Y;
			}

			// test z

			if (normal.Z>0.0f)
			{
				vmin.Z = -boxhalfsize.Z;
				vmax.Z =  boxhalfsize.Z;
			}
			else
			{
				vmin.Z =  boxhalfsize.Z;
				vmax.Z = -boxhalfsize.Z;
			}

			// test max and min 
			d = -(normal.X* v0.X + normal.Y* v0.Y + normal.Z* v0.Z);

			if (normal.X* vmin.X + normal.Y* vmin.Y + normal.Z* vmin.Z + d >  0.0f) return false;

			if (normal.X* vmax.X + normal.Y* vmax.Y + normal.Z* vmax.Z + d >= 0.0f) return true;

			// box and triangle don't overlap 
			return false;
		}

		public static Vector3 FindBoxCenter(BoundingBox box)
		{
			return FindBoxCenter(box.Min, box.Max);
		}

		public static Vector3 FindBoxCenter(Vector3 min, Vector3 max)
		{
			return new Vector3((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);
		}

		public static Vector3 FindBoxHalfSize(BoundingBox box)
		{
			return FindBoxHalfSize(box.Min, box.Max);
		}

		public static Vector3 FindBoxHalfSize(Vector3 min, Vector3 max)
		{
			return new Vector3(Math.Abs((max.X - min.X)) / 2, Math.Abs((max.Y - min.Y) / 2), Math.Abs((max.Z - min.Z) / 2));
		}

		private static Vector3 VectorAbs(Vector3 vec)
		{
			return new Vector3(Math.Abs(vec.X), Math.Abs(vec.Y), Math.Abs(vec.Z));
		}
	}
}
