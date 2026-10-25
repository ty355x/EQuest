using System;
using Microsoft.Xna.Framework;

namespace EQuestRender
{
	public class ZoneObject : IDisposable
	{
		public Mesh Mesh { get { return _mesh; } }
		public BoundingBox Bounds;
		public CollisionPoly[] CollisionPolys;
		
		public Vector3 Position = Vector3.Zero;
		public Vector3 Rotation = Vector3.Zero;
		public Vector3 Scale = Vector3.Zero;
		public Matrix Transform;
		public bool IsSmall { get; private set; }
		
		public bool ShouldDraw { get; set; }
		public bool Flag { get; set; }
		public OcclusionTracker Occlusion { get; set; }

		private Mesh _mesh;

		public ZoneObject(Mesh mesh, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			_mesh = mesh;
			
			this.Position = position;
			this.Rotation = rotation;
			this.Scale = scale;

			Transform =
				Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
				Matrix.CreateRotationY(MathHelper.ToRadians(-rotation.Y)) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z)) *
				Matrix.CreateScale(Scale) *
				Matrix.CreateTranslation(position);

			CreateCollisionObjects();
			DetermineIsSmall();
		}

		private void DetermineIsSmall()
		{
			Vector3 size = Bounds.Max - Bounds.Min;
			IsSmall = size.X < 10 && size.Y < 10 && size.Z < 10;
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(_mesh);
			_mesh = null;
		}

		private void CreateCollisionObjects()
		{
			var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			
			CollisionPolys = new CollisionPoly[_mesh.CollisionPolys.Length];

			for (int i = 0; i < CollisionPolys.Length; i++)
			{
				Vector3 v0, v1, v2;
				
				Vector3.Transform(ref _mesh.CollisionPolys[i].V0, ref Transform, out v0);
				Vector3.Transform(ref _mesh.CollisionPolys[i].V1, ref Transform, out v1);
				Vector3.Transform(ref _mesh.CollisionPolys[i].V2, ref Transform, out v2);

				CollisionPolys[i] = new CollisionPoly(v0, v1, v2, _mesh.CollisionPolys[i].Flags);

				UpdateBoundsVectors(ref v0, ref min, ref max);
				UpdateBoundsVectors(ref v1, ref min, ref max);
				UpdateBoundsVectors(ref v2, ref min, ref max);
			}

			Bounds = new BoundingBox(min, max);
		}

		private void UpdateBoundsVectors(ref Vector3 vec, ref Vector3 min, ref Vector3 max)
		{
			min = new Vector3(Math.Min(vec.X, min.X), Math.Min(vec.Y, min.Y), Math.Min(vec.Z, min.Z));
			max = new Vector3(Math.Max(vec.X, max.X), Math.Max(vec.Y, max.Y), Math.Max(vec.Z, max.Z));
		}
	}
}
