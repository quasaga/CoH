using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CoH.Content.Walls
{
	public class OakWall : ModWall
	{
		public override void SetStaticDefaults() {
			Main.wallHouse[Type] = true;

			VanillaFallbackOnModDeletion = WallID.DiamondGemspark;

			AddMapEntry(new Color(150, 150, 150));
		}
	}
}