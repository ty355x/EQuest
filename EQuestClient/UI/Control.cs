using System;
using Microsoft.Xna.Framework;
using EQuestRender;

namespace EQuestClient.UI
{
	/// <summary>
	/// Base class for UI controls and windows.
	/// </summary>
	public abstract class Control : UIElement
	{
		public Window ParentWindow { get; set; }
		public string Tooltip { get; set; }
		
		public Control()
		{
		}

		public virtual void Draw(Render render, GameTime time)
		{
			if (Tooltip != null)
				CheckDrawTooltip(render);
		}

		private void CheckDrawTooltip(Render render)
		{
			// Calculate the control's absolute bounds.
			var absoluteBounds = new Rectangle(Bounds.X + ParentWindow.X, Bounds.Y + ParentWindow.Y,
				Bounds.Width, Bounds.Height);

			// Get the position of the cursor.
			var mouseX = ParentWindow.ViewPort.MouseX;
			var mouseY = ParentWindow.ViewPort.MouseY;

			// Check if the cursor is within the bounds.
			if (!(mouseX >= absoluteBounds.X && 
				mouseX <= absoluteBounds.Right && 
				mouseY >= absoluteBounds.Y && 
				mouseY <= absoluteBounds.Bottom))
			{
				return;
			}

			// Calculate the tooltip bounds.
			var backgroundBounds = new Rectangle(ParentWindow.X + X, ParentWindow.Y + Y - 21, 150, 20);

			if (Viewport.TooltipBackground != null)
				Viewport.TooltipBackground.Draw(render, backgroundBounds, Viewport.ToolTipBackgroundTint);
			
			var textPos = new Vector2(backgroundBounds.X + 2, backgroundBounds.Y + 2);

			render.DrawText(Viewport.TooltipFont, Tooltip, textPos, Viewport.TooltipColor);
		}
	}
}
