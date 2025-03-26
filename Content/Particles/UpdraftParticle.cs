using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class UpdraftParticle : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/UpdraftParticle").Value;
        public override void onSpawn()
        {
            this.timeLeft = 20;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 20f;
            this.velocity *= 0.84f;
        }

        public override Vector2 getOrigin()
        {
            return new Vector2(200, 64);
        }
    }
}
