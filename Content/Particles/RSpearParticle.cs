using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class RedemptionSpearParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/RedemptionSpear").Value;
        public override void SetProperty()
        {
            this.timeLeft = 20;
        }
        public override void AI()
        {
            base.AI();
            this.alpha = this.timeLeft / 20f;
            this.velocity *= 0.8f;
            this.rotation = this.velocity.ToRotation() + MathHelper.PiOver4;
        }

    }
}
