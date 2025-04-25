using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class CruiserWarn : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/CrLine").Value;
        public override void onSpawn()
        {
            this.timeLeft = 30;
        }
        public override Vector2 getOrigin()
        {
            return new Vector2(0, texture.Height / 2);
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 30f;
        }
    }
}
