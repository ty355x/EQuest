using System;
using EQuestClient.UI;

namespace EQuestClient.Game
{
	public enum CursorItemType { Item }

	public abstract class PlayerCursorItem
	{
		public CursorItemType CursorType { get; protected set; }
		public abstract Image GetCursorIcon();
	}
}
