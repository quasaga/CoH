using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Enemies.Bloodmoon
{
	public class CoagultedHeart : ModProjectile
	{
		int timer = 0;
		public override void SetDefaults()
		{
			Projectile.width = 24; // The width of projectile hitbox
			Projectile.height = 24; // The height of projectile hitbox
			Projectile.aiStyle = 0; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.timeLeft = 210; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.penetrate = 100;
			Projectile.light = 0.5f; // How much light emit around the projectile
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
		}

		public override void AI() {
			timer++;
			if (timer >= 30)
			{
				Projectile.hostile = true;
			}
			Projectile.velocity *= .99f;

			Projectile.rotation += Math.Max((Projectile.velocity.Length() - 0.4f) / 10, 0.05f);

			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
				dust.velocity *= 1f;
				dust.noGravity = true;
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
			int coagultedHeartExplosion = ModContent.ProjectileType<CoagultedHeartExplosion>();
			int damage = 50;
			float knockBack = 5f;
			Vector2 spawnPos = Projectile.Center;

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Vector2.Zero, coagultedHeartExplosion, damage, knockBack);
			for (int i = 0; i < 3; i++) // Creates a splash of dust around the position the projectile dies.
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
				dust.noGravity = true;
			}
		}
	}
}