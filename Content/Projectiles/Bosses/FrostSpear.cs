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
		int aimTime = 75;
		int aimTimeCounter = 0;
		int lockTime = 25;
		int lockTimeCounter = 0;
		int state = 0;
		Vector2 storedDirection;
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.aiStyle = 0;
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.penetrate = 100;
			Projectile.timeLeft = 210;
			Projectile.light = 0f;
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

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
						Projectile.alpha -= Projectile.alpha / (int)remainingTicks * 3; // fade in a bit faster
					}
				}
			}
			else if (state == 1)
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.hostile = true;

				Projectile.rotation = storedDirection.ToRotation() + MathHelper.ToRadians(90f);

				lockTimeCounter++;
				if (lockTimeCounter >= lockTime)
				{
					storedDirection.Normalize();
					Projectile.velocity = storedDirection * projSpeed * -7;
					state = 2;
					Projectile.netUpdate = true;
				}
			}
			else if (state == 2)
			{
				Projectile.velocity += storedDirection * projSpeed;

				Projectile.rotation = storedDirection.ToRotation() + MathHelper.ToRadians(90f);
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			for (int i = 0; i < 9; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan);
				dust.noGravity = true;
			}
		}
	}
}