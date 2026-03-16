using CoH.Content.Tiles;
using System;
using Terraria.ModLoader;

namespace CoH.Common.Systems
{
	public class ValhallaBiomeTileCount : ModSystem
	{
		public int exampleBlockCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
			exampleBlockCount = tileCounts[ModContent.TileType<OakWood>()];
		}
	}
}
