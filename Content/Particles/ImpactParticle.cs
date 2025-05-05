using Microsoft.Xna.Framework.Graphics;

namespace CalamityEntropy.Content.Particles
{
    public class ImpactParticle : EParticle
    {
        public override Texture2D texture => Utilities.Util.getExtraTex("Impact2");
        public override void onSpawn()
        {
            base.onSpawn();
            this.timeLeft = 120;
        }
        public float sadd = 0.1f;
        public override void update()
        {
            base.update();
            this.scale += sadd;
            sadd *= 0.9f;
            this.color *= 0.96f;

        }
    }
}
