using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class ShineParticle : EParticle
    {
        public override Texture2D texture => Util.getExtraTex("Glow2");
        public Entity FollowOwner;
        public Vector2 ownerLastPos = Vector2.Zero;
        public override void update()
        {
            if (FollowOwner != null)
            {
                if (ownerLastPos != Vector2.Zero)
                {
                    position += (FollowOwner.Center - ownerLastPos);
                }
                ownerLastPos = FollowOwner.Center;
            }
            base.update();
            this.alpha = (float)(Math.Cos(((float)timeLeft / TimeLeftMax) * MathHelper.Pi - MathHelper.PiOver2));
        }
    }
}
