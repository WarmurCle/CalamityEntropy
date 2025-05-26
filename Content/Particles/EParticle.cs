using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public abstract class EParticle
    {
        public Effect useEffect = null;
        public int TimeLeftMax = 0;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public int Lifetime = 200;
        public bool useAdditive = false;
        public bool useAlphaBlend = true;
        public float Opacity = 1;
        public Color Color = Color.White;
        public bool glow = true;
        public float Rotation = 0;
        public float Scale = 1;
        public bool PixelShader = false;

        public static List<EParticle> particles = new List<EParticle>();
        public static void DrawPixelShaderParticles()
        {
            List<EParticle> additiveDraw = new List<EParticle>();
            List<EParticle> alphaBlendDraw = new List<EParticle>();
            List<EParticle> other = new List<EParticle>();
            Dictionary<Effect, List<EParticle>> useEffectParticle = new Dictionary<Effect, List<EParticle>>();
            foreach (EParticle p in particles)
            {
                if (!p.PixelShader)
                {
                    continue;
                }
                if (p.useEffect == null)
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
                else
                {
                    if (useEffectParticle.ContainsKey(p.useEffect))
                    {
                        useEffectParticle[p.useEffect].Add(p);
                    }
                    else
                    {
                        useEffectParticle[p.useEffect] = new List<EParticle>() { p };
                    }
                }
            }
            Main.spriteBatch.End();
            if (alphaBlendDraw.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in alphaBlendDraw)
                {
                    p.PreDraw();
                }
                Main.spriteBatch.End();
            }
            if (other.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in other)
                {
                    p.PreDraw();
                }
                Main.spriteBatch.End();
            }
            if (additiveDraw.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in additiveDraw)
                {
                    p.PreDraw();
                }
                Main.spriteBatch.End();
            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public static void drawAll()
        {
            List<EParticle> additiveDraw = new List<EParticle>();
            List<EParticle> alphaBlendDraw = new List<EParticle>();
            List<EParticle> other = new List<EParticle>();
            Dictionary<Effect, List<EParticle>> useEffectParticle = new Dictionary<Effect, List<EParticle>>();
            foreach (EParticle p in particles)
            {
                if (p.PixelShader)
                {
                    continue;
                }
                if (p.useEffect == null)
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
                else
                {
                    if (useEffectParticle.ContainsKey(p.useEffect))
                    {
                        useEffectParticle[p.useEffect].Add(p);
                    }
                    else
                    {
                        useEffectParticle[p.useEffect] = new List<EParticle>() { p };
                    }
                }
            }
            Main.spriteBatch.End();
            if (alphaBlendDraw.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in alphaBlendDraw)
                {
                    p.PreDraw();
                }
                Main.spriteBatch.End();
            }
            if (other.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in other)
                {
                    p.PreDraw();
                }
                Main.spriteBatch.End();
            }
            if (additiveDraw.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (EParticle p in additiveDraw)
                {
                    p.PreDraw();
                }
                Main.spriteBatch.End();
            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public virtual void prepareShader()
        {

        }

        public static void updateAll()
        {
            foreach (EParticle particle in particles)
            {
                particle.AI();
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].Lifetime <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
        }
        public virtual void AI()
        {
            this.position += this.velocity;
            Lifetime--;
        }
        public virtual void SetProperty()
        {

        }
        public static void NewParticle(EParticle particle, Vector2 pos, Vector2 vel, Color col, float scale, float a, bool glow, BlendState bs, float rotation = 0, int lifeTime = -1)
        {
            particle.position = pos;
            particle.velocity = vel;
            particle.Color = col;
            particle.Scale = scale;
            particle.glow = glow;
            particle.Opacity = a;
            particle.useAlphaBlend = false;
            particle.useAdditive = false;
            particle.Rotation = rotation;
            if (bs == BlendState.Additive)
            {
                particle.useAdditive = true;
            }
            else if (bs == BlendState.AlphaBlend)
            {
                particle.useAlphaBlend = true;
            }
            particle.SetProperty();
            if (lifeTime > 0)
            {
                particle.Lifetime = lifeTime;
            }
            particle.TimeLeftMax = particle.Lifetime;
            particles.Add(particle);
        }
        public virtual Vector2 getOrigin()
        {
            return this.Texture.Size() / 2;
        }
        public virtual void PreDraw()
        {
            Color clr = this.Color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.position.X / 16)), ((int)(this.position.Y / 16)), clr);
            }
            if (!this.useAdditive && !this.useAlphaBlend)
            {
                clr.A = (byte)(clr.A * Opacity);
            }
            else
            {
                clr *= Opacity;
            }
            Main.spriteBatch.Draw(this.Texture, this.position - Main.screenPosition, null, clr, Rotation, getOrigin(), Scale, SpriteEffects.None, 0);
        }
        public virtual Texture2D Texture => null;
    }
}