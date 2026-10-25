using System;
using Microsoft.Xna.Framework;

namespace EQuestLib.Movement
{
	public class MobBody
	{
		public Vector3 Position;
		public Vector3 Velocity;
		public float Width;
		public float Height;

		public MobBody(Vector3 position, Vector3 velocity, float width, float height)
		{
			this.Position = position;
			this.Velocity = velocity;
			this.Width = width;
			this.Height = height;
		}
	}
}
