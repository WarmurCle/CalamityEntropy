using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace CalamityEntropy.Content.Skies
{
    public class LlSky : CustomSky
    {
        private bool skyActive;
        private float opacity;

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
