using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class EchoCircle : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/HadCircle").Value;
        public override void onSpawn()
        {
            this.timeLeft = 16;
        }
        public override void update()
        {
            base.update();
            this.alpha = timeLeft / 16f;
            this.scale = timeLeft / 16f * 0.22f;
            this.velocity *= 0.96f;
            
        }
    }
}
