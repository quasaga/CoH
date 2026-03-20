using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CoH.Content.Projectiles.Melee;
using Terraria.Localization;
using CoH.Content.Projectiles;
using CoH.Content.Projectiles.Summon;

namespace CoH.Content.Items.Weapons.Desert
{
    public class RuggedCarpet : ModItem
    {
        int dmg = 140;
        float kb = 3f;
        float shootspd = 5.5f;
        int useTime = 45;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RuggedCarpetDebuff.TagDamage);

		public override void SetDefaults() {
			Item.DefaultToWhip(ModContent.ProjectileType<RuggedCarpetProjectile>(), dmg, kb, shootspd, useTime);
			Item.rare = ItemRarityID.Green;
		}

		public override bool MeleePrefix() {
			return true;
		}
    }

    public class RuggedCarpetDebuff : ModBuff
    {
	    public static readonly int TagDamage = 16;

		public override void SetStaticDefaults() {
			BuffID.Sets.IsATagBuff[Type] = true;
		}
	}

    public class RuggedCarpetDebuffNPC : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
			if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
				return;

			// SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
			var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
			if (npc.HasBuff<RuggedCarpetDebuff>()) {
				modifiers.FlatBonusDamage += RuggedCarpetDebuff.TagDamage * projTagMultiplier;
			}
		}
	}
}