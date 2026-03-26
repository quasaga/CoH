using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CoH.Content.NPCs.Bloodmoon.Morana;

namespace CoH.Content.Projectiles.Bosses.Morana
{
	public class FrostSpear : ModProjectile
	{
		float projSpeed = 3f;
		int aimTime = 70;
		int aimTimeCounter = 0;
		int state = 0;
		Vector2 storedDirection;
		NPC owner;
		int extraTime = 0;
		bool initialized = false;
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
			Projectile.timeLeft = 600;
			Projectile.light = 1f;
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		public override void AI() {
			if (state == 0)
			{
				int playerIndex = (int)Projectile.ai[0];
				Player player = playerIndex >= 0 && playerIndex < Main.maxPlayers ? Main.player[playerIndex] : null;

				int ownerIndex = (int)Projectile.ai[1];
				owner = Main.npc[ownerIndex];

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
						Projectile.alpha -= Projectile.alpha / (int)remainingTicks;
					}
				}

				if (owner.type == ModContent.NPCType<Content.NPCs.Bloodmoon.Morana.Morana>())
				{
					int count = extraTime / 32;
					Projectile.velocity.Y = owner.velocity.Y * (1 - count * 0.1f);
					Projectile.velocity.X = owner.velocity.X * (1 - count * 0.06f);
					Projectile.hostile = true;
				}

				if (!initialized)
				{
					extraTime = (int)Projectile.ai[2];
					aimTime += extraTime;
					initialized = true;
				}
			}
			else if (state == 1)
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.hostile = true;

				storedDirection.Normalize();
				Projectile.velocity = storedDirection * projSpeed * -4;
				
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