using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class GlowLightParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowLight").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 60;
        }
        public int HideTime = 20;
        public bool AlphaShrink = true;
        public float scale2 = 1;
        public override void AI()
        {
            base.AI();
            if (AlphaShrink)
            {
                this.Opacity = this.Lifetime / (float)HideTime;
            }
            else
            {
                scale2 = this.Lifetime / (float)TimeLeftMax;
            }
            if (Opacity > 1)
            {
                this.Opacity = 1;
            }
            this.Velocity *= 0.96f;
        }
        public Color lightColor = Color.White * 0.2f;

        public override void Draw()
        {
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, lightColor * Opacity, 0, Texture.Size() / 2f, Scale * 0.65f * scale2, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, Color * Opacity, 0, Texture.Size() / 2f, Scale * 0.08f * scale2, SpriteEffects.None, 0);
        }
    }
}
