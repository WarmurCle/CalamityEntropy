using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public class EModMenu : ModMenu
    {
        public int counter = 0;
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("CalamityEntropy/Extra/white");
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("CalamityEntropy/Extra/white");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/startmenu");
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("CalamityEntropy/Extra/Logo");
        public override string DisplayName => "Calamity Entropy";
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            drawColor = Color.White;
            counter++;
            logoScale = 1;
            logoRotation = 0;
            logoDrawCenter += new Vector2(0, (float)Math.Cos(counter * 0.008f) * 16 + 30);
            Texture2D txd = WallpaperHelper.getWallpaper();
            spriteBatch.Draw(txd, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            return true;
        }

        public override bool IsAvailable => false;
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<MenuBack>();
    }
}
