using CoH.Backgrounds;
using CoH.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CoH.Content.Biomes
{
	public class ValhallaBiome : ModBiome
	{
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ValhallaBackgroundStyle>();

		// Select Music
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

		// Sets how the Scene Effect associated with this biome will be displayed with respect to vanilla Scene Effects. For more information see SceneEffectPriority & its values.
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

		// Populate the Bestiary Filter
		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		// Calculate when the biome is active.
		public override bool IsBiomeActive(Player player) {
			// Limit the biome height to be underground snow
			return player.ZoneSnow && (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && 
				// Check how many tiles of our biome are present, such that biome should be active
				ModContent.GetInstance<ValhallaBiomeTileCount>().exampleBlockCount >= 150;
		}
	}
}
