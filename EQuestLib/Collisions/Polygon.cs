using System;
using Microsoft.Xna.Framework;

namespace EQuestLib.Collisions
{
	public class Polygon
	{
		public Vector3 V0;
		public Vector3 V1;
		public Vector3 V2;

		public Vector3 Normal;

		public Plane Plane;
		public int Flags; // 16 = water

		public Polygon(Vector3 v0, Vector3 v1, Vector3 v2, int flags)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;

			this.Flags = flags;
			Plane = new Plane(this.V0, this.V1, this.V2);

			CalcNormal();
		}

		private void CalcNormal()
		{
			Vector3 normal = Vector3.Cross((V1 - V0), (V2 - V0));
			normal.Normalize();
			this.Normal = normal;
		}
	}
}
