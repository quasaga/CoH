using Terraria;
using Terraria.ModLoader;

namespace CoH.Content.Items.Placeable.Furniture
{
	public class OakPlatform : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 200;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OakPlatform>());
			Item.width = 8;
			Item.height = 10;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe(2)
				.AddIngredient<OakWood>()
				.AddTile<Tiles.Furniture.OakWorkbench>()
				.Register();
		}
	}
}