using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using System;
using Terraria;
using Terraria.Graphics.Effects;

namespace CalamityEntropy.Content.Skies
{
    public class LlSky : CustomSky
    {
        private bool skyActive;
        private float opacity;
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex); 
        
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Entropy().llSky > 0;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }
        const int GWL_EXSTYLE = -20;
        const uint WS_EX_LAYERED = 0x80000;
        const uint WS_EX_TRANSPARENT = 0x20;
        const uint LWA_COLORKEY = 0x1;
        const uint LWA_ALPHA = 0x2;
        public override Color OnTileColor(Color inColor)
        {
            return inColor;
        }
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D txd = WallpaperHelper.getWallpaper();
            spriteBatch.Draw(txd, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * opacity);

        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.Entropy().llSky <= 0 || Main.gameMenu)
                skyActive = false;
            if (skyActive && opacity < 1f)
                opacity += 0.025f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.025f;
            Opacity = opacity;
        }

        public override float GetCloudAlpha()
        {
            return 1 - opacity;
        }
    }
}
