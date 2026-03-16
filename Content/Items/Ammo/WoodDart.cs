using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CoH.Content.Projectiles;
using CoH.Content.Projectiles.Ranged;

namespace CoH.Content.Items.Ammo
{
	public class WoodDart : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}

		public override void SetDefaults()
		{
			Item.width = 8;
			Item.height = 8;
			Item.damage = 9;
			Item.DamageType = DamageClass.Ranged;
			Item.knockBack = 4;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.ammo = AmmoID.Dart;
			Item.shoot = ModContent.ProjectileType<WoodDartProjectile>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(25);
			recipe.AddIngredient(ItemID.Wood, 1);
			recipe.AddTile(TileID.LivingLoom);
			recipe.Register();
		}
	}
}