using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class ATLaser : EBookBaseLaser
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public float w = 1f;
        public override void AI()
        {
            base.AI();
            w -= 0.034f;
            if (w <= 0)
            {
                Projectile.Kill();
            }
        }
        public override Color baseColor => new Color(255, 200, 60);
        public override int OnHitEffectProb => 3;
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> points = this.getSamplePoints();
            points.Insert(0, Projectile.Center - Projectile.velocity);
            if (points.Count < 2)
            {
                return false;
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            {
                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/StreakFire").Value;
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = this.color * w;
                float p = -Main.GlobalTimeWrappedHourly * 2;
                for (int i = 1; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 70 * Projectile.scale * w,
                          new Vector3(p, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 70 * Projectile.scale * w,
                          new Vector3(p, 0, 1),
                          b));
                    p += (CEUtils.getDistance(points[i], points[i - 1]) / tx.Width);
                }

                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            {
                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/StreakSolid").Value;
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = new Color(255, 255, 255) * w;
                float p = -Main.GlobalTimeWrappedHourly * 2;
                for (int i = 1; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 14 * Projectile.scale * w,
                          new Vector3(p, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 14 * Projectile.scale * w,
                          new Vector3(p, 0, 1),
                          b));
                    p += (CEUtils.getDistance(points[i], points[i - 1]) / tx.Width);
                }


                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            CEUtils.DrawGlow(points[points.Count - 1], Color.White * w, 2f * Projectile.scale * w, true);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}