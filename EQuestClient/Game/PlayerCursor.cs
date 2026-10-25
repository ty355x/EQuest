using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using EQuestRender;

namespace EQuestClient.Game
{
	public class PlayerCursor
	{
		public List<PlayerCursorItem> Items { get; private set; }
		private GameScene _scene;

		public PlayerCursor(GameScene scene)
		{
			_scene = scene;
			Items = new List<PlayerCursorItem>();
		}

		public void DrawItem(Render render)
		{
			var icon = Items[0].GetCursorIcon();

			if (icon != null)
			{
				var pos = new Rectangle(_scene.Input.MousePosition.X, _scene.Input.MousePosition.Y, 40, 40);
				icon.Draw(render, pos.X, pos.Y, Color.White);
			}
		}

		public void Add(PlayerCursorItem item)
		{
			Items.Add(item);
		}

		public void Remove(int index)
		{
			Items.RemoveAt(index);
		}

		public void Remove(PlayerCursorItem item)
		{
			Items.Remove(item);
		}

		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
		}

		public void Clear()
		{
			Items.Clear();
		}

		public bool HasItems()
		{
			return Items.Count > 0;
		}
	}
}
