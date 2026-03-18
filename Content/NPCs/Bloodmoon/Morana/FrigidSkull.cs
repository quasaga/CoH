using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.NPCs.Bloodmoon.Morana
{
	public class FrigidSkull : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 48;
			NPC.damage = 50;
			NPC.defense = 75;
			NPC.lifeMax = 450;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0.15f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.friendly = false;
			NPC.HitSound = SoundID.Item171;
			NPC.DeathSound = SoundID.NPCDeath6;
		}

		float accelSpeed = .85f;
		float sineMultiplier = 0f;
		float sineTime = 0f;
		float sineSpeed = 1f;
		float sineMin = 0.75f;
		float sineMax = 1f;
		int lifetime = 1250;
		int lifetimeCounter = 0;

		public override void AI()
		{
			lifetimeCounter++;
			if (lifetimeCounter >= lifetime)
			{
				NPC.StrikeInstantKill();
			}
			Player player = Main.player[NPC.target];

			Vector2 dir = player.Center - NPC.Center;

			if (!player.active || player.dead)
			{
				NPC.TargetClosest();
				player = Main.player[NPC.target];
				if (!player.active || player.dead)
				{
					dir *= -1;
				}
			}

			if (dir != Vector2.Zero) // only normalize if non-zero
			{
				dir.Normalize();
				NPC.rotation = dir.ToRotation();

				Vector2 velDir = (NPC.velocity != Vector2.Zero) ? Vector2.Normalize(NPC.velocity) : Vector2.Zero;
				float alignment = Vector2.Dot(velDir, dir);

				sineTime += 1f / 60f;
				sineMultiplier = sineMin + (float)((Math.Sin(sineTime * sineSpeed * Math.PI * 2) + 1) / 2) * (sineMax - sineMin);

				float velFactor = accelSpeed * Math.Max(alignment, 0.5f);
				NPC.velocity += dir * velFactor;
				NPC.velocity *= sineMultiplier;
			}
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
						DustID.Frost
					);

					dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
					dust.noGravity = false;
					dust.scale = 2f;
				}
			}
        }
	}
}