using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using CoH.Content.Projectiles.Melee;

namespace CoH.Content.Items.Weapons.Snow
{
    public class VikingFrancisca : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.damage = 17;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 30;
            Item.knockBack = 8f;
            Item.shoot = ModContent.ProjectileType<VikingFranciscaProj>();
            Item.shootSpeed = 16f;

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.buyPrice(silver: 777);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}
