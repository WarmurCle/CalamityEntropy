
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public abstract class EParticle
    {
        public Effect useEffect = null;
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
            Dictionary<Effect, List<EParticle>> useEffectParticle = new Dictionary<Effect, List<EParticle>>();
            foreach (EParticle p in particles)
            {
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
            foreach(Effect effect in useEffectParticle.Keys)
            {
                foreach (EParticle p in useEffectParticle[effect])
                {
                    p.prepareShader();
                    effect.CurrentTechnique.Passes[0].Apply();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, (p.useAdditive ? BlendState.Additive : (p.useAlphaBlend ? BlendState.AlphaBlend : BlendState.NonPremultiplied)), SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    p.draw();
                    Main.spriteBatch.End();
                }
            }
        }
        public virtual void prepareShader()
        {

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
        public virtual void onSpawn()
        {

        }
        public static void spawnNew(EParticle particle, Vector2 pos, Vector2 vel, Color col, float scale, float a, bool glow, BlendState bs, float rotation = 0)
        {
            particle.position = pos;
            particle.velocity = vel;
            particle.color = col;
            particle.scale = scale;
            particle.glow = glow;
            particle.alpha = a;
            particle.useAlphaBlend = false;
            particle.useAdditive = false;
            particle.rotation = rotation;
            if(bs == BlendState.Additive)
            {
                particle.useAdditive = true;
            }
            else if(bs == BlendState.AlphaBlend)
            {
                particle.useAlphaBlend = true;
            }
            particle.onSpawn();
            particles.Add(particle);
        }
        public virtual Vector2 getOrigin()
        {
            return this.texture.Size() / 2;
        }
        public virtual void draw()
        {
            Color clr = this.color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.position.X / 16)), ((int)(this.position.Y / 16)), clr);
            }
            Main.spriteBatch.Draw(this.texture, this.position - Main.screenPosition, null, clr * alpha, rotation, getOrigin(), scale, SpriteEffects.None, 0);
        }
        public virtual Texture2D texture => null;
    }
}