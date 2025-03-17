using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class RedemptionSpearParticle : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/RedemptionSpear").Value;
        public override void onSpawn()
        {
            this.timeLeft = 20;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 20f;
            this.velocity *= 0.8f;
            this.rotation = this.velocity.ToRotation() + MathHelper.PiOver4;
        }

    }
}
