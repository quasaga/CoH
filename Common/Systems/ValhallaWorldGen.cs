using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace CoH.Common.Systems
{
	public class ValhallaWorldGen : ModSystem
	{
		public static bool JustPressed(Keys key) {
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}

		public override void PostUpdateWorld() {
			if (JustPressed(Keys.D9))
				TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
		}

		private void TestMethod(int x, int y) {
			Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

			int radius = 115;
			int innerRadius = 90;
			int wallRadius = 60;

			Point point = new Point(x, y);

			WorldUtils.Gen(point, new Shapes.Slime(radius, 1.9, 1), Actions.Chain(new Modifiers.Blotches(4, 0.85), new Actions.SetTile(TileID.IceBlock, setSelfFrames: true)));
			WorldUtils.Gen(point, new Shapes.Slime(radius, 1.9, 1), Actions.Chain(new Modifiers.Blotches(4, 0.85), new Actions.PlaceWall(WallID.SnowWallUnsafe)));
			
			WorldUtils.Gen(point + new Point(0, -10), new Shapes.Slime(innerRadius, 2, 0.8), Actions.Chain(new Modifiers.Blotches(2, 0.99), new Actions.ClearTile(frameNeighbors: true)));
			WorldUtils.Gen(point + new Point(0, -13), new Shapes.Slime(wallRadius, 2.9, 0.9), Actions.Chain(new Modifiers.Blotches(2, 0.99), new Actions.ClearWall(frameNeighbors: true)));


		}
	}
}