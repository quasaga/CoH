using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CoH.Content.Projectiles;
using CoH.Common.Systems;
using CoH.Content.Projectiles.Enemies;

namespace CoH.Content.NPCs.Bloodmoon.Morana
{
	public class BloodCultistA : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 32;
			NPC.damage = 50;
			NPC.defense = 35;
			NPC.lifeMax = 9000;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.friendly = true;
			NPC.HitSound = SoundID.Item171;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		float accelSpeed = 0.35f;
		float friction = 0.94f;
		bool projOffsetNext = false;
		Vector2 targetPos;
		int slot = -1;
		float offset = 450f;
		int colTimer = 180;
		int colTimerCounter = 0;

		public override void AI()
		{
			Player player = Main.player[NPC.target];

			if (!player.active || player.dead)
			{
				NPC.TargetClosest();
				player = Main.player[NPC.target];
				if (!player.active || player.dead)
				{
					NPC.velocity.Y += 2f;
					return;
				}
			}

			colTimerCounter++;
			if (colTimerCounter >= colTimer)
			{
				NPC.friendly = false;
			}

			if (player.Center.X >= NPC.Center.X)
				NPC.spriteDirection = -1;
			else
				NPC.spriteDirection = 1;

			int who = NPC.whoAmI;

			for (int i = 0; i < 2; i++)
			{
				if (BloodCultistAttackManager.attackTickReady && BloodCultistAttackManager.attackers[i] == who && Main.netMode != NetmodeID.MultiplayerClient)
				{
					int attack = BloodCultistAttackManager.assignedAttack[i];
					switch (attack)
					{
						case 0: SpawnSpirit(); break;
						case 1: MalevolentProj(); break;
						case 2: CoagShot(); break;
					}
				}

				if (BloodCultistAttackManager.attackers[i] == NPC.whoAmI)
				{
					slot = i;
					NPC.netUpdate = true;
					break;
				}
				else
				{
					slot = -1;
					NPC.netUpdate = true;
				}
			}

			if (slot != -1)
			{
				float offsetX = slot == 0 ? -offset : offset;
				targetPos = player.Center + new Vector2(offsetX, 0f);
			}
			else
			{
				targetPos = player.Center + new Vector2(0f, offset);
			}

			//Main.NewText("local ready:" + BloodCultistAttackManager.attackTickReady);

			Vector2 dir = targetPos - NPC.Center;

			if (dir != Vector2.Zero) // only normalize if non-zero
			{
				dir.Normalize();

				Vector2 velDir = (NPC.velocity != Vector2.Zero) ? Vector2.Normalize(NPC.velocity) : Vector2.Zero;
				float alignment = Vector2.Dot(velDir, dir);

				float velFactor = accelSpeed * Math.Max(alignment, 0.5f);
				NPC.velocity += dir * velFactor;
				NPC.velocity *= friction;
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
			NPC.velocity += dir * -5f;
		}

		public void MalevolentProj()
		{
			Player player = Main.player[NPC.target];
			int malevolentProjectile = ModContent.ProjectileType<ActuallyMalevolentProjectile>();
			int damage = 30;
			float knockBack = 5f;
			float radius = 250f;
			projOffsetNext = Main.rand.NextBool();

			float startAngle = projOffsetNext ? MathHelper.PiOver4 : 0f;

			for (int projAmount = 0; projAmount < 4; projAmount++)
			{
				float angle = startAngle + MathHelper.TwoPi * projAmount / 4;
				Vector2 offset = angle.ToRotationVector2() * radius;
				Vector2 spawnPos = player.Center + offset;

				Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, Vector2.Zero, malevolentProjectile, damage, knockBack, Owner: -1, NPC.target);
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
			NPC.velocity += dir * -5f;
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

			if (Main.netMode != NetmodeID.Server)
			{
				for (int i = 0; i < 9; i++)
				{
					Dust dust = Dust.NewDustDirect(
						NPC.position,
						NPC.width,
						NPC.height,
						DustID.Blood
					);

					dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
					dust.noGravity = false;
					dust.scale = 2f;
				}
			}
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
						DustID.Blood
					);

					dust.velocity *= 1.25f;
					dust.noGravity = true;
					dust.scale = 1.5f;
				}
			}
		}
	}
}