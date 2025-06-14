using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class LifeLeaf : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/LifeLeaf").Value;
        public override void SetProperty()
        {
            this.Lifetime = 30;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 30f;
            this.Rotation += this.Velocity.X * 0.04f;
            this.Velocity *= 0.92f;
            this.Velocity = this.Velocity + Vector2.UnitY * 0.2f;
        }
    }
}
