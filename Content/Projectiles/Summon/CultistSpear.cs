using Terraria;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Summon
{
    public class CultistSpear : ModProjectile
    {
        public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
			DrawOriginOffsetX = -34;
			DrawOriginOffsetY = 18;
			Projectile.aiStyle = -1;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 180;
			Projectile.ignoreWater = false; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

        public override void AI()
        {
            if (Projectile.alpha < 255)
            {
                Projectile.alpha += 7;
            }
            else
            {
                Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}