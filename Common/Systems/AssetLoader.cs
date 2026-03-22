using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;

namespace CoH.Common.Systems
{
    public class AssetLoader : ModSystem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> radiusShader = Mod.Assets.Request<Effect>("Assets/Effects/RadiusCircleEffect");
                Filters.Scene["CoH:RadiusCircle"] = new Filter(new ScreenShaderData(radiusShader, "RadiusCirclePass"), EffectPriority.High);
            }
        }
    }
}