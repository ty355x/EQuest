using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class UnitCube : IDisposable
	{
		public VertexBuffer VertexBuffer;
		public IndexBuffer IndexBuffer;

		protected VertexPositionColor[] _vertices = new VertexPositionColor[8];
		protected short[] _indices = new short[36];

		private const float Size = 0.1f;

		public UnitCube(GraphicsDevice graphics)
		{
			VertexBuffer = new VertexBuffer(graphics, typeof(VertexPositionColor), _vertices.Length, BufferUsage.WriteOnly);
			IndexBuffer = new IndexBuffer(graphics, typeof(short), _indices.Length, BufferUsage.WriteOnly);
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(VertexBuffer);
			VertexBuffer = null;

			RenderUtil.DisposeIfNotNull(IndexBuffer);
			IndexBuffer = null;
		}

		private void MakeCube(GraphicsDevice graphics)
		{
			Color Color1 = Color.Red;
			Color Color2 = Color.Red;

			_vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color1);
			_vertices[1] = new VertexPositionColor(new Vector3(Size, 0, 0), Color1);
			_vertices[2] = new VertexPositionColor(new Vector3(Size, -Size, 0), Color1);
			_vertices[3] = new VertexPositionColor(new Vector3(0, -Size, 0), Color1);
			_vertices[4] = new VertexPositionColor(new Vector3(0, 0, Size), Color1);
			_vertices[5] = new VertexPositionColor(new Vector3(Size, 0, Size), Color1);
			_vertices[6] = new VertexPositionColor(new Vector3(Size, -Size, Size), Color1);
			_vertices[7] = new VertexPositionColor(new Vector3(0, -Size, Size), Color1);

			VertexBuffer.SetData<VertexPositionColor>(_vertices);

			_indices[0] = 0; _indices[1] = 1; _indices[2] = 2;
			_indices[3] = 0; _indices[2] = 2; _indices[3] = 3;
			_indices[6] = 1; _indices[7] = 5; _indices[8] = 6;
			_indices[9] = 1; _indices[10] = 6; _indices[11] = 2;
			_indices[12] = 5; _indices[13] = 4; _indices[14] = 7;
			_indices[15] = 5; _indices[16] = 7; _indices[17] = 6;
			_indices[18] = 0; _indices[19] = 3; _indices[20] = 7;
			_indices[21] = 0; _indices[22] = 7; _indices[23] = 4;
			_indices[24] = 4; _indices[25] = 5; _indices[26] = 1;
			_indices[27] = 4; _indices[28] = 1; _indices[29] = 0;
			_indices[30] = 2; _indices[31] = 6; _indices[32] = 7;
			_indices[33] = 2; _indices[34] = 7; _indices[35] = 3;

			IndexBuffer.SetData(_indices);
		}
	}
}
