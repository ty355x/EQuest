using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class Camera
	{
		public Vector3 Position { get; set; }
		public Matrix Rotation { get; set; }

		public Matrix World { get; set; }
		public Matrix View { get; set; }
		public Matrix Projection { get; set; }
		public Matrix WorldViewProjection { get; private set; }

		public float NearPlane { get; set; }
		public float FarPlane { get; set; }
		public float FieldOfView { get; set; }

		public Vector3 UpVector { get; private set; }
		public Vector3 ForwardVector { get; private set; }
		public Vector3 CameraReference { get; private set; }

		public BoundingFrustum Frustrum { get; private set; }

		private GraphicsDevice _graphics;
		
		public Camera(GraphicsDevice graphics)
		{
			_graphics = graphics;

			Position = Vector3.Zero;
			Rotation = Matrix.Identity;
			World = Matrix.CreateScale(1);
			View = Matrix.Identity;
			Projection = Matrix.Identity;
			NearPlane = 1f;
			FarPlane = 5000f;
			FieldOfView = 40f;
			UpVector = Vector3.UnitZ;
			ForwardVector = Vector3.UnitY;
			CameraReference = new Vector3(0, -1, 0);
			Frustrum = new BoundingFrustum(Matrix.Identity);
			
			Update();
		}

		public void Update()
		{
			// Calculate the view matrix.
			Vector3 cameraDirection = Vector3.Transform(CameraReference, Rotation);
			Vector3 target = Position + cameraDirection;
			View = Matrix.CreateLookAt(Position, target, UpVector);

			// Calculate the projection matrix.
			float aspectRatio = (float)_graphics.Viewport.Width / (float)_graphics.Viewport.Height;
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfView), aspectRatio, NearPlane, FarPlane);

			// Calculate the WorldViewProjection.
			// WorldViewProjection = World * View * Projection;

			// Calculate the frustrum.			
			Frustrum.Matrix = View * Projection;
		}

		public bool ContainsBox(BoundingBox box)
		{
			return Frustrum.Contains(box) != ContainmentType.Disjoint;
		}

		public bool ContainsMesh(Mesh mesh)
		{
			if (mesh.CollisionMethod == MeshCollisionMethod.BoundingBox)
				return Frustrum.Contains(mesh.Bounds) != ContainmentType.Disjoint;
			else
				return Frustrum.Contains(mesh.BoundingSphere) != ContainmentType.Disjoint;
		}
	}
}
