using System.IO;

namespace S3DFileLib.WldFrags
{
	public struct VertexPiece
	{
		public short PieceIndex;
		public short VertexCount;

		public override string ToString()
		{
			return string.Format("Index: {0},\tVertex Count: {1}", PieceIndex, VertexCount);
		}

		public string ToTabbedString()
		{
			return string.Format("{0}\t{1}", PieceIndex, VertexCount);
		}
	}

	public class Frag36 : Frag
	{
		public float Flags;

		public int Frag1;
		public int Frag2;
		public int Frag3;
		public int Frag4;

		public float CenterX;
		public float CenterY;
		public float CenterZ;

		public int Params2_0;
		public int Params2_1;
		public int Params2_2;

		public float MaxDist;
		public float MinX;
		public float MinY;
		public float MinZ;
		public float MaxX;
		public float MaxY;
		public float MaxZ;

		public short VertCount;
		public short TexCoordsCount;
		public short NormalsCount;
		public short ColorCount;
		public short PolyCount;
		public short VertPieceCount;
		public short PolyTexCount;
		public short VertTexCount;
		public short Size9;
		public short Scale;

		public FragVertex[] Verts;
		public FragPoly[] Polys;
		public FragPolyTex[] PolyTex;
		public VertexPiece[] VertexPieces;

		public Frag36(BinaryReader reader)
		{
			Flags = reader.ReadInt32();
			Frag1 = reader.ReadInt32();
			Frag2 = reader.ReadInt32();
			Frag3 = reader.ReadInt32();
			Frag4 = reader.ReadInt32();

			CenterX = reader.ReadSingle();
			CenterY = reader.ReadSingle();
			CenterZ = reader.ReadSingle();

			Params2_0 = reader.ReadInt32();
			Params2_1 = reader.ReadInt32();
			Params2_2 = reader.ReadInt32();

			MaxDist = reader.ReadSingle();
			MinX = reader.ReadSingle();
			MinY = reader.ReadSingle();
			MinZ = reader.ReadSingle();
			MaxX = reader.ReadSingle();
			MaxY = reader.ReadSingle();
			MaxZ = reader.ReadSingle();

			VertCount = reader.ReadInt16();
			TexCoordsCount = reader.ReadInt16();
			NormalsCount = reader.ReadInt16();
			ColorCount = reader.ReadInt16();
			PolyCount = reader.ReadInt16();
			VertPieceCount = reader.ReadInt16();
			PolyTexCount = reader.ReadInt16();
			VertTexCount = reader.ReadInt16();
			Size9 = reader.ReadInt16();
			Scale = reader.ReadInt16();

			// Read the vertices.
			Verts = new FragVertex[VertCount];
			for (int i = 0; i < VertCount; i++)
			{
				Verts[i] = new FragVertex();
				Verts[i].X = reader.ReadInt16();
				Verts[i].Y = reader.ReadInt16();
				Verts[i].Z = reader.ReadInt16();
			}

			// Read the texture coordinates.
			for (int i = 0; i < TexCoordsCount; i++)
			{
				Verts[i].U = reader.ReadInt16();
				Verts[i].V = reader.ReadInt16();
			}

			// Read normals.
			for (int i = 0; i < NormalsCount; i++)
			{
				Verts[i].NX = reader.ReadSByte();
				Verts[i].NY = reader.ReadSByte();
				Verts[i].NZ = reader.ReadSByte();
			}

			// reader.BaseStream.Position += 3 * NormalsCount;
			reader.BaseStream.Position += 4 * ColorCount;

			// Read the polygons.
			Polys = new FragPoly[PolyCount];
			for (int i = 0; i < PolyCount; i++)
			{
				Polys[i] = new FragPoly();
				Polys[i].Flags = reader.ReadInt16();
				Polys[i].V0 = reader.ReadInt16();
				Polys[i].V1 = reader.ReadInt16();
				Polys[i].V2 = reader.ReadInt16();
			}

			// Read the vertex pieces.
			if (VertPieceCount > 0)
			{ 
				VertexPieces = new VertexPiece[VertPieceCount];
				for (int i = 0; i < VertPieceCount; i++)
				{
					VertexPieces[i] = new VertexPiece();
					VertexPieces[i].VertexCount = reader.ReadInt16();
					VertexPieces[i].PieceIndex = reader.ReadInt16();
				}
			}

			// Read the textures.
			PolyTex = new FragPolyTex[PolyTexCount];
			for (int i = 0; i < PolyTexCount; i++)
			{
				PolyTex[i] = new FragPolyTex();
				PolyTex[i].PolyCount = reader.ReadInt16();
				PolyTex[i].TexIndex = reader.ReadInt16();
			}				
		}
	}
}
