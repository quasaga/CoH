using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Enemies.Bloodmoon
{
	public class ActuallyMalevolentProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 76; // The width of projectile hitbox
			Projectile.height = 72; // The height of projectile hitbox
			Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.penetrate = 25; // How many monsters the projectile can penetrate.
			Projectile.timeLeft = 150; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
			Projectile.knockBack = 0f;
		}

		bool isLocked = true;
		bool setDir = true;
		Vector2 aimDir;
		int lockTime = 100;
		int lockTimeCounter = 0;
		float projSpeed = 1f;

		public override void AI() {
			int playerIndex = (int)Projectile.ai[0];
			Player player = playerIndex >= 0 && playerIndex < Main.maxPlayers ? Main.player[playerIndex] : null;

			if (setDir)
			{
				aimDir = player.Center - Projectile.Center;
				if (aimDir != Vector2.Zero)
				{
					aimDir.Normalize();
				}
				setDir = false;
			}

			Projectile.rotation = aimDir.ToRotation();

			if (isLocked)
			{	
				lockTimeCounter++;
				if (lockTimeCounter >= lockTime)
				{
					isLocked = false;
				}
			}
			else
			{
				Projectile.hostile = true;
				Projectile.velocity += aimDir * projSpeed;
				Projectile.rotation = Projectile.velocity.ToRotation();
			}
		}

		public override void OnKill(int timeLeft)
		{
			// Spawn dust here
			for (int i = 0; i < 25; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Blood, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 0, default, 1f);
				dust.velocity = -Projectile.velocity / 4;
				dust.noGravity = true;
				dust.scale = 1.5f;
			}
		}
	}
}