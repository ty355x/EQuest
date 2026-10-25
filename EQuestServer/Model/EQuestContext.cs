using System;
using Microsoft.EntityFrameworkCore;

namespace EQuestServer.Repositories
{
	public class EQuestContext : DbContext
	{
		public DbSet<DbAccount> Accounts { get; set; }
		public DbSet<DbCharacter> Characters { get; set; }
		public DbSet<DbInventory> Inventory { get; set; }
		public DbSet<DBLootDropEntry> LootDropEntries { get; set; }
		public DbSet<DbLootTable> LootTables { get; set; }
		public DbSet<DbLootTableEntry> LootTableEntries { get; set; }
		public DbSet<DbItem> Items { get; set; }
		public DbSet<DbNpcType> NpcTypes { get; set; }
		public DbSet<DbSpawn2> Spawn2s { get; set; }
		public DbSet<DbSpawnEntry> SpawnEntries { get; set; }
		public DbSet<DbZone> Zones { get; set; }
		public DbSet<DbZoneLine> ZoneLines { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DBLootDropEntry>().HasKey(e => new { e.lootdrop_id, e.item_id });
			modelBuilder.Entity<DbLootTableEntry>().HasKey(e => new { e.lootdrop_id, e.loottable_id });
			modelBuilder.Entity<DbSpawnEntry>().HasKey(e => new { e.spawngroupID, e.npcID });
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var config = ServerConfig.Load();
			optionsBuilder.UseMySQL(config.DatabaseConnectionString);
		}
	}
}
