using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;
using EQuestVR;
using EQuestVR.Valve;
using EQuestClient.Scenes;

namespace EQuestClient.VR
{
	public class VRService : IDisposable
	{
		private VrContext _vrContext;
		private Render _render;
		private RenderTarget2D _leftEye;
		private RenderTarget2D _rightEye;
		private GraphicsDevice _graphics;
		private SpriteBatch _screen2D;

		public VRService(Render render)
		{
			_render = render;
		}

		public void Init()
		{
			Debug.WriteLine("Initializing VR service...");

			if (!VrContext.CanCallNativeDll(out string error))
			{
				throw new Exception(error);
			}

			if (!VrContext.RuntimeInstalled())
			{
				throw new Exception("Failed to initialize VR service - Runtime not installed");
			}

			if (!VrContext.HmdConnected())
			{
				throw new Exception("Failed to initialize VR service - No VR device is detected");
			}

			_vrContext = VrContext.Get();

			try
			{
				_vrContext.Initialize();
			}
			catch (SharpVRException ex)
			{
				if (ex.ErrorCode == 108)
					throw new Exception("Failed to initialize VR service - No VR device is detected");
				else
					throw new Exception("Failed to initialize VR service - " + ex.Message);
			}
		}

		public void SetupRender(GraphicsDevice graphics)
		{
			_graphics = graphics;
			_vrContext.GetRenderTargetSize(out var width, out var height);
			_leftEye = new RenderTarget2D(graphics, width, height, true, SurfaceFormat.Color, DepthFormat.Depth16);
			_rightEye = new RenderTarget2D(graphics, width, height, true, SurfaceFormat.Color, DepthFormat.Depth16);
			_screen2D = new SpriteBatch(graphics);
		}

		public void DrawScene(Scene scene, GameTime time)
		{
			if (_vrContext == null)
				return;
			
			// Get the HMD matrix.
			_vrContext.WaitGetPoses();
			var hmdMatrix = _vrContext.Hmd.GetNextPose().ToMg();

			// Render left eye to buffer.
			_graphics.SetRenderTarget(_leftEye);
			scene.DrawVR(Eye.Left, hmdMatrix, time);
			
			// Render right eye to buffer.
			_graphics.SetRenderTarget(_rightEye);
			scene.DrawVR(Eye.Right, hmdMatrix, time);
			
			// Render buffers to HMD.
			_graphics.SetRenderTarget(null);
			
			var fieldInfo = typeof(Texture2D).GetField("glTexture", System.Reflection.BindingFlags.Instance | 
				System.Reflection.BindingFlags.NonPublic);
			var leftPtr = new IntPtr((int)fieldInfo.GetValue(_leftEye));
			var rightPtr = new IntPtr((int)fieldInfo.GetValue(_rightEye));
			
			_vrContext.Submit(Eye.Left, leftPtr, ETextureType.OpenGL);
			_vrContext.Submit(Eye.Right, rightPtr, ETextureType.OpenGL);

			// Render to screen (left eye only).
			_screen2D.Begin();
			_screen2D.Draw(_leftEye, Vector2.Zero, Color.White);
			_screen2D.End();
		}

		public void Dispose()
		{
			if (_vrContext != null)
			{
				_vrContext.Shutdown();
				_vrContext = null;
			}
		}

		public VRRenderMatrices MakeMatricesForEye(Eye eye, Matrix hmdMatrix, Vector3 position, Matrix rotation)
		{
			/*_vrContext.GetProjectionMatrix(eye, _render.Camera.NearPlane, _render.Camera.FarPlane, out var hmdProjection);
			_vrContext.GetEyeMatrix(eye, out var hmdEye);

			Matrix projectionMatrix = hmdProjection.ToMg();
			Matrix eyeMatrix = hmdEye.ToMg();

			// Vector3 transformedReference = Vector3.Transform(_render.Camera.CameraReference, Matrix.Identity);

			Vector3 transformedReference;
			Vector3.Transform(ref _render.Camera.CameraReference, ref rotation, out transformedReference);
			Vector3 target = position + transformedReference;

			var view = Matrix.CreateLookAt(position, target, _render.Camera.UpVector);
			// var forward = Vector3.TransformNormal(_render.Camera.ForwardVector, hmdMatrix * eyeMatrix);

			Matrix viewMatrix = view * Matrix.Invert(hmdMatrix) * eyeMatrix;

			return new VRRenderMatrices(viewMatrix, projectionMatrix);*/
			return new VRRenderMatrices();
		}
	}
}
