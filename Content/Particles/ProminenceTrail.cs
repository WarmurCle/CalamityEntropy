using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ProminenceTrail : EParticle
    {
        public List<Vector2> odp = new List<Vector2>();
        public override Texture2D texture => Util.Util.getExtraTex("SimpleNoise");
        public override void onSpawn()
        {
            this.timeLeft = 11;
        }
        public int maxLength = 14;
        public override void update()
        {
            if(this.timeLeft < 10)
            {
                if(odp.Count > 0)
                {
                    odp.RemoveAt(0);
                }
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                }
            }
            base.update();
        }
        
        public void AddPoint(Vector2 pos) 
        {
            odp.Add(pos);
            if (odp.Count > maxLength)
            {
                odp.RemoveAt(0);
            }
        }

        public override void draw()
        {
            if (odp.Count < 3)
            {
                return;
            }
            List<Vertex> ve = new List<Vertex>();
            Color b = this.color * ((float)this.timeLeft / 12f);
            float width = 0;
            for (int i = 1; i < odp.Count; i++)
            {
                float c = (float)i / (float)(odp.Count - 1);
                if (c > 0.4)
                {
                    float x = (c - 0.4f) / 0.6f;
                    width = (float)Math.Sqrt(1 - x * x);
                }
                else
                {
                    width = 1f;
                }
                ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 8 * width * this.scale,
                      new Vector3((((float)i) / odp.Count), 1, 1),
                      b * ((odp.Count - i) / (float)odp.Count)));
                ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 8 * width * this.scale,
                      new Vector3((((float)i) / odp.Count), 0, 1),
                      b * ((odp.Count - i) / (float)odp.Count)));
            }
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/Prominence", AssetRequestMode.ImmediateLoad).Value;

                Main.spriteBatch.EnterShaderRegion(BlendState.NonPremultiplied, shader);
                Main.instance.GraphicsDevice.Textures[1] = Util.Util.getExtraTex("colormap_fire");
                shader.Parameters["color2"].SetValue(new Color(255, 231, 66).ToVector4());
                shader.Parameters["color1"].SetValue(new Color(151, 0, 5).ToVector4());
                shader.Parameters["ofs"].SetValue(Main.GlobalTimeWrappedHourly * 3);
                shader.Parameters["alpha"].SetValue(float.Min(1, this.timeLeft / 10f));
                shader.CurrentTechnique.Passes["EffectPass"].Apply();
                
                gd.Textures[0] = this.texture;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                Main.spriteBatch.ExitShaderRegion();
                Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
                
            }
        }
    }
}
