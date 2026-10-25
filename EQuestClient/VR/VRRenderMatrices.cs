using System;
using Microsoft.Xna.Framework;

namespace EQuestClient.VR
{
	public struct VRRenderMatrices
	{
		public Matrix View { get; private set; }
		public Matrix Project { get; private set; }

		public VRRenderMatrices(Matrix view, Matrix projection)
		{
			this.View = view;
			this.Project = projection;
		}
	}
}
