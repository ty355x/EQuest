using System;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestServer.Repositories;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Login
{
	public class AccountLogin
	{
		private LoginServer _loginServer;

		public AccountLogin(LoginServer loginServer)
		{
			_loginServer = loginServer;
		}
		
		public void Login(Client client, string email, string password)
		{
			if (client.State != ClientState.PendingLogin)
			{
				Log.Info("Login Server - Got unexpected login request from " + client.ClientIP);
				_loginServer.DisconnectClient(client);
			}

			DbAccount account = null;

			using (var accountRepository = new AccountRepository())
				account = accountRepository.FindByEmail(email);
			
			if (account == null)
			{
				Log.Info("Login Server - No account found for " + email);
				_loginServer.DisconnectClient(client);
				return;
			}

			if (!LoginUtil.VerifyPassword(password, account.password))
			{
				Log.Info("Login Server - Invalid password for " + email);
				_loginServer.DisconnectClient(client);
				return;
			}

			DbCharacter character = null;

			using (var characterRepository = new CharacterRepository())
				character = characterRepository.FindForAccount(account.id);

			if (character == null)
			{
				Log.Info("Login Server - No character found for " + account.email);
				_loginServer.DisconnectClient(client);
				return;
			}

			// Update the client.
			client.AccountId = account.id;
			client.Character = character;
			client.Name = character.name;
			client.State = ClientState.PendingEnterZone;
			client.Mob = MakeMobForClient(client, character);

			_loginServer.MoveClientToZone(client, ServerGameConfig.Instance.PlayerStartZone, ServerGameConfig.Instance.PlayerStartPos,
				ServerGameConfig.Instance.PlayerStartHeading);
		}

		private Mob MakeMobForClient(Client client, DbCharacter character)
		{
			Mob mob = new Mob()
			{
				Name = character.name,
				Client = client,
				Type = MobType.Player,
				HitPoints = 200,
				MaxHitPoints = 200,
				MinDamage = 15,
				MaxDamage = 50,
				Race = character.race,
				Gender = character.gender,
				SpawnPoint = new Vector3(0, 200, 10),
			};

			mob.Position = mob.SpawnPoint;
			mob.PlayerInventory.LoadInventory();
			
			return mob;
		}
	}
}
