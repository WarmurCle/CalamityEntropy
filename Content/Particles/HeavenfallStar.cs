using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class HeavenfallStar : EParticle
    {
        public override Texture2D Texture => CEUtils.getExtraTex("StarTexture_White");
        public Color InitialColor;
        public float xScale = 1;
        public override void OnSpawn()
        {
            InitialColor = Color;
        }
        public override void AI()
        {
            base.AI();
            Scale *= 0.92f;
            float LifetimeCompletion = 1 - ((float)Lifetime / TimeLeftMax);
            Color = Color.Lerp(InitialColor, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D));
            Velocity *= 0.92f;
        }
        public override void PreDraw()
        {
            Vector2 scaled = new Vector2(0.2f, 1.6f * xScale) * Scale;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(Texture, Position - Main.screenPosition, null, Color
                , Rotation + MathHelper.PiOver2, Texture.Size() * 0.5f, scaled, 0, 0f);
            spriteBatch.Draw(Texture, Position - Main.screenPosition, null, Color
                , Rotation + MathHelper.PiOver2, Texture.Size() * 0.5f, scaled * new Vector2(0.45f, 1f), 0, 0f);
        }
    }
}
