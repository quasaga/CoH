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
        public static Asset<Texture2D> voronoiTexture;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> darknessShader = Mod.Assets.Request<Effect>("Assets/Effects/DarknessCircleEffect");
                Filters.Scene["CoH:DarknessCircle"] = new Filter(new ScreenShaderData(darknessShader, "DarknessCirclePass"), EffectPriority.High);

                Asset<Effect> arenaShader = Mod.Assets.Request<Effect>("Assets/Effects/ArenaCircleEffect");
                Filters.Scene["CoH:ArenaCircle"] = new Filter(new ScreenShaderData(arenaShader, "ArenaCirclePass"), EffectPriority.High);

                voronoiTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Voronoi512");
            }
        }
    }
}