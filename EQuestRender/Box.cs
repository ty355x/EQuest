using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class Box : UnitCube, IDisposable
	{
		public Box(GraphicsDevice graphics) : base(graphics)
		{
		}

		public void RemakeCube(Vector3 min, Vector3 max, GraphicsDevice graphics)
		{
			Color Color1 = Color.Red;
			Color Color2 = Color.Red;

			_vertices[0] = new VertexPositionColor(new Vector3(min.X, max.Y, max.Z), Color1);
			_vertices[1] = new VertexPositionColor(max, Color1);
			_vertices[2] = new VertexPositionColor(new Vector3(max.X, min.Y, max.Z), Color1);
			_vertices[3] = new VertexPositionColor(new Vector3(min.X, min.Y, max.Z), Color1);
			_vertices[4] = new VertexPositionColor(new Vector3(min.X, max.Y, min.Z), Color1);
			_vertices[5] = new VertexPositionColor(new Vector3(max.X, max.Y, min.Z), Color1);
			_vertices[6] = new VertexPositionColor(new Vector3(max.X, min.Y, min.Z), Color1);
			_vertices[7] = new VertexPositionColor(min, Color1);

			_indices[0] = 0; _indices[1] = 1; _indices[2] = 2;
			_indices[3] = 0; _indices[2] = 2; _indices[3] = 3;
			_indices[6] = 4; _indices[7] = 7; _indices[8] = 6;
			_indices[9] = 4; _indices[10] = 6; _indices[11] = 5;
			_indices[12] = 4; _indices[13] = 5; _indices[14] = 1;
			_indices[15] = 4; _indices[16] = 1; _indices[17] = 0;
			_indices[18] = 2; _indices[19] = 6; _indices[20] = 7;
			_indices[21] = 2; _indices[22] = 7; _indices[23] = 3;
			_indices[24] = 4; _indices[25] = 0; _indices[26] = 3;
			_indices[27] = 4; _indices[28] = 3; _indices[29] = 7;
			_indices[30] = 1; _indices[31] = 5; _indices[32] = 6;
			_indices[33] = 1; _indices[34] = 6; _indices[35] = 2;

			VertexBuffer.SetData<VertexPositionColor>(_vertices);
			IndexBuffer.SetData(_indices);
		}
	}
}
