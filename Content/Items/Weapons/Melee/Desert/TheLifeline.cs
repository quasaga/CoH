using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CoH.Content.Projectiles.Melee;
using CoH.Content.Projectiles.Melee.Desert;

namespace CoH.Content.Items.Weapons.Melee.Desert
{
    public class TheLifeline : ModItem
    {
        public override void SetStaticDefaults() {
			ItemID.Sets.Yoyo[Type] = true;
			ItemID.Sets.GamepadExtraRange[Type] = 15;
			ItemID.Sets.GamepadSmartQuickReach[Type] = true;
		}
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.damage = 32;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 2.5f;
            Item.crit = 12;
            Item.shootSpeed = 16;

            Item.shoot = ModContent.ProjectileType<TheLifelineProjectile>();
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(gold: 9);
            Item.rare = ItemRarityID.Lime;
        }
    }
}