using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Melee
{
	public class CirculationExplosion : ModProjectile
	{
		public int parentProjID = -1;
		float toRotate = 0.2f;
		public override void SetDefaults()
		{
			Projectile.width = 128;
			Projectile.height = 128;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 120;
			Projectile.light = 1f;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 1;
			Projectile.alpha = 0;
		}

		public override void AI()
		{
			if (parentProjID != -1 && Main.projectile[parentProjID].active)
			{
				Projectile.Center = Main.projectile[parentProjID].Center;
			}
			else if (!Main.projectile[parentProjID].active)
			{
				Projectile.velocity = Vector2.Zero;
			}

			if (Projectile.alpha < 255)
			{
				Projectile.alpha += 5;
				Projectile.rotation += toRotate;
				toRotate *= 0.97f;
			}
			else
			{
				Projectile.Kill();
			}
		}
	}
}