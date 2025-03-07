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
    public class GlowSpark : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark").Value;
        public override void onSpawn()
        {
            this.timeLeft = 26;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 26f;
            this.velocity = this.velocity + Vector2.UnitY * 0.2f;
            this.rotation = this.velocity.ToRotation();
        }
    }
    public class GlowSpark2 : EParticle
    {
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/GlowSpark2").Value;
        public override void onSpawn()
        {
            this.timeLeft = 26;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 26f;
            this.velocity = this.velocity + Vector2.UnitY * 0.2f;
            this.rotation = this.velocity.ToRotation();
        }
    }
}
