using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class RuneParticle : EParticle
    {
        public int frame = Main.rand.Next(0, 14);
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Runes/r" + frame.ToString()).Value;
        public override void onSpawn()
        {
            this.timeLeft = 42;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 42f;
            this.color = Color.Lerp(new Color(110, 120, 255), Color.White, this.alpha);
        }
    }
    public class RuneParticleHoming : EParticle
    {
        public int frame = Main.rand.Next(0, 14);
        public Entity homingTarget;
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Runes/r" + frame.ToString()).Value;
        public override void onSpawn()
        {
            this.timeLeft = 3;
        }
        float speed = 0;
        public override void update()
        {
            if(homingTarget == null)
            {
                timeLeft = 0;
                return;
            }
            if(speed < 10)
            {
                speed += 0.05f;
            }
            base.update();
            if(this.timeLeft > 1)
            {
                this.timeLeft = 3;
            }
            this.color = Color.Lerp(new Color(160, 170, 255), Color.White, (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f + 0.5f);
            
            this.velocity *= 1 - speed * 0.08f;
            this.velocity += (homingTarget.Center - position).normalize() * speed * 1.4f;
            
            if(Utilities.Util.getDistance(position, homingTarget.Center) < this.velocity.Length() * 1.2f)
            {
                this.timeLeft = 0;
            }
        }
    }
}
