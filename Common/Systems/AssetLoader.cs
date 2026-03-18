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
        public static Asset<Texture2D> perlinTexture;
        public static Asset<Texture2D> veinTexture;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> arenaShader = Mod.Assets.Request<Effect>("Assets/Effects/ArenaCircleEffect");
                Filters.Scene["CoH:ArenaCircle"] = new Filter(new ScreenShaderData(arenaShader, "ArenaCirclePass"), EffectPriority.Medium);
                
                Asset<Effect> darknessShader = Mod.Assets.Request<Effect>("Assets/Effects/DarknessCircleEffect");
                Filters.Scene["CoH:DarknessCircle"] = new Filter(new ScreenShaderData(darknessShader, "DarknessCirclePass"), EffectPriority.High);

                perlinTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Perlin");
                veinTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Vein");
            }
        }
    }
}