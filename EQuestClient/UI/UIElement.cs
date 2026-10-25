using System;
using Microsoft.Xna.Framework;

namespace EQuestClient.UI
{
	public delegate void OnClick(UIElement sender, int x, int y);
	
	public class UIElement
	{
		public string ID { get; set; }
		public Rectangle Bounds { get; set; }
		public bool IsVisible { get; set; }
		public object Data { get; set; }
		public OnClick Click;

		public UIElement()
		{
			IsVisible = true;
		}

		public int X
		{
			get { return Bounds.X; }
			set { Bounds = new Rectangle(value, Bounds.Y, Bounds.Width, Bounds.Height); }
		}

		public int Y
		{
			get { return Bounds.Y; }
			set { Bounds = new Rectangle(Bounds.X, value, Bounds.Width, Bounds.Height); }
		}

		public int Width
		{
			get { return Bounds.Width; }
			set { Bounds = new Rectangle(Bounds.X, Bounds.Y, value, Bounds.Height); }
		}

		public int Height
		{
			get { return Bounds.Height; }
			set { Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, value); }
		}

		public virtual bool CheckClick(int x, int y)
		{
			if (!IsVisible)
				return false;
			
			if (!Bounds.Contains(x, y))
				return false;
			
			Click?.Invoke(this, x, y);
			return true;
		}
	}
}
