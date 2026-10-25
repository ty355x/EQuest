using System;
using Microsoft.Xna.Framework;

namespace EQuestClient.Game
{
	public class PlayerMovementActions
	{
		public float StandHeight = 6.0f;
		public float CrouchHeight = 3.0f;
		
		public bool IsFalling { get => _isFalling; }
		public float FallVelocityCurrent { get => _fallVelocityCurrent; }

		public const float GravityVelocityInitial = -5f;
		public const float FallVelocityMax = -325.0f;
		public const float GravityAccel = -65.0f;

		public const float JumpVelocity = 22.5f;
		
		private bool _isFalling = false;
		private float _fallStartTime = -1;
		private float _fallVelocityInitial = 0;
		private float _fallVelocityCurrent = 0;
		
		private bool _isCrouching = false;

		private Player _player;

		public PlayerMovementActions(Player player)
		{
			_player = player;
		}

		public void ToggleCrouch(GameTime time)
		{
			if (!_isCrouching)
				StartCrouch(time);
			else
				EndCrouch(time);
		}

		public void StartCrouch(GameTime time)
		{
			if (_isCrouching || _isFalling)
				return;

			_isCrouching = true;
			_player.Body.Height = CrouchHeight;
		}

		public void EndCrouch(GameTime time)
		{
			if (_isCrouching)
			{
				_isCrouching = false;
				_player.Body.Height = StandHeight;
			}
		}

		public void Update(GameTime time)
		{
			if (_isFalling)
				UpdateFalling(time);
		}

		public void StartFalling(GameTime time)
		{
			if (_isFalling)
				return;

			_isFalling = true;
			_fallVelocityInitial = GravityVelocityInitial;
			_fallVelocityCurrent = GravityVelocityInitial;
			_fallStartTime = time != null ? (float)time.TotalGameTime.TotalSeconds : -1;
		}

		public void StopFalling()
		{
			if (!_isFalling)
				return;

			_isFalling = false;
			_fallVelocityInitial = 0;
			_fallVelocityCurrent = 0;
			_fallStartTime = -1;
		}

		public void Jump(GameTime time)
		{
			if (_isFalling)
				return;

			EndCrouch(time);

			_isFalling = true;
			_fallVelocityInitial = JumpVelocity;
			_fallVelocityCurrent = JumpVelocity;
			_fallStartTime = time != null ? (float)time.TotalGameTime.TotalSeconds : -1;
		}

		private void UpdateFalling(GameTime time)
		{
			if (_fallStartTime == -1)
			{
				_fallStartTime = (float)time.TotalGameTime.TotalSeconds;
				return;
			}

			if (_fallVelocityCurrent > FallVelocityMax)
			{
				float elapsedTime = (float)(time.TotalGameTime.TotalSeconds - _fallStartTime);

				_fallVelocityCurrent = _fallVelocityInitial + elapsedTime * GravityAccel;

				if (_fallVelocityCurrent < FallVelocityMax)
					_fallVelocityCurrent = FallVelocityMax;
			}
		}

	}
}
