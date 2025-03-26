using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class LifeLeaf : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/LifeLeaf").Value;
        public override void onSpawn()
        {
            this.timeLeft = 30;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 30f;
            this.rotation += this.velocity.X * 0.04f;
            this.velocity *= 0.92f;
            this.velocity = this.velocity + Vector2.UnitY * 0.2f;
        }
    }
}
