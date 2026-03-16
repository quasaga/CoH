using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CoH.Common.Players;
using CoH.Common.Interfaces;

namespace CoH.Common.DrawLayers
{
    public class CustomHeldItemLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            return player.itemAnimation > 0;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            Item held = player.HeldItem;
            if (held?.ModItem == null || held.ModItem.Mod != Mod || held.ModItem is not ICustomDrawnWeapon) return;

            Texture2D texture = Terraria.GameContent.TextureAssets.Item[held.type].Value;
            Vector2 gripPivot;

            if (held.ModItem is ICustomDrawnWeapon customWeapon)
                gripPivot = customWeapon.GetGripPivot(texture);
            else
                gripPivot = new Vector2(texture.Width / 2f, texture.Height / 2f);

            var recoilPlayer = player.GetModPlayer<ImpactPlayer>();

            // calculate rotation with recoil
            float aimRotation = (Main.MouseWorld - player.MountedCenter).ToRotation();
            int dir = player.direction == 1 ? 1 : -1;
            float recoilRotation = recoilPlayer.recoilRotation * dir;
            float totalRotation = recoilRotation + aimRotation;

            Vector2 drawPos = player.MountedCenter - Main.screenPosition;

            SpriteEffects effects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            if (effects.HasFlag(SpriteEffects.FlipVertically))
            {
                gripPivot.Y = texture.Height - gripPivot.Y;
            }
            Color lightColor = Lighting.GetColor((int)(drawPos.X + Main.screenPosition.X) / 16, (int)(drawPos.Y + Main.screenPosition.Y) / 16);

            drawInfo.DrawDataCache.Add(new DrawData(texture, drawPos, null, lightColor, totalRotation, gripPivot, 1f, effects, 0));
        }
    }
}
