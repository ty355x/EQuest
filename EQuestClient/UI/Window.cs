using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.UI
{
	/// <summary>
	/// Base class for UI windows.
	/// </summary>
	public class Window : UIElement
	{
		public Image Background { get; set; }
		public Color BackgroundTint { get; set; }
		public Viewport ViewPort { get; set; }
		public bool Transparent { get; set; }

		protected List<Control> _children = new List<Control>();
		
		public Window()
		{
			Background = Viewport.DefaultBackground;
			BackgroundTint = Viewport.DefaultBackgroundTint;
		}

		public virtual void Initialize()
		{
		}

		public virtual void Draw(Render render, GameTime time)
		{
			if (Background != null)
				Background.Draw(render, Bounds, BackgroundTint);
		
			foreach (var child in _children)
			{
				if (child.IsVisible)
					child.Draw(render, time);
			}
		}

		public void AddChild(Control child)
		{
			if (child.ParentWindow != null)
				throw new Exception("Child control already belongs to a window");
			
			child.ParentWindow = this;
			_children.Add(child);
		}

		public void RemoveChild(Control child)
		{
			child.ParentWindow = null;
			_children.Remove(child);
		}

		public void ClearChilden()
		{
			foreach (var child in _children)
				child.ParentWindow = null;
			
			_children.Clear();
		}

		public override bool CheckClick(int x, int y)
		{
			if (!IsVisible)
				return false;
			
			if (!Bounds.Contains(x, y))
				return false;

			int windowx = x - Bounds.X;
			int windowy = y - Bounds.Y;
			
			foreach (var child in _children)
			{
				if (child.CheckClick(windowx, windowy))
					return true;
			}

			Click?.Invoke(this, x, y);
			return true;
		}
	}
}
