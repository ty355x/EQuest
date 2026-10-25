using System;
using Microsoft.Xna.Framework;
using EQuestRender;

namespace EQuestClient.Game
{
	public class PlayerCamera
	{
		public Vector3 Position { get; private set; }
		private Player _player;
		
		public PlayerCamera(Player player)
		{
			_player = player;
		}

		public void Update(GameTime time)
		{
			Position = new Vector3(_player.Body.Position.X, _player.Body.Position.Y, _player.Body.Position.Z + _player.Body.Height - 0.2f);
		}

		public void UpdateRenderCamera(Camera camera)
		{
			camera.World = Matrix.CreateScale(1.0f);
			camera.Position = Position;
			camera.Rotation = _player.Rotation;
			camera.Update();
		}
	}
}
