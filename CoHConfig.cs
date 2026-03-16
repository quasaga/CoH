using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace CoH
{
    public class CoHConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool ShowHealthDrainText { get; set; }
    }
}