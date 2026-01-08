using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

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
            return Color.Lerp(inColor, Color.White, opacity);
        }
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D txd = WallpaperHelper.getWallpaper();
            Vector2 scale = Vector2.One;
            scale *= Main.screenWidth / ((float)txd.Width * scale.X);
            
            if (txd.Height * scale.Y < Main.screenHeight)
            {
                scale *= Main.screenHeight / ((float)txd.Height * scale.Y);
            }
            spriteBatch.Draw(txd, Main.ScreenSize.ToVector2() / 2f, null, Color.White * opacity, 0, txd.Size() / 2f, scale, SpriteEffects.None, 0);

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
