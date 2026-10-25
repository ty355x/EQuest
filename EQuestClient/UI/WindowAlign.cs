using System;

namespace EQuestClient.UI
{
	public static class WindowAlign
	{
		public static void Top(Window window, Viewport viewport)
		{
			window.Y = Viewport.WindowMargin;
		}
		
		public static void Right(Window window, Viewport viewport)
		{
			window.X = viewport.Width - window.Width - Viewport.WindowMargin;
		}

		public static void Bottom(Window window, Viewport viewport)
		{
			window.Y = viewport.Height - window.Height - Viewport.WindowMargin;
		}

		public static void Left(Window window, Viewport viewport)
		{
			window.X = Viewport.WindowMargin;
		}

		public static void TopLeft(Window window, Viewport viewport)
		{
			WindowAlign.Top(window, viewport);
			WindowAlign.Left(window, viewport);
		}

		public static void TopRight(Window window, Viewport viewport)
		{
			WindowAlign.Top(window, viewport);
			WindowAlign.Right(window, viewport);
		}

		public static void BottomRight(Window window, Viewport viewport)
		{
			WindowAlign.Bottom(window, viewport);
			WindowAlign.Right(window, viewport);
		}

		public static void BottomLeft(Window window, Viewport viewport)
		{
			WindowAlign.Bottom(window, viewport);
			WindowAlign.Left(window, viewport);
		}

		public static void Center(Window window, Viewport viewport)
		{
			window.X = viewport.Width / 2 - window.Width / 2;
			window.Y = viewport.Height / 2 - window.Height / 2;
		}

		public static void Center(Viewport viewport, params Window[] windows)
		{
			// Make sure we have windows to center.
			if (windows.Length == 0)
				return;

			// Calculate the total window width and maximum window height.
			int totalWidth = windows[0].Width;
			int maxHeight = windows[0].Height;

			for (int i = 1; i < windows.Length; i++)
			{
				totalWidth = windows[i].Width + Viewport.WindowMargin;
				maxHeight = Math.Max(maxHeight, windows[i].Height);
			}

			// Assign the window X positions.
			windows[0].X = viewport.Width / 2 - totalWidth / 2;

			for (int i = 1; i < windows.Length; i++)
				WindowAlign.RightOfWindow(windows[i], windows[i - 1]);

			// Assign the window Y;
			int windowTop = viewport.Height / 2 - maxHeight / 2;

			foreach (var window in windows)
				window.Y = windowTop;
		}

		public static void RightOfWindow(Window window, Window parent)
		{
			window.X = parent.X + parent.Width + Viewport.WindowMargin;
		}

		public static void BelowWindow(Window window, Window parent)
		{
			window.Y = parent.Y + parent.Height + Viewport.WindowMargin;
		}
	}
}
