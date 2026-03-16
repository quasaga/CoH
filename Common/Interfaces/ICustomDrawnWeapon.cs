using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CoH.Common.Interfaces
{
    public interface ICustomDrawnWeapon
    {
        bool ShouldRotate {get;}
        Vector2 GetGripPivot(Texture2D texture);
    }
}
