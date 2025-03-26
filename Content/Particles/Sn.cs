using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class Sn : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Sn").Value;
        public override void onSpawn()
        {
            this.timeLeft = 12;
        }
        public override void update()
        {
            this.scale = 5.2f;
            base.update();
            this.alpha = this.timeLeft / 12f;
        }
    }
}
