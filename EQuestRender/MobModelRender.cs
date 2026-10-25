using System;
using Microsoft.Xna.Framework;

namespace EQuestRender
{
	public class MobModelRender
	{
		private Render _render;
		public bool MobHasTransparentTextures;

		public MobModelRender(Render render)
		{
			_render = render;
		}
		
		public void Draw(MobModel model, GameTime time, Matrix transform)
		{
			MobHasTransparentTextures = false;
			bool bodyTransparent = false;
			bool headTransparent = false;
			
			model.MakeCurrentFrame(time.TotalGameTime.TotalMilliseconds);

			if (model.BodyMesh != null)
			{
				model.BodyMesh.Transform = transform;
				_render.DrawMesh(model.BodyMesh, model.BodyMesh.VertexBuffer, model.BodyMesh.IndexBuffer, time);
				bodyTransparent = _render.LastMeshHasTransparentTextures;
			}

			if (model.HeadMesh != null)
			{
				model.HeadMesh.Transform = transform;
				_render.DrawMesh(model.HeadMesh, model.HeadMesh.VertexBuffer, model.HeadMesh.IndexBuffer, time);
				headTransparent = _render.LastMeshHasTransparentTextures;
			}

			MobHasTransparentTextures = bodyTransparent || headTransparent;
		}
	}
}
