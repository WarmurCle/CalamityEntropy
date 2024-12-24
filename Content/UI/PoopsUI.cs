using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.UI.Poops;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityEntropy.Content.UI
{
    public static class PoopsUI
    {
        public static void Draw()
        {
            Vector2 pos = new Vector2(880, 40);
            int maxShow = 9;
            Vector2 drawPos = pos;
            Texture2D frame = ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/frame").Value;
            List<Poop> poops = Main.LocalPlayer.Entropy().poops;
            for (int i = 0; i < maxShow; i++)
            {
                Main.spriteBatch.Draw(frame, drawPos, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                drawPos.X += 48;
            }

            drawPos = pos;
            for (int i = 0; i < maxShow; i++)
            {
                if (i < poops.Count)
                {
                    Texture2D tex = poops[i].getTexture();
                    Main.spriteBatch.Draw(tex, drawPos + new Vector2(24, 24), null, Color.White, 0, tex.Size() / 2, 2, SpriteEffects.None, 0);
                    drawPos.X += 48;
                }
            }
        }
    }
}
