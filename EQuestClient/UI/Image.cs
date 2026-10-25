using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.UI
{
	public class Image
	{
		private Texture2D _texture;
		private Rectangle _bounds;
		private bool _isCropped;

		public Image(Texture2D texture)
		{
			_texture = texture;
			_isCropped = false;
		}
		
		public Image(Texture2D texture, Rectangle bounds)
		{
			_texture = texture;
			_bounds = bounds;
			_isCropped = true;
		}

		public void Draw(Render render, int x, int y, Color tint)
		{
			if (!_isCropped)
			{
				render.DrawImage(_texture, new Vector2(x, y), tint);
			}
			else
			{
				render.DrawImageCropped(_texture, new Rectangle(x, y, _bounds.Width, _bounds.Height), _bounds, tint);
			}
		}

		public void Draw(Render render, Rectangle dest, Color tint)
		{
			if (!_isCropped)
			{
				render.DrawImage(_texture, dest, tint);
			}
			else
			{
				render.DrawImageCropped(_texture, dest, _bounds, tint);
			}
		}
	}
}
