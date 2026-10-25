using System;
using EQuestClient.Network;
using EQuestClient.UI;

namespace EQuestClient.Game
{
	public class Item : PlayerCursorItem
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int Icon { get; set; }
		public bool IsKnown { get; set; }
		private GameScene _scene;

		public Item(GameScene scene)
		{
			_scene = scene;
			this.CursorType = CursorItemType.Item;
		}

		public override Image GetCursorIcon()
		{
			return _scene.Assets.ItemIcons.GetIcon(Icon);
		}
	}
}
