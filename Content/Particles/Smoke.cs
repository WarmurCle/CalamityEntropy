using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class Smoke : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Smoke").Value;

        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 200f;
        }
    }
}
