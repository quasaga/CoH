using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CoH.Content.Buffs;
using CoH.Common.Players;
using Microsoft.Xna.Framework.Graphics;

namespace CoH.Content.Projectiles.Ranged
{
	public class ElectricSneeze : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			Projectile.knockBack = 3f;
		}

		public override void AI()
		{	
			Projectile.velocity.Y = Projectile.velocity.Y + 0.04f;
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Main.rand.NextBool(3))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);

				dust.noGravity = true;
				dust.scale = 0.95f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

			target.AddBuff(ModContent.BuffType<HighVoltage>(), 180);
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath3, Projectile.position);
		}
	}
}