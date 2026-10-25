using System;
using Microsoft.Xna.Framework;

namespace EQuestRender
{
	public class CollisionPoly
	{
		public Vector3 V0;
		public Vector3 V1;
		public Vector3 V2;

		public Vector3 Normal;

		public Plane Plane;
		public int Flags; // 16 = water

		public CollisionPoly(Vector3 v0, Vector3 v1, Vector3 v2, int flags)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;

			CalcNormal();

			this.Flags = flags;
			Plane = new Plane(this.V0, this.V1, this.V2);
		}

		private void CalcNormal()
		{
			Vector3 normal = Vector3.Cross((V1 - V0), (V2 - V0));
			normal.Normalize();
			this.Normal = normal;
		}

	#region Debug
#if DEBUG2
		private VertexBuffer _vertexBuffer;

		public void MakeVertexBuffer(GraphicsDevice graphics)
		{
			var vertices = new VertexPositionNormalTexture[] {
				new VertexPositionNormalTexture(V0, Normal, new Vector2(0, 0)),
				new VertexPositionNormalTexture(V1, Normal, new Vector2(1, 1)),
				new VertexPositionNormalTexture(V2, Normal, new Vector2(1, 0))};

			_vertexBuffer = new VertexBuffer(graphics, typeof(VertexPositionNormalTexture), 3, BufferUsage.WriteOnly);
			_vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
		}

		public void Draw(Render render)
		{
			render.DrawPoly(_vertexBuffer);
		}

		public void Dispose()
		{
			RenderUtil.Dispose(_vertexBuffer);
			_vertexBuffer = null;
		}
#endif
		#endregion

	}
}
