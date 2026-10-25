using System;
using Microsoft.Xna.Framework;

using EQuestLib.Collisions;

namespace EQuestServer.Zones
{
	public class ZoneMap
	{
		private string _name;
		private MeshCollision[] _meshes;
		
		public string Name { get { return _name; } }

		public ZoneMap(string name, MeshCollision[] meshes)
		{
			_name = name;
			_meshes = meshes;
		}

		public float FindBestZ(Vector3 pos)
		{
			// Find a collision point below.
			Vector3 rayStart = new Vector3(pos.X, pos.Y, pos.Z + 3);
			Vector3 rayEnd = new Vector3(pos.X, pos.Y, pos.Z - 10000);

			float? dist = RayCollision.RayCollidesWithMesh(rayStart, rayEnd, _meshes);

			if (dist != null)
			{
				Ray r = new Ray(rayStart, rayEnd);
				return (r.Position + (r.Direction * dist.Value)).Z;
			}

			// Find a collision point above.
			rayStart = new Vector3(pos.X, pos.Y, pos.Z - 3);
			rayEnd = new Vector3(pos.X, pos.Y, pos.Z + 10000);

			dist = RayCollision.RayCollidesWithMesh(rayStart, rayEnd, _meshes);

			if (dist != null)
			{
				Ray r = new Ray(rayStart, rayEnd);
				return (r.Position + (r.Direction * dist.Value)).Z;
			}

			// No best Z found.
			return pos.Z;
		}
	}
}
