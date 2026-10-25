using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3DFileLib;
using S3DFileLib.WldFrags;

namespace EQuestRender.Readers
{
	public abstract class BaseModelReader
	{
		public Mesh ReadMesh(string name, Frag36 frag36, TextureListContainer textures, WLDFile wld, GraphicsDevice graphics,
			bool isMob)
		{
			// Find the texture set for this model.
			var textureList = textures.FindTextureList(frag36.Frag1 - 1);

			if (textureList == null)
			{
				throw new Exception("Unable to find texture list with frag index " + (frag36.Frag1 - 1) + " for model " + frag36.FragName);
			}

			// Create frames
			MeshFrame[] frames = null;

			if (frag36.Frag2 == 0)
			{
				frames = new MeshFrame[] 
				{
					CreateSingleFrame(frag36.Verts, frag36.Verts, frag36.Scale, frag36.CenterX, frag36.CenterY, frag36.CenterZ) 
				};
			}
			else
			{
				frames = CreateFrames(frag36, wld);
			}

			// Create polygons;
			var polys = CreatePolygons(frag36);

			// Create collision polygons.
			var colPolys = CreateCollisionPolys(frag36, frames[0].Vertices);

			// Create indices.
			short[] indices = CreateIndices(frag36);
			
			MeshPiece[] pieces = null;

			// Create mesh pieces.
			if (frag36.VertexPieces != null)
			{
				pieces = new MeshPiece[frag36.VertexPieces.Length];
				short vertIndex = 0;

				for (int i = 0; i < frag36.VertexPieces.Length; i++)
				{
					VertexPiece piece = frag36.VertexPieces[i];

					pieces[i] = new MeshPiece(
						piece.PieceIndex,
						vertIndex,
						piece.VertexCount);

					vertIndex += piece.VertexCount;
				}
			}

			// Create bounding box.
			BoundingBox bounds = CreateBounds(frag36);

			Vector3 center = new Vector3(frag36.CenterX, frag36.CenterY, frag36.CenterZ);

			// Create the model.
			if (isMob)
			{
				return Mesh.MakeMobMesh(name, frag36.FragIndex, polys, colPolys, frames, indices, bounds,
					center, frag36.MaxDist, textureList, pieces, graphics);
			}
			else
			{
				return Mesh.MakeMesh(name, frag36.FragIndex, polys, colPolys, frames, indices, bounds, center, frag36.MaxDist,
					textureList, graphics);
			}
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

		protected MeshFrame[] CreateFrames(Frag36 frag36, WLDFile wld)
		{
			// Get the Frag27 for this model.
			var frag2F = (Frag2F) wld.FindFragByRef(frag36.Frag2);
			var frag27 = (Frag37) wld.FindFragByRef(frag2F.Ref);

			MeshFrame[] meshFrames = new MeshFrame[frag27.FrameCount];
			
			// Loop through the frames in the Frag27.
			for (int i = 0; i < frag27.Frames.Length; i++)
			{
				// Create the model frame for the current frame.
				meshFrames[i] = CreateSingleFrame(frag27.Frames[i].Verts, frag36.Verts, frag27.Scale,
					frag36.CenterX, frag36.CenterY, frag36.CenterZ);
			}

			return meshFrames;
		}

		protected MeshFrame CreateSingleFrame(FragVertex[] fragVerts, FragVertex[] texNormalVerts,
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
				
				v.TextureCoordinate.X = texNormalVerts[i].U / 256.0f;
				v.TextureCoordinate.Y = -texNormalVerts[i].V / 256.0f;
				
				v.Normal.X = fragVerts[i].NX / 128.0f;
				v.Normal.Y = fragVerts[i].NY / 128.0f;
				v.Normal.Z = fragVerts[i].NZ / 128.0f;
				v.Normal.Normalize();

				verts[i] = v;
			}

			return new MeshFrame() { Vertices = verts };
		}

		protected MeshPolygon[] CreatePolygons(Frag36 frag36)
		{
			var polys = new MeshPolygon[frag36.PolyTex.Length];

			int i = 0;
			foreach (var fragPoly in frag36.PolyTex)
				polys[i++] = new MeshPolygon(fragPoly.PolyCount, fragPoly.TexIndex);

			return polys;
		}

		protected CollisionPoly[] CreateCollisionPolys(Frag36 frag36, VertexPositionNormalTexture[] verts)
		{
			var colPolys = new CollisionPoly[frag36.PolyCount];
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

				colPolys[i++] = new CollisionPoly(v1, v2, v3, fragPoly.Flags);
			}

			return colPolys;
		}

		protected BoundingBox CreateBounds(Frag36 frag)
		{
			return new BoundingBox(
				new Vector3(frag.MinX, frag.MinY, frag.MinZ),
				new Vector3(frag.MaxX, frag.MaxY, frag.MaxZ));
		}
	}
}
