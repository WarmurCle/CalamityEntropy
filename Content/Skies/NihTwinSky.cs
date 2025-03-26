using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace CalamityEntropy.Content.Skies
{
    public class NihTwinSky : CustomSky
    {
        private bool skyActive;
        private float opacity;


        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Entropy().NihSky > 0;
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

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Main.spriteBatch.Draw(Util.Util.pixelTex, new Rectangle(-1000, -1000, Main.screenWidth + 1000, Main.screenHeight + 1000), new Color(0, 10, 60) * 0.5f * opacity);
        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.Entropy().NihSky <= 0 || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;

            Opacity = opacity;
        }
        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.97f + 0.03f;
        }
    }
}
