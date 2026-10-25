using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.UI.Controls
{
	public class Label : Control
	{
		public string Text;
		public SpriteFont TextFont;
		public Color TextColor;
		public HAlign TextAlign = HAlign.Left;

		public Label()
		{
			TextFont = Viewport.DefaultTextFont;
			TextColor = Viewport.DefaultTextColor;
		}

		public override void Draw(Render render, GameTime time)
		{
			base.Draw(render, time);

			if (Text != null && Text.Length > 0)
			{
				var pos = new Vector2(ParentWindow.Bounds.X + Bounds.X, ParentWindow.Bounds.Y + Bounds.Y);

				if (TextAlign == HAlign.Center)
				{
					Vector2 textBounds = TextFont.MeasureString(Text);
					pos.X = pos.X + ((int)Bounds.Width / 2) - ((int)textBounds.X / 2);
				}

				render.DrawText(TextFont, Text, pos, TextColor);
			}
		}
	}
}
