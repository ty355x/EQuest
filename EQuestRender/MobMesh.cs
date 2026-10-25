using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class MobMeshPiece
	{
		public int VertexIndex;
		public int VertexCount;
	}

	public class MobMesh : Mesh, IDisposable
	{
		public VertexPositionNormalTexture[] Vertices;
		public new DynamicVertexBuffer VertexBuffer { get { return _vertexBuffer; } }
		private DynamicVertexBuffer _vertexBuffer;

		public MobMesh(string name, int fragIndex, MeshPolygon[] polygons, CollisionPoly[] collisionPolys,
			MeshFrame[] frames, short[] indices, BoundingBox bounds, Vector3 center, float maxDist,
			TextureList textureList, MeshPiece[] pieces, GraphicsDevice graphics) : 
			base(name, fragIndex, polygons, collisionPolys, frames, indices, bounds, center, maxDist, textureList, pieces, graphics)
		{
			// Create the vertices array for this mesh.
			Vertices = new VertexPositionNormalTexture[_frames[0].Vertices.Length]; _frames[0].Vertices.CopyTo(Vertices, 0);
			
			// Create the vertex buffer.
			_vertexBuffer = new DynamicVertexBuffer(graphics, VertexPositionNormalTexture.VertexDeclaration,
				frames[0].Vertices.Length, BufferUsage.WriteOnly);
		}

		public MobMesh Copy(GraphicsDevice graphics)
		{
			var copy = new MobMesh(_name, 0, _polygons, _collisionPolys, _frames, _indices, Bounds, BoundingSphere.Center, 
				BoundingSphere.Radius, Textures, Pieces, graphics);

			// Create the vertices array for this mesh.
			copy.Vertices = new VertexPositionNormalTexture[_frames[0].Vertices.Length];
			copy._frames[0].Vertices.CopyTo(copy.Vertices, 0);

			// Create the vertex buffer.
			copy._vertexBuffer = new DynamicVertexBuffer(graphics, VertexPositionNormalTexture.VertexDeclaration,
				copy._frames[0].Vertices.Length, BufferUsage.WriteOnly);

			copy.CreateVertexBuffers(graphics);
			copy.CreateIndexBuffer(graphics);
			
			return copy;
		}

		public override void Dispose()
		{
			base.Dispose();

			RenderUtil.DisposeIfNotNull(_vertexBuffer);
			_vertexBuffer = null;
		}
	}
}
