using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Books;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Cruiser
{

    public class FurnaceBlast : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
        }
        public float Scale = 0;
        public float Counter = 0;
        public override void AI()
        {
            Counter += 0.035f;
            Scale = CEUtils.Parabola(Counter, 1);
            if (Counter >= 1)
            {
                Scale = 0;
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            EffectLoader.PreparePixelShader(gd);
            Draw();
            Main.spriteBatch.End();
            EffectLoader.ApplyPixelShader(gd);
            Main.spriteBatch.begin_();
            return false;
        }
        public List<Vector2> GP(float distAdd = 0, float c = 1)
        {
            float dist = distAdd;
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i <= 60; i++)
            {
                points.Add(new Vector2(dist, 0).RotatedBy(MathHelper.ToRadians(i * 6 - 80 * c * Main.GlobalTimeWrappedHourly)));
            }
            return points;
        }
        public void Draw()
        {
            float a = Scale;
            if (a > 1)
            {
                a = 1;
            }
            a *= Projectile.Opacity;
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<Vector2> points = GP(0);
                List<Vector2> pointsOutside = GP(240 * Scale);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + points[i],
                    new Vector3((float)i / points.Count, 1, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = CEUtils.getExtraTex("AbyssalCircle3");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<Vector2> points = GP(0, -1);
                List<Vector2> pointsOutside = GP(200 * Scale, -1);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + points[i],
                          new Vector3((float)i / points.Count, 1, 1f),
                          Color.White * a));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          Color.White * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = CEUtils.getExtraTex("AbyssalCircle3");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<Vector2> points = GP(0, 0.6f);
                List<Vector2> pointsOutside = GP(240 * Scale, -1);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + points[i],
                          new Vector3((float)i / points.Count, 1, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = CEUtils.getExtraTex("AbyssalCircle4");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
    public class AzafureMagicBlast : EBookBaseProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 330;
            Projectile.height = 330;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
        }
        public float Scale = 0;
        public float Counter = 0;
        public override void ApplyHoming()
        {
            
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Counter += 0.035f;
            Scale = CEUtils.Parabola(Counter, 1) * 0.72f;
            if (Counter >= 1)
            {
                Scale = 0;
                Projectile.Kill();
            }
            if (Projectile.localAI[2] ++ == 0)
            {
                CEUtils.PlaySound("energyImpact", Main.rand.NextFloat(0.9f, 1.2f), Projectile.Center, 8, 0.6f);

                for (float i = 0; i < 1; i += 0.2f)
                    GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Orange, i) * i, 0.02f, i + 1.2f, 16));
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            EffectLoader.PreparePixelShader(gd);
            Draw();
            Main.spriteBatch.End();
            EffectLoader.ApplyPixelShader(gd);
            Main.spriteBatch.begin_();
            return false;
        }
        public List<Vector2> GP(float distAdd = 0, float c = 1)
        {
            float dist = distAdd;
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i <= 60; i++)
            {
                points.Add(new Vector2(dist, 0).RotatedBy(MathHelper.ToRadians(i * 6 - 80 * c * Main.GlobalTimeWrappedHourly)));
            }
            return points;
        }
        public void Draw()
        {
            return;
            float a = 1;
            if (a > 1)
            {
                a = 1;
            }
            a *= Projectile.Opacity;
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<Vector2> points = GP(0);
                List<Vector2> pointsOutside = GP(240 * Scale);
                int i;
                Projectile.ai[0] = 1;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + points[i],
                    new Vector3((float)i / points.Count, 1, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = CEUtils.getExtraTex("AbyssalCircle3");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<Vector2> points = GP(0, -1);
                List<Vector2> pointsOutside = GP(200 * Scale, -1);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + points[i],
                          new Vector3((float)i / points.Count, 1, 1f),
                          Color.White * a));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          Color.White * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = CEUtils.getExtraTex("AbyssalCircle3");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                List<Vector2> points = GP(0, 0.6f);
                List<Vector2> pointsOutside = GP(240 * Scale, -1);
                int i;
                for (i = 0; i < points.Count; i++)
                {
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + points[i],
                          new Vector3((float)i / points.Count, 1, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + pointsOutside[i],
                          new Vector3((float)i / points.Count, 0, 1f),
                          (Projectile.ai[0] == 1 ? Color.Red : Color.LightBlue) * a));

                }
                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = CEUtils.getExtraTex("AbyssalCircle4");
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
}