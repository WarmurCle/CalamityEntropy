using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class RuneParticle : EParticle
    {
        public int frame = Main.rand.Next(0, 14);
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Runes/r" + frame.ToString()).Value;
        public override void OnSpawn()
        {
            this.Lifetime = 42;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / 42f;
            this.Color = Color.Lerp(new Color(110, 120, 255), Color.White, this.Opacity);
        }
    }
    public class RuneParticleHoming : EParticle
    {
        public int frame = Main.rand.Next(0, 14);
        public Entity homingTarget;
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Runes/r" + frame.ToString()).Value;
        public override void OnSpawn()
        {
            this.Lifetime = 3;
        }
        float speed = 0;
        public override void AI()
        {
            if (homingTarget == null)
            {
                Lifetime = 0;
                return;
            }
            if (speed < 10)
            {
                speed += 0.05f;
            }
            base.AI();
            if (this.Lifetime > 1)
            {
                this.Lifetime = 3;
            }
            this.Color = Color.Lerp(new Color(160, 170, 255), Color.White, (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.5f + 0.5f);

            this.Velocity *= 1 - speed * 0.08f;
            this.Velocity += (homingTarget.Center - Position).normalize() * speed * 1.4f;

            if (CEUtils.getDistance(Position, homingTarget.Center) < this.Velocity.Length() * 1.2f)
            {
                this.Lifetime = 0;
            }
        }
    }
}
