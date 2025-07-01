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
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / (float)HideTime;
            if (Opacity > 1)
            {
                this.Opacity = 1;
            }
        }
        public Color lightColor = Color.White * 0.8f;

        public override void Draw()
        {
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, lightColor * Opacity, 0, Texture.Size() / 2f, Scale * 0.65f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, Color * Opacity, 0, Texture.Size() / 2f, Scale * 0.08f, SpriteEffects.None, 0);
        }
    }
}
