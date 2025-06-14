using Microsoft.Xna.Framework.Graphics;

namespace CalamityEntropy.Content.Particles
{
    public class ImpactParticle : EParticle
    {
        public override Texture2D Texture => CEUtils.getExtraTex("Impact2");
        public override void OnSpawn()
        {
            base.OnSpawn();
            this.Lifetime = 120;
        }
        public float sadd = 0.1f;
        public override void AI()
        {
            base.AI();
            this.Scale += sadd;
            sadd *= 0.9f;
            this.Color *= 0.96f;

        }
    }
}
