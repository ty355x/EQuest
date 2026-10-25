using System;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestServer.Zones.Combat;
using EQuestServer.Zones.MobAI;
using EQuestServer.Zones.Spawns;

namespace EQuestServer.Zones.Mobs
{
	public class Mob : ICloneable
	{
		private const int ThinkInterval = 50;

		public int Id = -1;
		public Zone Zone { get; set; }
		public Spawn Spawn { get; set; }
		public string Name { get; set; }
		public MobType Type = MobType.Monster;
		public int TextureIndex { get; set; }
		public float Size { get; set; }
		public MobLoot Loot { get; private set; }
		public PlayerInventory PlayerInventory { get; set; }

		public bool IsDisabled = false;

		public int Race = 0;
		public int Gender = 0;
		
		public Vector3 Position = Vector3.Zero;
		public float Heading;
		public float MoveSpeed = 0;

		public int MinDamage = 1;
		public int MaxDamage = 5;
		public int HitPoints;
		public int MaxHitPoints;
		public int HPRegen = 1;
		public float OOCRegenMultiplier = 3;
		public bool StatsChanged = false;
				
		public Mob Target;
		public Vector3 LastTargetPosition = Vector3.Zero;

		public bool IsAttacking = false;
		public int CombatDelay = 30;
		public long NextAttack = -1;
		public float AttackRadius = 30;

		public Vector3 SpawnPoint = Vector3.Zero;
		public long RespawnTime = -1;
		public long RespawnDelay = 60;

		public Client Client = null;
						
		public int FactionId = 0;
		public bool IsAggro = false;

		public float CanHitRadius = 30;
		public float AggroRadius = 50;

		public float WalkSpeed = 10;
		public float RunSpeed = 65;
		public bool Wander = false;

		public string AggroMessage = null;
		private long _nextUpdate = 0;

		private AIBase _ai;
		private MobCombat _combat;
		private MobMovement _movement;
		
		public event EventHandler<DeadMobEventArgs> Dead;

		public MobCombat Combat { get { return _combat; } }
		public bool IsClient { get { return Type == MobType.Player; } }
		public MobMovement Movement { get { return _movement; } }

		public string Identity { get { return Id + " " + Name; } }

		public int HitpointPercent
		{
			get
			{
				if (HitPoints == 0 || MaxHitPoints == 0)
					return 0;

				int hp = (int)Math.Round(100.0 / ((double)MaxHitPoints / HitPoints));
				return hp < 0 ? 0 : hp;
			}
		}

		public Mob()
		{
			InitMob();
		}

		public Mob(Zone zone)
		{
			this.Zone = zone;
			InitMob();
		}

		public void CheckAssist(long time, Mob attacked)
		{
			if (!IsDisabled && !IsAttacking && FactionId != 0 && attacked.FactionId == FactionId && attacked.Target != null &&
				CalcDistance(attacked) <= AggroRadius)
			{
				OnAttacked(time, attacked.Target);
			}
		}

		public float CalcDistance(Mob other)
		{
			return (other.Position - Position).Length();
		}

		public void SetAI(AIBase ai)
		{
			if (_ai != null && _ai.Type == ai.Type)
				return;

			_ai = ai;
		}

		public void SetDefaultAI()
		{
			if (IsClient)
				_ai = new ClientAI(this, Zone);
			else
				_ai = new IdleAI(this, Zone);
		}

		public void ResetMob()
		{
			Target = null;
			IsDisabled = false;
			IsAttacking = false;
			HitPoints = MaxHitPoints;
			MoveSpeed = 0;
			Heading = 0;
			Movement.RotateDirection = 0;
			StatsChanged = true;
		}

		public void Update(long time)
		{
			// If the mob is disabled, do nothing.
			if (IsDisabled)
				return;

			// Check if the update time has expired.
			if (time  < _nextUpdate)
				return;

			// Set the next update time.
			_nextUpdate = time + ThinkInterval;

			// Update the AI.
			_ai.Update(time);

			// If the mob is a corpse, no more action is required.
			if (Type == MobType.Corpse)
				return;
			
			// Update combat and movement.
			if (IsAttacking && Target != null)
				Combat.DoCombat(time, Target);

			if (_movement.IsMoving)
				_movement.CheckUpdateMovePosition(time);
		}

		public void DoRegen()
		{
			// Don't do regen if the mob is dead or at max hp.
			if (IsDisabled || HitPoints >= MaxHitPoints)
				return;

			// Get the base hp regen value.
			int regen = HPRegen;

			// Multiply by the OOC regen multiplier if they are out of combat.
			if (!IsAttacking)
				regen = (int)Math.Round(regen * OOCRegenMultiplier);

			// Adjust hitpoints.
			ChangeHitPoints(regen);
		}

		public void OnAttacked(long time, Mob attacker)
		{
			if (IsClient)
				return;

			// Set the combat AI if necessary.
			if (_ai.Type != MobAIType.Combat)
			{
				CombatAI combatAI = new CombatAI(this, Zone);
				SetAI(combatAI);
				combatAI.StartAttack(time, attacker);
			}

			((CombatAI)_ai).OnAttacked(time, attacker);
		}

		public void ChangeHitPoints(int delta)
		{
			ChangeHitPoints(delta, null);
		}

		public void ChangeHitPoints(int delta, Mob other)
		{
			if (delta == 0)
				return;

			HitPoints += delta;
			StatsChanged = true;

			if (HitPoints > MaxHitPoints)
				HitPoints = MaxHitPoints;

			if (HitPoints <= 0)
				HandleDeath(other);
		}

		public void RemoveTargetFromHateList(Mob target)
		{
			if (target != this.Target)
				return;

			if (IsClient && IsAttacking)
				Client.Send(CombatPacketMaker.MakeStopCombat());
			
			Target = null;
			IsAttacking = false;
			LastTargetPosition = Vector3.Zero;

			if (!IsClient)
			{
				if (_ai.Type != MobAIType.Idle)
				{
					IdleAI idleAI = new IdleAI(this, Zone);
					SetAI(idleAI);
					idleAI.ReturnToSpawn();
				}
			}
		}

		public void MoveToTarget()
		{
			if (Target == null || Target.Position == LastTargetPosition)
				return;

			// Make a vector from the mob to the target.
			Vector3 toTarget = Target.Position - Position;

			// Determine the distance to the target.
			float distance = toTarget.Length();

			// Determine the position to move to.
			toTarget.Normalize();
			Vector3 newPosition = Position + ((distance - 8) * toTarget);

			// Move to new position.
			_movement.MoveTo(newPosition);

			// Store the last position of the target.
			LastTargetPosition = Target.Position;
		}

		public float CalculateHeadingToPosition(Vector3 positionToFace)
		{
			Vector3 direction = positionToFace - Position;
			float newHeading = MathHelper.ToDegrees((float)Math.Atan2(direction.Y, direction.X));
			return newHeading;
		}

		public object Clone()
		{
			Mob mob = MemberwiseClone() as Mob;
			mob._combat = new MobCombat(mob);
			mob._movement = new MobMovement(mob);
			return mob;
		}

		public Mob MakeCorpse(int corpseId)
		{
			var corpse = new Mob(Zone);
			corpse.Id = corpseId;
			corpse.Name = Name + "'s corpse";
			corpse.Type = MobType.Corpse;
			corpse.Race = Race;
			corpse.TextureIndex = TextureIndex;
			corpse.MaxDamage = 0;
			corpse.MaxHitPoints = 0;
			corpse.Position = Position;
			corpse.Heading = Heading;
			corpse.SetAI(new CorpseAI(corpse, Zone, Zone.Server.CurrentTime));
			corpse.Loot = Loot;
			corpse.Loot.Mob = corpse;

			return corpse;
		}

		private void InitMob()
		{
			IsDisabled = false;
			Loot = new MobLoot(this);
			PlayerInventory = new PlayerInventory(this);
			_combat = new MobCombat(this);
			_movement = new MobMovement(this);
		}

		private void HandleDeath(Mob attacker)
		{
			if (IsDisabled)
				return;

			// Disable the mob and add a corpse.
			IsDisabled = true;
			Zone.RemoveTargetFromAllHateLists(this);
			Zone.AddCorpseForMob(this, attacker);

			if (IsClient)
			{
				// Reset the client and stop combat.
				ResetMob();
				Client.Send(CombatPacketMaker.MakeStopCombat());

				// Update the client's position.
				Position = SpawnPoint;
				Client.Send(PacketMaker.MakeMobMovementUpdate(this));
			}
			else
			{
				// Queue mob removal from the zone.
				Zone.Server.QueueRemoveMob(this);
			}

			Dead?.Invoke(this, new DeadMobEventArgs() { Attacker = attacker });
		}
	}
}
