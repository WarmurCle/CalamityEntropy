using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ShadeDashParticle : EParticle
    {
        public List<Vector2> odpl = new List<Vector2>();
        public List<Vector2> odpr = new List<Vector2>();
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/ShadeDashParticle").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 14;
        }
        public float c = Main.rand.NextFloat() * MathHelper.TwoPi;
        public override void AI()
        {
            if (Lifetime == TimeLeftMax)
            {
                Rotation = Velocity.ToRotation();
            }
            int ac = 1;
            if (odpl.Count == 0)
            {
                ac = 8;
                Lifetime += ac;
            }
            for (int i = 0; i < ac; i++)
            {
                base.AI();
                this.Opacity = this.Lifetime / (float)this.TimeLeftMax;
                this.Velocity = this.Rotation.ToRotationVector2() * this.Velocity.Length();
                this.Rotation = this.Rotation + (float)(Math.Sin(c)) * 0.065f;
                Velocity *= 0.98f;
                c += 0.46f;
                odpl.Insert(0, this.Position + Velocity.RotatedBy(MathHelper.PiOver2).normalize() * this.Scale * 13);
                odpr.Insert(0, this.Position - Velocity.RotatedBy(MathHelper.PiOver2).normalize() * this.Scale * 13);
                if (odpl.Count > 160)
                {
                    odpl.RemoveAt(odpl.Count - 1);
                    odpr.RemoveAt(odpr.Count - 1);
                }
            }
        }
        public int dir = Main.rand.NextBool() ? 1 : -1;

        public override void Draw()
        {
            ;
            Texture2D trail = Texture;
            List<ColoredVertex> ve = new List<ColoredVertex>();

            for (int i = 0; i < odpr.Count; i++)
            {
                Color b = new Color(220, 200, 255);
                ve.Add(new ColoredVertex(odpl[i] - Main.screenPosition,
                      new Vector3((i) / ((float)odpl.Count - 1), 1, 1),
                      b));
                ve.Add(new ColoredVertex(odpr[i] - Main.screenPosition,
                      new Vector3((i) / ((float)odpr.Count - 1), 0, 1),
                      b));
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/ShadeDashParticle", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color1"].SetValue((new Color(40, 40, 40, 255)).ToVector4());
                shader.Parameters["color2"].SetValue((new Color(0, 0, 0, 255)).ToVector4());
                shader.Parameters["alpha"].SetValue(Lifetime / (float)TimeLeftMax);
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                Main.spriteBatch.ExitShaderRegion();
            }
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
        }

    }
}
