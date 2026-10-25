using System;
using Microsoft.Xna.Framework;
using EQuestClient.Network;

namespace EQuestClient.Game
{
	public class PlayerTargetting
	{
		private const float CorpseInteractDistance = 20.0f;

		private Player _player;
		private GameScene _scene;

		public PlayerTargetting(Player player, GameScene scene)
		{
			_player = player;
			_scene = scene;
		}

		public void SelectTarget(Point pos)
		{
			var ray = MakeIntersectRay(pos);

			foreach (var mob in _scene.Zone.Mobs)
			{
				if (!mob.IsVisible)
					continue;
				
				float? result = mob.BoundingBox.Bounds.Intersects(ray);

				if (result != null)
				{
					_player.Target = mob;
					return;
				}
			}

			_player.Target = null;

			if (_player.IsAttacking)
				_player.ToggleAttack();
		}

		public void Interact(Point pos)
		{
			var ray = MakeIntersectRay(pos);
			float? distance = null;
			Mob interactMob = null;

			foreach (var mob in _scene.Zone.Mobs)
			{
				distance = mob.BoundingBox.Bounds.Intersects(ray);

				if (distance != null)
				{
					interactMob = mob;
					break;
				}
			}

			if (interactMob == null || !interactMob.IsCorpse)
				return;

			if (_player.Looting.IsLooting)
				return;
			
			if (distance > CorpseInteractDistance)
			{
				_scene.UI.Write("You are too far away to loot this corpse");
				return;
			}

			_scene.Network.SendPacket(PacketMaker.MakeLootCorpse(interactMob.ID));
		}

		private Ray MakeIntersectRay(Point pos)
		{
			_player.Camera.UpdateRenderCamera(_scene.Render.Camera);
			
			Vector3 nearScreenPoint = new Vector3(pos.X, pos.Y, 0);
			Vector3 farScreenPoint = new Vector3(pos.X, pos.Y, 1);

			Vector3 nearWorldPoint = _scene.Render.Graphics.Viewport.Unproject(
				nearScreenPoint, _scene.Render.Camera.Projection, _scene.Render.Camera.View, _scene.Render.Camera.World);

			Vector3 farWorldPoint = _scene.Render.Graphics.Viewport.Unproject(
				farScreenPoint, _scene.Render.Camera.Projection, _scene.Render.Camera.View, _scene.Render.Camera.World);

			Vector3 direction = farWorldPoint - nearWorldPoint;
			direction.Normalize();

			return new Ray(nearWorldPoint, direction);
		}
	}
}
