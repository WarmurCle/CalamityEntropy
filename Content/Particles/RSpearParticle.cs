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
            this.Lifetime = 20;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 20f;
            this.Velocity *= 0.8f;
            this.Rotation = this.Velocity.ToRotation() + MathHelper.PiOver4;
        }

    }
}
