using System;
using Microsoft.Xna.Framework;
using EQuestClient.Network;
using EQuestRender;
using EQuestClient.UI;
using EQuestClient.UI.Controls;

namespace EQuestClient.Game.UI
{
	public class InventoryWindow : Window
	{
		public const int DefaultWidth = 400;
		public const int DefaultHeight = 400;

		private Icon[] _inventorySlots;
		private Button _destroyButton;
		private GameScene _scene;

		public InventoryWindow(GameScene scene)
		{
			_scene = scene;
			_inventorySlots = new Icon[PlayerInventory.SlotCount];
		}

		public override void Draw(Render render, GameTime time)
		{
			Update();
			base.Draw(render, time);
		}

		public override void Initialize()
		{
			var header = new Label();
			header.TextAlign = HAlign.Center;
			header.Bounds = new Rectangle(Viewport.ItemMargin, Viewport.ItemMargin + 5, 
				Width - Viewport.ItemMargin, 10);
			header.Text = "Inventory";
			AddChild(header);

			MakeEquipmentSlots();
			MakeInventorySlots();
			
			_destroyButton = new Button();
			_destroyButton.Bounds = new Rectangle(4, Height - 24, 80, 20);
			_destroyButton.Text = "Destroy";
			_destroyButton.Click += OnDestroyButtonClick;
			AddChild(_destroyButton);
		}

		private void Update()
		{
			for (int i = 0; i < _inventorySlots.Length; i++)
			{
				var item = _scene.Player.Inventory.Items[i];

				if (item != null)
				{
					var icon = _scene.Assets.ItemIcons.GetIcon(item.Icon);
					_inventorySlots[i].Image = icon;
					_inventorySlots[i].Tooltip = item.Name;
				}
				else
				{
					_inventorySlots[i].Image = null;
					_inventorySlots[i].Tooltip = null;
				}
			}
		}

		private void MakeEquipmentSlots()
		{
			AddEquipmentSlot(0, 0, 1, 0);
			AddEquipmentSlot(1, 1, 2, 0);
			AddEquipmentSlot(2, 3, 3, 0);
			AddEquipmentSlot(3, 2, 4, 0);
			AddEquipmentSlot(4, 0, 5, 0);
			AddEquipmentSlot(5, 13, 0, 1);
			AddEquipmentSlot(6, 9, 1, 1);
			AddEquipmentSlot(7, 7, 2, 1);
			AddEquipmentSlot(8, 12, 3, 1);
			AddEquipmentSlot(9, 9, 4, 1);
			AddEquipmentSlot(10, 13, 5, 1);
			AddEquipmentSlot(11, 8, 0, 2);
			AddEquipmentSlot(12, 4, 1, 2);
			AddEquipmentSlot(13, 6, 2, 2);
			AddEquipmentSlot(14, 10, 3, 2);
			AddEquipmentSlot(15, 14, 4, 2);
			AddEquipmentSlot(16, 15, 5, 2);
			AddEquipmentSlot(17, 16, 2, 3);
			AddEquipmentSlot(18, 16, 3, 3);
			AddEquipmentSlot(19, 11, 4, 3);
			AddEquipmentSlot(20, 17, 5, 3);
		}

		private void AddEquipmentSlot(int slotIndex, int iconIndex, int x, int y)
		{
			int regionX = 20;
			int regionY = 40;

			var slot = new Icon();
			slot.Bounds = new Rectangle(x * 40 + regionX, y * 40 + regionY, 40, 40);
			slot.Data = slotIndex;
			slot.Background = _scene.Assets.UI.EquipSlots[iconIndex];
			slot.BackgroundTint = new Color(100, 100, 100, 150);
			slot.Click += OnInventorySlotIconClick;
			_inventorySlots[slotIndex] = slot;
			AddChild(slot);
		}

		private void MakeInventorySlots()
		{
			int offsetX = 179;
			int offsetY = 208;
			int slotIndex = 21;

			for (int j = 0; j < 4; j++)
			{
				for (int i = 0; i < 2; i++)
				{
					var slot = new Icon();
					slot.Bounds = new Rectangle(i * 41 + offsetX, j * 41 + offsetY, 40, 40);
					slot.Background = _scene.Assets.UI.ItemSlot;
					slot.Data = slotIndex;
					slot.Click += OnInventorySlotIconClick;
					slot.BackgroundTint = new Color(100, 100, 100, 150);
					_inventorySlots[slotIndex++] = slot;
					AddChild(slot);
				}
			}
		}

		private void OnInventorySlotIconClick(UIElement button, int x, int y)
		{
			// If the cursor has something, it must be an item.
			if (_scene.Player.Cursor.HasItems() && _scene.Player.Cursor.Items[0].CursorType != CursorItemType.Item)
				return;
			
			int slotIndex = (int)button.Data;
			_scene.Player.Inventory.PickupItem(slotIndex);
			_scene.Network.SendPacket(PacketMaker.MakePickupInventoryItem(slotIndex));
		}

		private void OnDestroyButtonClick(UIElement button, int x, int y)
		{
			// Cursor must have an item.
			if (!_scene.Player.Cursor.HasItems() || _scene.Player.Cursor.Items[0].CursorType != CursorItemType.Item)
				return;
				
			_scene.Player.Cursor.RemoveAt(0);
			_scene.Network.SendPacket(PacketMaker.MakeDestroyItem());
		}
	}
}
