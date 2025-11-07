using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Particles
{
    public class EGlowOrb : EParticle
    {
        public override Texture2D Texture => CEUtils.getExtraTex("Glow2");
        public Color CenterColor = Color.White;
        public float CenterScale = 0.36f;
        public float Slowdown = 0.92f;
        public override void AI()
        {
            base.AI();
            Velocity *= Slowdown;
        }
        public override void Draw()
        {
            float scale = Lifetime / (float)TimeLeftMax;
            
            for(int i = 0; i < 5; i++)
            {
                Main.spriteBatch.Draw(this.Texture, this.Position - Main.screenPosition, null, Color * scale, Rotation, getOrigin(), Scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(this.Texture, this.Position - Main.screenPosition, null, CenterColor * scale, Rotation, getOrigin(), Scale * CenterScale, SpriteEffects.None, 0);
            }
        }
    }
}
