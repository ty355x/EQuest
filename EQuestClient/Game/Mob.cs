using System;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestRender;

namespace EQuestClient.Game
{
	public class Mob : IDisposable
	{
		private const float MobSizeScaleRatio = 6;

		public int ID { get; private set; }
		public string Name { get; private set; }
		public MobType Type { get; private set; }

		public bool ShouldDraw { get; set; }
		public bool ShouldDrawTitle { get; set; }
		public bool Flag { get; set; }

		public MobModel Model { get; private set; }
		public MobAnimation Animation { get; private set; }
		public Vector3 Position { get; set; }
		public Vector3 FinalPosition { get; set; }

		public bool IsCurrentPlayer { get; set; }
		public bool IsMoving { get; private set; }
		public bool IsCorpse { get => Type == MobType.Corpse; }
		public bool IsVisible { get => DetermineIsVisible(); }

		public int Race { get; set; }
		public int Gender { get; set; }
		public int TextureIndex { get; private set; }
		public float MobSize { get; private set; }
		public float RaceSize { get; set; }
		public float TotalSize { get; private set; }

		public float Width { get; set; }
		public float Height { get; set; }

		public int Damage { get; set; }
		public int HitPoints { get; set; }
		public int MaxHitPoints { get; set; }
		public int HitpointPercent { get; set; }

		public float Heading { get; set; }
		public float FinalHeading { get; set; }
		public float MoveRate { get; set; }
		public float RotateRate { get; set; }
		public int RotateDirection { get; set; }
		public MobBoundingBox BoundingBox { get; set; }

		public OcclusionTracker Occlusion { get; set; }

		private MobTitle _mobTitle;

		public Mob(int id, string name, MobType type, int textureIndex, float raceSize, float mobSize, MobModel mobModel, 
			Vector3 position, int hitPoints, int maxHitPoints, int hitpointPercent, float heading, GameScene scene)
		{
			ID = id;
			Name = name;
			Type = type;
			MobSize = mobSize;
			TextureIndex = textureIndex;
			RaceSize = raceSize;
			Width = 2;
			Height = 6;
			HitPoints = hitPoints;
			MaxHitPoints = maxHitPoints;
			HitpointPercent = hitpointPercent;
			Position = this.FinalPosition = position;
			Heading = this.FinalHeading = heading;
			Model = mobModel;

			if (scene != null)
				_mobTitle = new MobTitle(this, scene.Assets.UI.MobTitleFont, scene.Assets.UI.TransFull, scene.Render);
			
			Animation = new MobAnimation(this);
			Model.SetTextureIndex(textureIndex);
			CalculateTotalSize();

			if (scene != null)
			{
				BoundingBox = new MobBoundingBox(scene.Render.Graphics);
				BoundingBox.Update(this);
			}

			PrintDebug();
		}

		public void SetNewFinalPosition(Vector3 position, float moveRate, float heading, int rotateDirection)
		{
			if (this.FinalPosition != position)
				this.FinalPosition = position;

			if (this.MoveRate != moveRate)
				MoveRate = moveRate;

			if (this.FinalHeading != heading)
				SetNewRotation(heading);
		}

		public void SetNewRotation(float newFinalPosition)
		{
			if (newFinalPosition < 0) newFinalPosition += 360;
			if (newFinalPosition > 360) newFinalPosition -= 360;

			FinalHeading = newFinalPosition;

			if (FinalHeading != Heading)
			{
				float rotateDistance = EQuestMath.FindShortestRotateDistance(Heading, FinalHeading);
				RotateDirection = rotateDistance > 0 ? 1 : -1;
				RotateRate = 200;
			}
		}

		public void Update(GameTime time)
		{
			if (Position != FinalPosition)
				UpdatePosition(time);

			if (Heading != FinalHeading)
				UpdateHeading(time);

			if (!IsCorpse)
				Animation.ShowBestMovementAnimation();
			
			Animation.Update(time);
		}

		public void Draw(Render render, GameTime time, Player player, bool drawTitle, bool drawBounds)
		{
			Vector3 translate = new Vector3(Position.X, Position.Y, Position.Z + (TotalSize * 3));

			var transform =
				Matrix.CreateScale(TotalSize) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(Heading)) *
				Matrix.CreateTranslation(translate);

			render.DrawMobModel(Model, time, transform);

			Flag = render.LastMeshHasTransparentTextures;
				
			if (player != null && drawTitle)
				_mobTitle.Draw(render, player);

			if (drawBounds)
				BoundingBox.Draw(render);
		}

		public void Dispose()
		{
			BoundingBox.Dispose();
			BoundingBox = null;

			_mobTitle.Dispose();
			_mobTitle = null;
		}

		private void CalculateTotalSize()
		{
			float normalizedMobSize = MobSize > 0 ? MobSize : MobSizeScaleRatio;
			TotalSize = RaceSize * (normalizedMobSize / MobSizeScaleRatio);
		}

		private bool DetermineIsVisible()
		{
			return Race != 127 && Race != 240;
		}

		private void UpdatePosition(GameTime time)
		{
			double distance =
				Math.Pow(FinalPosition.X - Position.X, 2) +
				Math.Pow(FinalPosition.Y - Position.Y, 2) +
				Math.Pow(FinalPosition.Z - Position.Z, 2);

			if (distance <= 0.05)
			{
				IsMoving = false;
				Position = FinalPosition;
				return;
			}

			IsMoving = true;
			double step = MoveRate * time.ElapsedGameTime.TotalSeconds;

			if (step > distance)
				step = distance;

			Vector3 startToEnd = FinalPosition - Position;
			startToEnd.Normalize();
			Position += (startToEnd * (float)step);
			BoundingBox.Update(this);
		}

		private void UpdateHeading(GameTime time)
		{
			if (RotateRate == 0) RotateRate = 250;
			if (RotateDirection == 0) RotateDirection = 1;

			float rotateDistance = EQuestMath.FindShortestRotateDistance(Heading, FinalHeading);
			RotateDirection = rotateDistance > 0 ? 1 : -1;
			float rotateDelta = (float)(RotateRate * time.ElapsedGameTime.TotalSeconds * RotateDirection);

			if (rotateDistance == 0 || Math.Abs(rotateDelta) > Math.Abs(rotateDistance))
			{
				Heading = FinalHeading;
				return;
			}

			Heading += rotateDelta;

			if (Heading < 0) Heading += 360;
			if (Heading > 360) Heading -= 360;
		}

		private void PrintDebug()
		{
			/*Log.Debug("Printing mob meshes for " + Name);
			string meshes = "";
			
			foreach (var mesh in Model.Meshes)
			{
				if (meshes.Length > 0)
					meshes += ", ";

				meshes += mesh.Name;
			}

			Log.Debug(meshes);*/
		}
	}
}
