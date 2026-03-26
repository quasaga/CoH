using Terraria;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Magic.Desert
{
	public class SandExplosion : ModProjectile
	{
		int dir = Main.rand.NextBool() ? 1 : -1;
		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1;
			Projectile.knockBack = 0f;
			Projectile.rotation = Main.rand.NextFloat(0f, 360f);
			Projectile.alpha = 0;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{	
			if (Projectile.alpha < 255)
			{
				Projectile.alpha += 3;
				Projectile.scale -= 0.007f;
				Projectile.rotation += .02f * dir;
			}
			else
			{
				Projectile.Kill();
			}
		}
	}
}