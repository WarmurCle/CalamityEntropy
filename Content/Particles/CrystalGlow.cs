using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class CrystalGlow : EParticle
    {
        public override Texture2D Texture => CEUtils.getExtraTex("CrystalGlow");
        public float r1 = 0;
        public float r2 = 0;
        public override void OnSpawn()
        {
            base.OnSpawn();
            r1 = CEUtils.randomRot();
            r2 = CEUtils.randomRot();
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / (float)this.TimeLeftMax;
        }
        public override void Draw()
        {
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, this.Color * Opacity, r1, Texture.Size() * 0.5f, this.Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, this.Color * Opacity, r2, Texture.Size() * 0.5f, this.Scale, SpriteEffects.None, 0);
        }
    }
}
