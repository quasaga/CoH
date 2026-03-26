using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace CoH.Content.Projectiles.Melee.Bloodmoon
{
    public class CirculationProjectile : ModProjectile
    {
        int hitAmount = 0;
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = 25f; // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
			ProjectileID.Sets.YoyosMaximumRange[Type] = 450f; // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
			ProjectileID.Sets.YoyosTopSpeed[Type] = 12f; // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
		}

		public override void SetDefaults()
        {
			Projectile.width = 16;
			Projectile.height = 16;

			Projectile.aiStyle = ProjAIStyleID.Yoyo;

			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the item will not scale with attack speed.
			Projectile.penetrate = -1;
			Projectile.scale = 1.25f;
		}

        // notes for aiStyle 99:
        // localAI[0] is used for timing up to YoyosLifeTimeMultiplier
        // localAI[1] can be used freely by specific types
        // ai[0] and ai[1] usually point towards the x and y world coordinate hover point
        // ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the player is stoned/frozen, when the yoyo is too far away, or the player is no longer clicking the shoot button.
        // ai[0] being negative makes the yoyo move back towards the player
        // Any AI method can be used for dust, spawning projectiles, etc specific to your yoyo.

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            hitAmount++;
            if (hitAmount >= 6)
            {
                Projectile newProj = Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<CirculationExplosion>(),
                    Projectile.damage * 5,
                    Projectile.knockBack * 3f,
                    Projectile.owner
                );
                ((CirculationExplosion)newProj.ModProjectile).parentProjID = Projectile.whoAmI;

                hitAmount = 0;

                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }
        }

		public override void PostAI() {
			if (Main.rand.NextBool(5)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
				dust.noGravity = true;
			}
		}
    }
}