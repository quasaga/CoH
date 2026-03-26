using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CoH.Content.Projectiles.Magic;
using CoH.Content.Projectiles.Magic.Snow;

namespace CoH.Content.Items.Weapons.Magic.Snow
{
    public class CryoSurge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 23;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 0.1f;
            Item.shoot = ModContent.ProjectileType<CryoRang>();
            Item.shootSpeed = 16; //pretty much changes how far the projectile spawns so leave at 1

            Item.mana = 5;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
