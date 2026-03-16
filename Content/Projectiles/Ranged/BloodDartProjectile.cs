using CoH.Content.Buffs;
using CoH.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Projectiles.Ranged
{
	public class BloodDartProjectile : ModProjectile
	{
		public bool IsStickingToTarget {
			get => Projectile.ai[0] == 1f;
			set => Projectile.ai[0] = value ? 1f : 0f;
		}

		// Index of the current target
		public int TargetWhoAmI {
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public float StickTimer {
			get => Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		private const int StickTime = 60 * 4;
		private const int MaxStickingJavelin = 9;
		private readonly Point[] stickingJavelins = new Point[MaxStickingJavelin];

		public override void SetStaticDefaults() {
			ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 600;
			Projectile.light = 0.05f;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;
		}

		public override void AI() {
			// Run either the Sticky AI or Normal AI
			// Separating into different methods helps keeps your AI clean
			if (IsStickingToTarget) {
				StickyAI();
			}
			else {
				NormalAI();
			}
		}

		private void NormalAI()
		{
			Projectile.velocity.Y += 0.05f;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Blood, Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 200, Scale: 1.7f);
				dust.velocity += Projectile.velocity * 0.3f;
				dust.velocity *= 0.2f;
			}
		}
		private void StickyAI() {
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			StickTimer += 1f;

			bool hitEffect = StickTimer % 30f == 0f;
			int npcTarget = TargetWhoAmI;
			if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200) { // If the index is past its limits, kill it
				Projectile.Kill();
			}
			else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) {
				Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
				Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
				if (hitEffect) { // purely visual
					Main.npc[npcTarget].HitEffect(0, 1.0);
				}
			}
			else {
				Projectile.Kill();
			}
		}

		public override void OnKill(int timeLeft) {
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			Vector2 usePos = Projectile.position;

			Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2();
			usePos += rotationVector * 16f;

			// Spawn some dusts upon javelin death
			for (int i = 0; i < 20; i++) {
				// Create a new dust
				Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Blood);
				dust.position = (dust.position + Projectile.Center) / 2f;
				dust.velocity += rotationVector * 2f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
				usePos -= rotationVector * 4f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			IsStickingToTarget = true;
			TargetWhoAmI = target.whoAmI;
			Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
			Projectile.netUpdate = true;
			Projectile.damage = 0; //debuff deals damage

			target.AddBuff(ModContent.BuffType<Bludgeoning>(), 900);

			// KillOldestJavelin will kill the oldest projectile stuck to the specified npc.
			// It only works if ai[0] is 1 when sticking and ai[1] is the target npc index, which is what IsStickingToTarget and TargetWhoAmI correspond to.
			// whatever that guy said ^^^
			Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, stickingJavelins);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// By shrinking target hitboxes by a small amount, this projectile only hits if it more directly hits the target.
			// This helps the javelin stick in a visually appealing place within the target sprite.
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8) {
				targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
			}
			
			return projHitbox.Intersects(targetHitbox);
		}
	}
}