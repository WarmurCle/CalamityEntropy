using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ShellParticle : EParticle
    {
        public override Texture2D Texture => Tex;
        public Texture2D Tex = ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Shell").Value;
        public int Fade = 20;
        public float Gravity = 0.56f;
        public Vector2 Size = Vector2.One * 8;
        public override void OnSpawn()
        {
            this.Lifetime = 70;
        }
        public override void AI()
        {
            base.AI();
            Velocity.Y += Gravity;
            Rotation += Velocity.X * 0.05f;
            Velocity.X *= 0.96f;
            if (Lifetime < Fade)
                Opacity -= 1f / Fade;
        }
    }
}
