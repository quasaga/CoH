using CoH.Content.Items.Placeable;
using CoH.Content.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace CoH.Content.Items.Placeable
{
	public class OakWall : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 400;
		}

		public override void SetDefaults() {
			// ModContent.WallType<Walls.ExampleWall>() retrieves the id of the wall that this item should place when used.
			// DefaultToPlaceableWall handles setting various Item values that placeable wall items use.
			// Hover over DefaultToPlaceableWall in Visual Studio to read the documentation!
			Item.DefaultToPlaceableWall(ModContent.WallType<Walls.OakWall>());
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe(4)
				.AddIngredient<OakWood>()
				.AddTile<OakWorkbench>()
				.Register();
		}
	}
}
