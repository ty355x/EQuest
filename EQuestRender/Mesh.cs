using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class Mesh : IDisposable
	{
		private const double FrameChangeIntervalMs = 100;

		public int RenderCount;
		
		public BoundingBox CalculatedBounds;
		public BoundingBox Bounds;
		public BoundingSphere BoundingSphere;
		public MeshCollisionMethod CollisionMethod;
		public Matrix Transform = Matrix.Identity;
		
		protected string _name;
		protected TextureList _textureList;
		protected TextureList _currentTextureList;
		protected MeshPolygon[] _polygons;
		protected CollisionPoly[] _collisionPolys;
		protected MeshFrame[] _frames;
		protected MeshPiece[] _pieces;
		protected short[] _indices;
		protected VertexBuffer[] _vertexBuffers;
		protected VertexBuffer _currentBuffer;
		protected IndexBuffer _indexBuffer;
		protected int _currentFrame = 0;
		protected double _nextFrameChange = -1;
		
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public MeshPolygon[] Polygons
		{
			get { return _polygons; }
		}

		public CollisionPoly[] CollisionPolys
		{
			get { return _collisionPolys; }
		}

		public TextureList Textures
		{
			get { return _currentTextureList; }
			set { _currentTextureList = value; }
		}

		public MeshPiece[] Pieces
		{
			get { return _pieces; }
		}

		public MeshFrame[] Frames
		{
			get { return _frames; }
		}

		public VertexBuffer VertexBuffer
		{
			get { return _currentBuffer; }
		}

		public IndexBuffer IndexBuffer
		{
			get { return _indexBuffer; }
		}

		public short[] Indices
		{
			get { return _indices; }
		}

		public float Height
		{
			get
			{
				if (CollisionMethod == MeshCollisionMethod.BoundingBox)
					return Bounds.Max.Z - Bounds.Min.Z;
				else
					return BoundingSphere.Radius;
			}
		}

		public static Mesh MakeMesh(string name, int fragIndex, MeshPolygon[] polygons, CollisionPoly[] collisionPolys, 
			MeshFrame[] frames, short[] indices, BoundingBox bounds, Vector3 center, float maxDist, TextureList textures, 
			GraphicsDevice graphics)
		{
			var m = new Mesh(name, fragIndex, polygons, collisionPolys, frames, indices, bounds, center, maxDist, textures, 
				null, graphics);
			m.CreateVertexBuffers(graphics);
			m.CreateIndexBuffer(graphics);
			return m;
		}

		public static MobMesh MakeMobMesh(string name, int fragIndex, MeshPolygon[] polygons, CollisionPoly[] collisionPolys, 
			MeshFrame[] frames, short[] indices, BoundingBox bounds, Vector3 center, float maxDist, TextureList textures, 
			MeshPiece[] pieces, GraphicsDevice graphics)
		{
			var m = new MobMesh(name, fragIndex, polygons, collisionPolys, frames, indices, bounds, center, maxDist, 
				textures, pieces, graphics);
			m.CreateIndexBuffer(graphics);
			return m;
		}
		
		public float CalcDistanceSquared(Vector3 v)
		{
			if (CollisionMethod == MeshCollisionMethod.BoundingBox)
			{
				return Math.Abs(Vector3.DistanceSquared(v, (Bounds.Max - Bounds.Min) / 2));
			}
			else
			{
				return Math.Abs(Vector3.DistanceSquared(v, BoundingSphere.Center));
			}
		}

		protected Mesh(string name, int fragIndex, MeshPolygon[] polygons, CollisionPoly[] collisionPolys, 
			MeshFrame[] frames, short[] indices, BoundingBox bounds, Vector3 center, float maxDist, 
			TextureList textureList, MeshPiece[] pieces, GraphicsDevice graphics)
		{
			_name = name;
			_polygons = polygons;
			_collisionPolys = collisionPolys;
			_frames = frames;
			_textureList = textureList;
			_currentTextureList = _textureList;
			_pieces = pieces;
			_indices = indices;
			
			Bounds = bounds;
			BoundingSphere = new BoundingSphere(center, maxDist);

			if (bounds.Min == Vector3.Zero && bounds.Max == Vector3.Zero)
				CollisionMethod = MeshCollisionMethod.Sphere;
			else
				CollisionMethod = MeshCollisionMethod.BoundingBox;

			CalculateCalculatedBounds();
		}

		public void Update(GameTime time)
		{
			if (_frames == null)
				return;

			if (_nextFrameChange == -1)
			{
				_nextFrameChange = time.TotalGameTime.TotalMilliseconds + FrameChangeIntervalMs;
				return;
			}

			if (time.TotalGameTime.TotalMilliseconds >= _nextFrameChange)
			{
				_currentFrame++;

				if (_currentFrame >= _frames.Length)
					_currentFrame = 0;

				_nextFrameChange = time.TotalGameTime.TotalMilliseconds + FrameChangeIntervalMs;
				_currentBuffer = _vertexBuffers[_currentFrame];
			}
		}

		public void CalculateCalculatedBounds()
		{
			Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			foreach (VertexPositionNormalTexture vert in Frames[0].Vertices)
			{
				min.X = Math.Min(min.X, vert.Position.X);
				min.Y = Math.Min(min.Y, vert.Position.Y);
				min.Z = Math.Min(min.Z, vert.Position.Z);
				max.X = Math.Max(max.X, vert.Position.X);
				max.Y = Math.Max(max.Y, vert.Position.Y);
				max.Z = Math.Max(max.Z, vert.Position.Z);
			}

			CalculatedBounds = new BoundingBox(min, max);
		}

		public void CreateVertexBuffers(GraphicsDevice graphics)
		{
			RenderUtil.Dispose(_vertexBuffers);
			_vertexBuffers = new VertexBuffer[_frames.Length];

			// Create the vertex buffers.
			for (int i = 0; i < _frames.Length; i++)
			{
				MeshFrame frame = _frames[i];
				_vertexBuffers[i] = new VertexBuffer(graphics, VertexPositionNormalTexture.VertexDeclaration, 
					_frames[i].Vertices.Length, BufferUsage.WriteOnly);

				_vertexBuffers[i].SetData(_frames[i].Vertices);
			}

			_currentBuffer = _vertexBuffers[_currentFrame];

#if DEBUG2
			foreach (var ply in _collisionPolys)
				ply.MakeVertexBuffer(graphics);
#endif
		}

		public void CreateIndexBuffer(GraphicsDevice graphics)
		{
			RenderUtil.DisposeIfNotNull(_indexBuffer);
			_indexBuffer = new IndexBuffer(graphics, typeof(short), _indices.Length, BufferUsage.WriteOnly);
			_indexBuffer.SetData(_indices);
		}

		public void SetCurrentTextureList(int textureListIndex)
		{
			_currentTextureList = _textureList.GetAltTextureList(textureListIndex);
		}

		public virtual void Dispose()
		{
			RenderUtil.Dispose(_vertexBuffers);
			_vertexBuffers = null;

			RenderUtil.DisposeIfNotNull(_indexBuffer);
			_indexBuffer = null;
		}

		public override string ToString()
		{
			return string.Format("Mesh Name: {0}, Vertices: {1}, Indices: {2}, Polygons {3}",
				Name, _frames[0].Vertices.Length, _indices.Length, _collisionPolys.Length);
		}

		#region Debug Region
#if DEBUG2

		public void DrawCollisionPoly(Render render)
		{
			foreach (var poly in _collisionPolys)
				poly.Draw(render);
		}

#endif
		#endregion
	}
}
