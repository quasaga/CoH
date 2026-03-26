using CoH.Content.Projectiles.Enemies.Bloodmoon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.NPCs.Bloodmoon
{
	public class BloodCultist : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 32;
			NPC.damage = 50;
			NPC.defense = 35;
			NPC.lifeMax = 12500;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0.15f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.friendly = false;
			NPC.HitSound = SoundID.Item171;
			NPC.DeathSound = SoundID.NPCDeath6;
		}

		int teleportDelay = 5 * 60;
		int teleportDelayCounter = 4 * 60;
		int attackDelay = 45; //ticks
		int attackDelayCounter = 0;
		int teleportRadius = 24 * 16;
		int minDist = 8 * 24;
		bool attackNow = false;
		int attackQueued = 0;
		float friction = 0.95f;

		public override void AI()
		{
			Player player = Main.player[NPC.target];

			if (!player.active || player.dead)
			{
				NPC.TargetClosest();
				player = Main.player[NPC.target];
				if (!player.active || player.dead)
				{
					return;
				}
			}

			if (player.Center.X >= NPC.Center.X)
				NPC.spriteDirection = -1;
			else
				NPC.spriteDirection = 1;

			NPC.velocity *= friction;

			teleportDelayCounter++;
			if (teleportDelayCounter >= teleportDelay)
			{
				teleportDelayCounter = 0;
				SpawnParticle(NPC.position);
				Teleport();
				SpawnParticle(NPC.position);
			}

			if (attackNow == true)
			{
				attackDelayCounter++;
				if (attackDelayCounter >= attackDelay)
				{
					if (attackQueued == 0)
					{
						SpawnSpirit();
						attackDelayCounter = 0;
						attackQueued = 1;
						attackNow = false;
					}
					else if (attackQueued == 1)
					{
						MalevolentProj();
						attackDelayCounter = 0;
						attackQueued = 2;
						attackNow = false;
					}
					else if (attackQueued == 2)
					{
						CoagShot();
						attackDelayCounter = 0;
						attackQueued = 0;
						attackNow = false;
					}
				}
			}
		}

		public void Teleport()
		{
			Player player = Main.player[NPC.target];

			for (int attempt = 0; attempt < 100; attempt++)
			{
				int randX = Main.rand.Next(minDist, teleportRadius);
				if (Main.rand.Next(2) == 0) randX *= -1;
				int randY = Main.rand.Next(-teleportRadius, teleportRadius);
				
				Vector2 randPos = player.Center + new Vector2(randX, randY);
				if (!Collision.SolidCollision(randPos, NPC.width, NPC.height))
				{
					Vector2 groundedPos = randPos;

					int tileX = (int)(groundedPos.X + NPC.width / 2) / 16;
					int tileY = (int)(groundedPos.Y + NPC.height) / 16;

					int scaledRadiusTiles = Math.Min(teleportRadius / 16, Main.maxTilesY - 10 - tileY);
					bool foundGround = false;

					for (int i = 0; i < scaledRadiusTiles; i++)
					{
						Tile tile = Framing.GetTileSafely(tileX, tileY);

						if (tile.HasTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
						{
							groundedPos.Y = tileY * 16 - NPC.height;
							NPC.position = groundedPos;
							attackNow = true;
							foundGround = true;
							break;
						}

						tileY++;
					}
					if (foundGround) return;
				}
			}
		}

		public void SpawnSpirit()
		{
			Player player = Main.player[NPC.target];
			Vector2 dir = player.Center - NPC.Center;
			dir.Normalize();

			Vector2 spawnPos = NPC.Center + dir * 10f;

			int bloodSpirit = ModContent.NPCType<BloodSpirit>();
			NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, bloodSpirit);
			NPC.velocity += new Vector2(dir.X * -4f, 0);
		}

		public void MalevolentProj()
		{
			Player player = Main.player[NPC.target];
			int malevolentProjectile = ModContent.ProjectileType<ActuallyMalevolentProjectile>();
			int damage = 30;
			float knockBack = 5f;
			float circleRad = 250f;

			for (int projAmount = 2; projAmount >= 1; projAmount--)
			{
				Vector2 dir = Main.rand.NextVector2CircularEdge(circleRad, circleRad);

				Vector2 spawnPos = player.Center + dir;

				Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, Vector2.Zero, malevolentProjectile, damage, knockBack);
				SpawnParticle(spawnPos);
			}
		}

		public void CoagShot()
		{
			Player player = Main.player[NPC.target];
			int coagShot = ModContent.ProjectileType<CoagultedHeart>();
			int damage = 30;
			float knockBack = 5f;

			Vector2 dir = player.Center - NPC.Center;
			float velocity = dir.Length() / 92;
			if (dir != Vector2.Zero)
			{
				dir.Normalize();
			}

			Vector2 spawnPos = NPC.Center + dir * 10f; //magic number is a little offset along dir

			Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, dir * velocity, coagShot, damage, knockBack);
			NPC.velocity += new Vector2(dir.X * -4f, 0);
			SpawnParticle(spawnPos);
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			//EoL scaling
			NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment * 0.6f);

			NPC.damage = (int)(NPC.damage * 0.8f);
		}

        public override void OnKill()
        {
            base.OnKill();

			SpawnParticle(NPC.position);
        }

		public void SpawnParticle(Vector2 spawnPos)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				for (int i = 0; i < 12; i++)
				{
					Dust dust = Dust.NewDustDirect(
						spawnPos,
						NPC.width,
						NPC.height,
						DustID.Ice_Red
					);

					dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
					dust.noGravity = false;
					dust.scale = 1.2f;
				}
			}
		}
	}
}