
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public abstract class EParticle
    {
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }
        public int timeLeft = 200;
        public bool useAdditive = false;
        public bool useAlphaBlend = true;
        public float alpha = 1;
        public Color color = Color.White;
        public bool glow = true;
        public float rotation = 0;
        public float scale = 1;

        public static List<EParticle> particles = new List<EParticle>();
        public static void drawAll()
        {
            List<EParticle> additiveDraw = new List<EParticle>();
            List<EParticle> alphaBlendDraw = new List<EParticle>();
            List<EParticle> other = new List<EParticle>();
            foreach (EParticle p in particles)
            {
                if (p.useAdditive)
                {
                    additiveDraw.Add(p);
                }
                else if (p.useAlphaBlend)
                {
                    alphaBlendDraw.Add(p);
                }
                else
                {
                    other.Add(p);
                }
            }
            Main.spriteBatch.End();
            if (alphaBlendDraw.Count > 0) 
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach(EParticle p in alphaBlendDraw)
                {
                    p.draw();
                }
                Main.spriteBatch.End();
            }
            if (other.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in other)
                {
                    p.draw();
                }
                Main.spriteBatch.End();
            }
            if (additiveDraw.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in additiveDraw)
                {
                    p.draw();
                }
                Main.spriteBatch.End();
            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public static void updateAll()
        {
            foreach (EParticle particle in particles)
            {
                particle.update();
            }
            for(int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].timeLeft <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
        }
        public virtual void update()
        {
            this.position += this.velocity;
            timeLeft--;
        }

        public static void spawnNew(EParticle particle, Vector2 pos, Vector2 vel, Color col, float scale, float a, bool glow, BlendState bs)
        {
            particle.position = pos;
            particle.velocity = vel;
            particle.color = col;
            particle.scale = scale;
            particle.glow = glow;
            particle.alpha = a;
            particle.useAlphaBlend = false;
            particle.useAdditive = false;
            if(bs == BlendState.Additive)
            {
                particle.useAdditive = true;
            }
            else if(bs == BlendState.AlphaBlend)
            {
                particle.useAlphaBlend = true;
            }
            particles.Add(particle);
        }

        public virtual void draw()
        {
            Color clr = this.color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.position.X / 16)), ((int)(this.position.Y / 16)), clr);
            }
            Main.spriteBatch.Draw(this.texture, this.position - Main.screenPosition, null, clr * alpha, rotation, this.texture.Size() / 2, scale, SpriteEffects.None, 0);
        }
        public virtual Texture2D texture => null;
    }
}