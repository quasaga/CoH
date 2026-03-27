using CoH.Content.Biomes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Tiles
{
	public class OakWood : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			VanillaFallbackOnModDeletion = TileID.DiamondGemspark;

			AddMapEntry(new Color(200, 200, 200));
		}
	}
}