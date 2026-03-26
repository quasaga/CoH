using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Ranged.Bloodmoon
{
	public class BloodClotExplosion : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 120;
			Projectile.light = 1f;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
			Projectile.alpha = 0;
			Projectile.rotation = Main.rand.NextFloat(-70f, 70f);
		}

		public override void AI()
		{

			if (Projectile.alpha < 255)
			{
				Projectile.alpha += 12;
				Projectile.scale += 0.15f;
			}
			else
			{
				Projectile.Kill();
			}
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
	}
}