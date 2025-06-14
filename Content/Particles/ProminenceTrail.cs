using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ProminenceTrail : EParticle
    {
        public List<Vector2> odp = new List<Vector2>();
        public override Texture2D Texture => CEUtils.getExtraTex("SimpleNoise");
        public override void OnSpawn()
        {
            this.Lifetime = 11;
            this.PixelShader = true;
        }
        public int maxLength = 21;
        public override void AI()
        {
            if (this.Lifetime < 10)
            {
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                }
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                }
            }
            base.AI();
        }

        public void AddPoint(Vector2 pos)
        {
            odp.Add(pos);
            if (odp.Count > maxLength)
            {
                odp.RemoveAt(0);
            }
        }
        public Color color1 = new Color(151, 0, 5);
        public Color color2 = new Color(255, 231, 66);
        public override void PreDraw()
        {
            if (odp.Count < 3)
            {
                return;
            }
            List<ColoredVertex> ve = new List<ColoredVertex>();
            Color b = this.Color * ((float)this.Lifetime / 12f);
            float width = 0;
            for (int i = 1; i < odp.Count; i++)
            {
                float c = (float)i / (float)(odp.Count - 1);
                if (c > 0.4)
                {
                    float x = (c - 0.4f) / 0.6f;
                    width = (float)Math.Sqrt(1 - x * x) * this.Scale;
                }
                else
                {
                    width = 1f * this.Scale;
                }
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 8 * width,
                      new Vector3((((float)i) / odp.Count), 1, 1),
                      b * ((odp.Count - i) / (float)odp.Count)));
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 8 * width,
                      new Vector3((((float)i) / odp.Count), 0, 1),
                      b * ((odp.Count - i) / (float)odp.Count)));
            }
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/Prominence", AssetRequestMode.ImmediateLoad).Value;

                Main.spriteBatch.EnterShaderRegion(BlendState.NonPremultiplied, shader);
                Main.instance.GraphicsDevice.Textures[1] = CEUtils.getExtraTex("colormap_fire");
                shader.Parameters["color2"].SetValue(color2.ToVector4());
                shader.Parameters["color1"].SetValue(color1.ToVector4());
                shader.Parameters["ofs"].SetValue(Main.GlobalTimeWrappedHourly * 3);
                shader.Parameters["alpha"].SetValue(float.Min(1, this.Lifetime / 10f));
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = this.Texture;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                Main.spriteBatch.ExitShaderRegion();
                Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);

            }
        }
    }
}
