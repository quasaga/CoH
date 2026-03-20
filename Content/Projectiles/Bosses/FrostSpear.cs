using CoH.Content.NPCs.Bloodmoon.Morana;
using FullSerializer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Bosses
{
	public class FrostSpear : ModProjectile
	{
		float projSpeed = 3f;
		int aimTime = 70;
		int aimTimeCounter = 0;
		int state = 0;
		bool initiliazed = false;
		Vector2 storedDirection;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.penetrate = 100;
			Projectile.timeLeft = 210;
			Projectile.light = 1f;
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		public override void AI() {
			int playerIndex = (int)Projectile.ai[0];
			Player player = playerIndex >= 0 && playerIndex < Main.maxPlayers ? Main.player[playerIndex] : null;

			int ownerID = (int)Projectile.ai[1];
			int ownerIndex = (int)Projectile.ai[2];

			if (state == 0)
			{
				Vector2 aimDir = player.Center - Projectile.Center;
				if (aimDir != Vector2.Zero)
				{
					aimDir.Normalize();
				}

				Projectile.rotation = aimDir.ToRotation() + MathHelper.ToRadians(90f);

				aimTimeCounter++;
				if (aimTimeCounter >= aimTime)
				{
					storedDirection = aimDir;
					state = 1;
					Projectile.netUpdate = true;
				}

				if (Projectile.velocity != Vector2.Zero)
				{
					float remainingTicks = aimTime - aimTimeCounter;
					if (remainingTicks > 0)
					{
						Projectile.velocity -= Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() / remainingTicks);
						Projectile.alpha -= Projectile.alpha / (int)remainingTicks * 2; // fade in a bit faster
					}
				}

				if (ownerID == (float)ModContent.NPCType<Morana>())
				{
					NPC owner = Main.npc[ownerIndex];
					Projectile.velocity = owner.velocity;
					if (initiliazed) return;
					initiliazed = true;
					aimTime += 30;
				}
			}
			else if (state == 1)
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.hostile = true;

				storedDirection.Normalize();
				Projectile.velocity = storedDirection * projSpeed * -7;
				
				state = 2;
				Projectile.netUpdate = true;
			}
			else if (state == 2)
			{
				Projectile.velocity += storedDirection * projSpeed;

				Projectile.rotation = storedDirection.ToRotation() + MathHelper.ToRadians(90f);
			}
		}
	}
}