using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.UI
{
	/// <summary>
	/// Manages the UI viewport, which can contain windows.
	/// </summary>
	public class Viewport : UIElement
	{
		public static int ViewPortPadding = 5;
		public static int WindowMargin = 4;
		public static int ItemMargin = 2;
		public static int DefaultTextHeight = 16;
		public static Color DefaultTextColor = Color.White;
		public static SpriteFont DefaultTextFont = null;

		public static SpriteFont TooltipFont = null;
		public static Color TooltipColor = Color.White;
		public static Image TooltipBackground;
		public static Color ToolTipBackgroundTint = Color.White;

		public static Color DefaultBackgroundTint = Color.White;
		public static Image DefaultBackground = null;
		public static Image DefaultTransBackground = null;
		public static Image DefaultProgressBackground = null;

		public int MouseX { get; private set; }
		public int MouseY { get; private set; }

		private List<Window> _windows = new List<Window>();

		public Viewport(Rectangle bounds)
		{
			this.Bounds = bounds;
		}

		public void AddWindow(Window window)
		{
			if (window.ViewPort != null)
				throw new Exception("Window has already been added to a viewport");

			window.ViewPort = this;
			_windows.Add(window);
			window.Initialize();
		}

		public void RemoveWindow(Window window)
		{
			window.ViewPort = null;
			_windows.Remove(window);
		}

		public void ClearWindows()
		{
			foreach (var window in _windows)
				RemoveWindow(window);
		}

		public void Draw(Render render, GameTime time, int mouseX, int mouseY)
		{
			this.MouseX = mouseX;
			this.MouseY = mouseY;

			foreach (var window in _windows)
			{
				if (window.IsVisible)
					window.Draw(render, time);
			}
		}

		public override bool CheckClick(int x, int y)
		{
			foreach (var window in _windows)
			{
				if (window.CheckClick(x, y))
					return true;
			}

			return false;
		}
	}
}
