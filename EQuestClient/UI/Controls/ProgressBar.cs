using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.UI.Controls
{
	public class ProgressBar : Control
	{
		public SpriteFont TextFont { get; set; }
		public Color TextColor { get; set; }
		public Image Background { get; set; }
		public Color BackgroundTint { get; set; }

		public string Text { get; set; }
		public int Value { get; set; }
		public HAlign TextAlign { get; set; }

		public ProgressBar() 
		{
			TextFont = Viewport.DefaultTextFont;
			TextColor = Viewport.DefaultTextColor;
			TextAlign = HAlign.Left;
			Background = Viewport.DefaultProgressBackground;
			BackgroundTint = new Color(255, 255, 255, 255);
		}

		public override void Draw(Render render, GameTime time)
		{
			base.Draw(render, time);
			
			if (Background != null)
			{
				int x = Bounds.X + ParentWindow.Bounds.X;
				int y = Bounds.Y + ParentWindow.Bounds.Y;

				int displayValue = Value <= 100 ? Value : 100;

				if (displayValue > 0)
				{
					int width = (int)Math.Round(Bounds.Width * ((double)Value / 100));

					Rectangle dest = new Rectangle(
						x,
						y,
						width,
						Bounds.Height
					);

					Background.Draw(render, dest, BackgroundTint);
				}
			}

			if (Text != null && Text.Length > 0)
				DrawLabel(render);
		}

		private void DrawLabel(Render render)
		{
			int x = Bounds.X + ParentWindow.Bounds.X;
			int y = Bounds.Y + ParentWindow.Bounds.Y;

			if (TextAlign == HAlign.Center)
			{
				Vector2 textBounds = TextFont.MeasureString(Text);
				x = x + ((int)Bounds.Width / 2) - ((int)textBounds.X / 2);
			}

			render.DrawText(TextFont, Text, new Vector2(x, y), TextColor);
		}
	}
}
