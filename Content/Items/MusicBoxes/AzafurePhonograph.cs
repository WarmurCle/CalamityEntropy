using CalamityEntropy.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.MusicBoxes
{
    public class AzafurePhonograph : MusicBox
    {
        public override string MusicFile => "Assets/Sounds/Music/HellBlazenRobotics";
        public override int MusicBoxTile => ModContent.TileType<AzafurePhonographTile>();
    }
    public class AzafurePhonographTile : MusicBoxTile
    {
        public Texture2D tex => CEUtils.RequestTex("CalamityEntropy/Content/Items/MusicBoxes/AzafurePhonographTileReal");
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile t = Main.tile[i, j];
            if (t.TileFrameX % 36 == 0 && t.TileFrameY % 36 == 0)
            {
                Vector2 scaling = new Vector2(1 + (float)Math.Sin(Main.GameUpdateCount * 0.06f) * 0.05f, 1 + (float)Math.Sin(Main.GameUpdateCount * 0.06f + MathHelper.Pi) * 0.05f);
                Rectangle rect = t.TileFrameX == 0 ? new Rectangle(0, 0, 56, 46) : new Rectangle(60, 0, 60, 46);
                Main.spriteBatch.Draw(tex, new Vector2(i * 16 + 16 * 13, j * 16 + 16 * 14) - Main.screenPosition, rect, Lighting.GetColor(new Point(i, j)), 0, new Vector2(30, 46), t.TileFrameX == 0 ? Vector2.One : scaling, SpriteEffects.None, 0);
            }
            return false;
        }
        public override bool PreDrawPlacementPreview(int i, int j, SpriteBatch spriteBatch, ref Rectangle frame, ref Vector2 position, ref Color color, bool validPlacement, ref SpriteEffects spriteEffects)
        {
            if (frame.X == 0 && frame.Y == 0)
            {
                Main.spriteBatch.Draw(tex, new Vector2(i * 16 + 16 * 13, j * 16 + 16 * 14) - Main.screenPosition, new Rectangle(0, 0, 60, 46), color, 0, new Vector2(30, 46), 1, spriteEffects, 0);
            }
            return false;
        }
    }
}
