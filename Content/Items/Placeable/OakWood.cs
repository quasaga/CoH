using CoH.Content.Items.Placeable.Furniture;
using CoH.Content.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Items.Placeable
{
	public class OakWood : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
			ItemID.Sets.ExtractinatorMode[Type] = Item.type;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OakWood>());
			Item.width = 12;
			Item.height = 12;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe() // Add multiple recipes set to one Item.
				.AddIngredient<OakWall>(4)
				.AddTile<Tiles.Furniture.OakWorkbench>()
				.Register();
			CreateRecipe()
				.AddIngredient<OakPlatform>(2)
				.AddTile<Tiles.Furniture.OakWorkbench>()
				.Register();
		}
	}
}
