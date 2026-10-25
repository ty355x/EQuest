using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.UI.Controls
{
	public class Icon : Control
	{
		public string Text { get; set; }
		public SpriteFont TextFont { get; set; }
		public Color TextColor { get; set; }
		public Image Background { get; set; }
		public Color BackgroundTint { get; set; }
		public Image Image { get; set; }
		public Color ImageTint { get; set; }

		public Icon()
		{
			TextFont = Viewport.DefaultTextFont;
			TextColor = Viewport.DefaultTextColor;
			Background = Viewport.DefaultBackground;
			BackgroundTint = Viewport.DefaultBackgroundTint;
			ImageTint = Color.White;
		}

		public override void Draw(Render render, GameTime time)
		{
			base.Draw(render, time);

			if (Background != null)
			{
				var pos = new Rectangle(ParentWindow.Bounds.X + Bounds.X, ParentWindow.Bounds.Y + Bounds.Y, Bounds.Width, Bounds.Height);
				Background.Draw(render, pos, BackgroundTint);
			}

			if (Image != null)
			{
				var pos = new Rectangle(ParentWindow.Bounds.X + Bounds.X, ParentWindow.Bounds.Y + Bounds.Y, Bounds.Width, Bounds.Height);
				Image.Draw(render, pos, ImageTint);
			}

			if (Text != null && Text.Length > 0)
			{
				var pos = new Vector2(ParentWindow.Bounds.X + Bounds.X, ParentWindow.Bounds.Y + Bounds.Y);
				render.DrawText(TextFont, Text, pos, TextColor);
			}
		}
	}
}
