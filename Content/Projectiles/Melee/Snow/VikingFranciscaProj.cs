using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CoH.Content.Buffs;

namespace CoH.Content.Projectiles.Melee.Snow
{
	public class VikingFranciscaProj : ModProjectile
	{
		int returnTimer = 0;
		int returnTimerEnd = 15;
		int dmg = 0;
		float vel;
		float tolerance = 80f;
		public bool IsStickingToTarget {
			get => Projectile.ai[0] == 1f;
			set => Projectile.ai[0] = value ? 1f : 0f;
		}

		public bool IsStickingToTile {
			get => Projectile.ai[2] == 1f;
			set => Projectile.ai[2] = value ? 1f : 0f;
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

		private const int StickTime = 90;
		private readonly Point[] stickingJavelins = new Point[1];

		public override void SetStaticDefaults() {
			ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 36;
			Projectile.height = 36;
            Projectile.knockBack = 4f;
			Projectile.aiStyle = 0;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.light = .2f;
            Projectile.timeLeft = 3;
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true;
			Projectile.rotation = Main.rand.NextFloat(-180, 180);
		}

        public override void OnSpawn(IEntitySource source)
        {
			dmg = Projectile.damage;
			vel = Projectile.velocity.Length();
            base.OnSpawn(source);
        }

		public override void AI()
		{
			returnTimer++;
			if (IsStickingToTarget)
			{
				StickyAI();
			}
			else if (IsStickingToTile)
			{
				TileStickyAI();
			}
			else
			{
				NormalAI();
				Projectile.damage = dmg;
			}
			Projectile.timeLeft = 3;
		}

		private void NormalAI()
		{
			Player player = Main.player[Projectile.owner];

			if (returnTimer >= returnTimerEnd)
			{
				Vector2 dir = player.Center - Projectile.Center;
				if (dir.Length() < 48f) Projectile.Kill();
				dir.Normalize();
				Projectile.velocity += dir * (returnTimer - returnTimerEnd) * 0.5f;
			}

			if (Projectile.velocity.Length() > vel)
			{
				Projectile.velocity.Normalize();
				Projectile.velocity *= vel;
			}

			Projectile.rotation += 0.3f;

			if (Main.rand.NextBool(4))
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Frost);
				dust.scale = 1f;
				dust.noGravity = false;
			}
		}

		private void StickyAI() {
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			StickTimer += 1f;

			bool hitEffect = StickTimer % 30f == 0f;
			int npcTarget = TargetWhoAmI;
			if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200) { // If the index is past its limits, kill it
				IsStickingToTarget = false;
			}
			else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) {
				Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
				Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
				if (hitEffect) { // purely visual
					Main.npc[npcTarget].HitEffect(0, 1.0);
				}
			}
			else {
				IsStickingToTarget = false;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if (Math.Abs(MathHelper.WrapAngle(Projectile.velocity.ToRotation() - Projectile.rotation + MathHelper.ToRadians(-45f))) < MathHelper.ToRadians(tolerance))
			{
				IsStickingToTarget = true;
				TargetWhoAmI = target.whoAmI;
				Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
				Projectile.netUpdate = true;
				Projectile.damage = 0; //debuff deals damage

				target.AddBuff(ModContent.BuffType<VikingsPenetration>(), StickTime);
			}
			else
			{
				Projectile.velocity *= -1f;
			}
			target.AddBuff(BuffID.Frostburn, StickTime + 300);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Math.Abs(MathHelper.WrapAngle(Projectile.velocity.ToRotation() - Projectile.rotation + MathHelper.ToRadians(-45f))) < MathHelper.ToRadians(tolerance))
			{
				IsStickingToTile = true;
				Projectile.velocity = Vector2.Zero;
				Projectile.Center += oldVelocity;
			}
			else
			{
				Projectile.velocity *= -1f;
			}
			Projectile.tileCollide = false;

			return false;
		}

		private void TileStickyAI()
		{
			StickTimer++;
			if (StickTimer >= StickTime / 3)
			{
				IsStickingToTile = false;
			}
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

	public class VikingsPenetration : ModBuff
	{
		public override void SetStaticDefaults() {
			// NPCs will automatically be immune to this buff if they are immune to BoneJavelin. SkeletronHead and SkeletronPrime are immune to BoneJavelin.
			BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BoneJavelin);
		}

		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<VikingsPenetrationGlobalNPC>().Penetrated = true;
		}
	}

	internal class VikingsPenetrationGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public bool Penetrated;

		public override void ResetEffects(NPC npc) {
			Penetrated = false;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage) {
			if (Penetrated) {
				if (npc.lifeRegen > 0) {
					npc.lifeRegen = 0;
				}
				
				if (damage < 10)
					damage = 10;

				npc.lifeRegen -= 20 * 2;
			}
		}
	}
}