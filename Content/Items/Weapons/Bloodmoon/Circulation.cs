using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using CoH.Content.Projectiles.Melee;

namespace CoH.Content.Items.Weapons.Bloodmoon
{
    public class Circulation : ModItem
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
            Item.damage = 85;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 3f;
            Item.crit = 8;
            Item.shootSpeed = 16;

            Item.shoot = ModContent.ProjectileType<CirculationProjectile>();
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Lime;
        }
    }
}