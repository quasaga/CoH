using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.NPCs.Bloodmoon.Morana
{
	public class OccultSkull : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 48;
			NPC.height = 48;
			NPC.damage = 50;
			NPC.defense = 15;
			NPC.lifeMax = 750;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0.4f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.friendly = false;
			NPC.HitSound = SoundID.Item171;
			NPC.DeathSound = SoundID.NPCDeath6;
		}

		float accelSpeed = 0.45f;
		float friction = 0.97f;
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
			dir.Normalize();
			NPC.rotation = dir.ToRotation();

			if (!player.active || player.dead)
			{
				NPC.TargetClosest();
				player = Main.player[NPC.target];
				if (!player.active || player.dead)
				{
					dir *= -1;
				}
			}

			Vector2 velDir = (NPC.velocity != Vector2.Zero) ? Vector2.Normalize(NPC.velocity) : Vector2.Zero;
			float alignment = Vector2.Dot(velDir, dir);

			float velFactor = accelSpeed * Math.Max(alignment, 0.5f);
			NPC.velocity += dir * velFactor;
			NPC.velocity *= friction;
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
	}
}