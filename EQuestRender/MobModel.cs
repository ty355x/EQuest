using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class MobModel : IDisposable
	{
		public static double AnimationFrameLength = 150;

		public string Name { get; private set; }
		
		public MobMesh BodyMesh { get; private set; }
		public MobMesh HeadMesh { get; private set; }
		public MobMesh[] HeadMeshes { get; private set; }

		public SkeletonPiece[] Skeleton { get; private set; }
				
		public List<MobModelAnimation> Animations { get; private set; }
		public MobModelAnimation CurrentAnimation { get { return Animations[_animationIndex]; } }

		public int AnimationCount { get { return Animations.Count; } }
		public int AnimationIndex { get { return _animationIndex; } }
		public int AnimationFrameIndex { get { return _animationFrameIndex; } }

		public float RaceSize { get; private set; }
		public float Height { get { return 6; } }

		private int _animationIndex = 0;
		private int _animationFrameIndex = 0;
		private bool _repeatAnimation = false;

		private int _previousAnimationIndex = -1;
		private int _previousAnimationFrameIndex = -1;

		private string _fragName;
		private double _frameStartTime = 0;
		
		public int FrameCount
		{
			get
			{
				if (CurrentAnimation.Skeleton.Length > 1 && CurrentAnimation.Skeleton[1] != null)
					return CurrentAnimation.Skeleton[1].Frames.Count;
				else
					return 1;
			}
		}

		public MobModel(string fragName, MobMesh bodyMesh, MobMesh[] headMeshes, SkeletonPiece[] skeleton, GraphicsDevice graphics)
		{
			_fragName = fragName;
			
			if (_fragName != null && _fragName.Length > 0)
				this.Name = _fragName.Substring(0, _fragName.IndexOf('_'));

			this.BodyMesh = bodyMesh;
			this.HeadMeshes = headMeshes;
			this.Skeleton = skeleton;
			
			Animations = new List<MobModelAnimation>();
			UpdateCurrentHeadMesh(0);
		}

		public void SetTextureIndex(int textureIndex)
		{
			if (BodyMesh != null)
				BodyMesh.SetCurrentTextureList(textureIndex);

			UpdateCurrentHeadMesh(textureIndex);

			if (HeadMesh != null)
				HeadMesh.SetCurrentTextureList(textureIndex);
		}

		public MobModel Copy(GraphicsDevice graphics)
		{
			MobMesh body = BodyMesh != null ? BodyMesh.Copy(graphics) : null;
			MobMesh[] heads = new MobMesh[HeadMeshes.Length];

			for (int i = 0; i < HeadMeshes.Length; i++)
				heads[i] = HeadMeshes[i].Copy(graphics);

			var model = new MobModel(_fragName, body, heads, Skeleton, graphics);
			model.Animations = Animations;
			
			return model;
		}

		public void SetAnimation(string name, bool repeat, double time)
		{
			_repeatAnimation = repeat;
			int newAnimationIndex = 0;

			for (int i = 0; i < Animations.Count; i++)
			{
				if (Animations[i].Name == name)
				{
					newAnimationIndex = i;
					break;
				}
			}

			_previousAnimationIndex = _animationIndex;
			_previousAnimationFrameIndex = _animationFrameIndex; 
						
			_animationIndex = newAnimationIndex;
			_animationFrameIndex = 0;
			_frameStartTime = time;
		}

		public void SetAnimation(int index, bool repeat, double time)
		{
			_repeatAnimation = repeat;
			
			if (index >= Animations.Count)
				index = 0;

			_previousAnimationIndex = _animationIndex;
			_previousAnimationFrameIndex = _animationFrameIndex;

			_animationIndex = index;
			_animationFrameIndex = 0;
			_frameStartTime = time;
		}

		public void NextFrame(double time)
		{
			_previousAnimationIndex = _animationIndex;
			_previousAnimationFrameIndex = _animationFrameIndex;

			_frameStartTime = time;
			_animationFrameIndex++;

			if (_animationFrameIndex >= FrameCount)
				_animationFrameIndex = 0;
		}

		public void PreviousFrame(double time)
		{
			_previousAnimationIndex = _animationIndex;
			_previousAnimationFrameIndex = _animationFrameIndex;

			_frameStartTime = time;
			_animationFrameIndex--;

			if (_animationFrameIndex < 0)
				_animationFrameIndex = FrameCount - 1;
		}

		public bool HasAnimation(string name)
		{
			foreach (var anim in Animations)
			{
				if (anim.Name == name)
					return true;
			}

			return false;
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(BodyMesh);
			BodyMesh = null;

			RenderUtil.Dispose(HeadMeshes);
			HeadMeshes = null;
		}

		public void MakeCurrentFrame(double time)
		{
			if (BodyMesh != null)
			{
				MakeMeshFrame(BodyMesh, time);
				BodyMesh.VertexBuffer.SetData(BodyMesh.Vertices);
			}

			if (HeadMesh != null)
			{
				MakeMeshFrame(HeadMesh, time);
				HeadMesh.VertexBuffer.SetData(HeadMesh.Vertices);
			}
		}

		private void UpdateCurrentHeadMesh(int textureIndex)
		{
			if (HeadMeshes.Length == 0)
			{
				HeadMesh = null;
				return;
			}

			HeadMesh = textureIndex < HeadMeshes.Length ? HeadMeshes[textureIndex] : HeadMeshes[0];
		}

		private void MakeMeshFrame(MobMesh mesh, double time)
		{
			foreach (MeshPiece piece in mesh.Pieces)
			{
				for (int i = piece.VertexIndex; i < piece.VertexIndex + piece.VertexCount; i++)
				{
					if (piece.SkelPieceIndex >= CurrentAnimation.Skeleton.Length)
						return;

					SkeletonPiece skelPiece = CurrentAnimation.Skeleton[piece.SkelPieceIndex];

					if (skelPiece == null)
						continue;

					var originalVert = mesh.Frames[0].Vertices[i].Position;
					var finalVert = Vector3.Transform(originalVert, skelPiece.Frames[_animationFrameIndex].Transform);

					if (_previousAnimationIndex > -1 && _previousAnimationFrameIndex > -1)
					{
						var prevPiece = Animations[_previousAnimationIndex].Skeleton[piece.SkelPieceIndex];

						if (prevPiece != null && prevPiece.Frames[_previousAnimationFrameIndex] != null)
						{
							var prevFrame = prevPiece.Frames[_previousAnimationFrameIndex];

							Vector3 startVert = Vector3.Transform(originalVert, prevFrame.Transform);
							float lerpAmount = (float)((time - _frameStartTime) / AnimationFrameLength);

							if (lerpAmount < 0) lerpAmount = 0;
							if (lerpAmount > 1) lerpAmount = 1;

							finalVert = Vector3.Lerp(startVert, finalVert, lerpAmount);
						}
					}

					mesh.Vertices[i].Position = finalVert;
				}
			}
		}
	}
}
