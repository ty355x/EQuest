using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3DFileLib;
using S3DFileLib.WldFrags;
using EQuestRender;
using EQuestLib.Collisions;

namespace EQuestServer.Zones
{
	public class ZoneMapReader
	{
		public ZoneMap ReadZoneMap(string zoneName)
		{
			string zoneFileName = zoneName + ".s3d"; ;
			Log.Info("Loading zone map file " + zoneFileName);

			string basePath = AppDomain.CurrentDomain.BaseDirectory;
			string s3dPath = Path.Combine(basePath, @"Data\Maps\" + zoneFileName);

			using (S3DFile s3dZone = S3DFile.Open(s3dPath))
			using (WLDFile wldZone = s3dZone.GetWldFile(zoneName + ".wld"))
			{
				var meshes = new List<MeshCollision>();

				foreach (var frag in wldZone.Frags)
				{
					if (frag.FragType == 0x36)
						meshes.Add(ReadMesh((Frag36)frag));
				}

				return new ZoneMap(zoneName, meshes.ToArray());
			}
		}

		protected MeshCollision ReadMesh(Frag36 frag36)
		{
			// Create frames
			MeshFrame[] frames = null;

			frames = new MeshFrame[] {
				CreateSingleFrame(frag36.Verts, frag36.Verts, frag36.Scale, frag36.CenterX, frag36.CenterY,
				frag36.CenterZ) };

			// Create collision polygons.
			var polygons = CreateCollisionPolys(frag36, frames[0].Vertices);

			// Create bounding box.
			BoundingBox bounds = CreateBounds(frag36);

			Vector3 center = new Vector3(frag36.CenterX, frag36.CenterY, frag36.CenterZ);

			return new MeshCollision(polygons, bounds, center, frag36.MaxDist);
		}

		protected short[] CreateIndices(Frag36 frag36)
		{
			var indices = new short[frag36.Polys.Length * 3];
			int i = 0;

			foreach (var poly in frag36.Polys)
			{
				indices[i++] = poly.V0;
				indices[i++] = poly.V1;
				indices[i++] = poly.V2;
			}

			return indices;
		}

		private MeshFrame CreateSingleFrame(FragVertex[] fragVerts, FragVertex[] texNormalVerts,
			short scale, float cx, float cy, float cz)
		{
			float vertScale = 1.0f / (1 << scale);
			var verts = new VertexPositionNormalTexture[fragVerts.Length];

			for (int i = 0; i < fragVerts.Length; i++)
			{
				var v = new VertexPositionNormalTexture();
				v.Position.X = cx + fragVerts[i].X * vertScale;
				v.Position.Y = cy + fragVerts[i].Y * vertScale;
				v.Position.Z = cz + fragVerts[i].Z * vertScale;
				v.TextureCoordinate.X = texNormalVerts[i].U / 256f;
				v.TextureCoordinate.Y = -texNormalVerts[i].V / 256f;

				verts[i] = v;
			}

			return new MeshFrame() { Vertices = verts };
		}

		private MeshPolygon[] CreatePolygons(Frag36 frag36)
		{
			var polys = new MeshPolygon[frag36.PolyTex.Length];

			int i = 0;
			foreach (var fragPoly in frag36.PolyTex)
				polys[i++] = new MeshPolygon(fragPoly.PolyCount, fragPoly.TexIndex);

			return polys;
		}

		private Polygon[] CreateCollisionPolys(Frag36 frag36, VertexPositionNormalTexture[] verts)
		{
			var colPolys = new Polygon[frag36.PolyCount];
			int i = 0;
			foreach (var fragPoly in frag36.Polys)
			{
				Vector3 v1 = new Vector3(
					verts[fragPoly.V0].Position.X,
					verts[fragPoly.V0].Position.Y,
					verts[fragPoly.V0].Position.Z);

				Vector3 v2 = new Vector3(
					verts[fragPoly.V1].Position.X,
					verts[fragPoly.V1].Position.Y,
					verts[fragPoly.V1].Position.Z);

				Vector3 v3 = new Vector3(
					verts[fragPoly.V2].Position.X,
					verts[fragPoly.V2].Position.Y,
					verts[fragPoly.V2].Position.Z);

				colPolys[i++] = new Polygon(v1, v2, v3, fragPoly.Flags);
			}

			return colPolys;
		}

		private BoundingBox CreateBounds(Frag36 frag)
		{
			return new BoundingBox(
				new Vector3(frag.MinX, frag.MinY, frag.MinZ),
				new Vector3(frag.MaxX, frag.MaxY, frag.MaxZ));
		}
	}
}

