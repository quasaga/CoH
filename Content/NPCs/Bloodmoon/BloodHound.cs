using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace CoH.Content.NPCs.Bloodmoon
{
	public class BloodHound : ModNPC
	{
		public override void SetDefaults()
		{
			NPC.width = 64;
			NPC.height = 48;
			NPC.damage = 50;
			NPC.defense = 35;
			NPC.lifeMax = 400;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0.15f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.friendly = false;
			NPC.HitSound = SoundID.Item171;
			NPC.DeathSound = SoundID.NPCDeath6;
		}

		int dir = 0;
		float accelSpeed = 0.5f;
		float moveSpeed = 20f;
		float friction = 1f;
		float jumpPower = 10f;
		bool wasGround = false;
		bool isDropping = false;
		public override void AI()
		{
			Player player = Main.player[NPC.target];

			if (NPC.Center.X <= player.Center.X)
			{
				dir = 1;
			}
			else
			{
				dir = -1;
			}

			if (!player.active || player.dead)
			{
				NPC.TargetClosest();
				player = Main.player[NPC.target];
				if (!player.active || player.dead)
				{
					dir *= -1;
				}
			}

			NPC.velocity.X += dir * Math.Min(accelSpeed, moveSpeed);
			NPC.velocity.X *= friction;

			bool isGrounded = NPC.collideY;

			if ((NPC.collideY && NPC.collideX) ||
			(NPC.collideY && player.Center.Y + 4 < NPC.Center.Y && Math.Abs(player.Center.X - NPC.Center.X) <= 12f * 16) ||
			wasGround && !isGrounded && !isDropping)
			{
				NPC.velocity.Y = -jumpPower;
			}

			int npcFeetX = (int)(NPC.position.X + NPC.width / 2) / 16;
			int npcFeetY = (int)(NPC.position.Y + NPC.height) / 16;

			Tile tileBelow = Framing.GetTileSafely(npcFeetX, npcFeetY);

			if (tileBelow.HasTile && Main.tileSolidTop[tileBelow.TileType] && NPC.collideY && player.Center.Y - 4 > NPC.Center.Y && Math.Abs(player.Center.X - NPC.Center.X) <= 12f * 16)
			{
				isDropping = true;
				NPC.position.Y += .1f;
			}
			else isDropping = false;

			wasGround = isGrounded;
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