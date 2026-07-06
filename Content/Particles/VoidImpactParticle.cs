using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class VoidImpactParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/VoidImpactParticle").Value;
        public override void OnSpawn()
        {
        }
        public float scaleX = 1f;
        public float scale2 = 0.4f;
        public override void AI()
        {
            base.AI();
            this.Velocity *= 0.92f;
            scaleX *= 0.86f;
            scale2 = float.Lerp(scale2, 1, 0.14f);
        }
        public override void Draw()
        {
            Main.spriteBatch.Draw(Texture, Position - Main.screenPosition, null, this.Color * Opacity, this.Rotation, getOrigin(), new Vector2(scaleX * 2, 1) * this.Scale * scale2, SpriteEffects.None, 0);
        }
    }
}
