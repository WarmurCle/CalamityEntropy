using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class HeavenfallStar : EParticle
    {
        public override Texture2D texture => Utilities.Util.getExtraTex("StarTexture_White");
        public Color InitialColor;
        public override void onSpawn()
        {
            InitialColor = color;
        }
        public override void update()
        {
            base.update();
            scale *= 0.92f;
            float LifetimeCompletion = 1 - ((float)timeLeft / TimeLeftMax);
            color = Color.Lerp(InitialColor, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D));
            velocity *= 0.92f;
        }
        public override void draw()
        {
            Vector2 scaled = new Vector2(0.2f, 1.6f) * scale;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, position - Main.screenPosition, null, color
                , rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scaled, 0, 0f);
            spriteBatch.Draw(texture, position - Main.screenPosition, null, color
                , rotation + MathHelper.PiOver2, texture.Size() * 0.5f, scaled * new Vector2(0.45f, 1f), 0, 0f);
        }
    }
}
