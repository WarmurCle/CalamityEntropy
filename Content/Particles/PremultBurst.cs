using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class PremultBurst : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/PremultBurst").Value;
        public override void onSpawn()
        {
            this.timeLeft = 2;
        }
        public override void update()
        {
            base.update();
            this.timeLeft = 2;
            this.scale -= 0.1f;
            if(this.scale < 0)
            {
                this.timeLeft = 0;
                this.scale = 0;
            }
        }
    }
}
