using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace CoH
{
    public class CoHConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        public bool GenerateValhallaMicroBiome { get; set; }
        [DefaultValue(true)]
        public bool GenerateDesertCivilisation { get; set; }

        
    }
}