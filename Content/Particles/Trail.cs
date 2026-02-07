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
    public class TrailParticle : EParticle
    {
        public List<Vector2> odp = new List<Vector2>();
        public bool SameAlpha = false;
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Trail").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 13;
        }
        public int maxLength = 64;
        public override void AI()
        {
            base.AI();
        }

        public void AddPoint(Vector2 pos)
        {
            odp.Insert(0, pos);
            if (odp.Count > maxLength)
            {
                odp.RemoveAt(odp.Count - 1);
            }
        }

        public override void Draw()
        {
            if (odp.Count < 3)
            {
                return;
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            List<ColoredVertex> ve = new List<ColoredVertex>();
            Color b = this.Color * ((float)this.Lifetime / 12f);
            ve.Add(new ColoredVertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 12 * this.Scale,
                      new Vector3((((float)0) / odp.Count), 1, 1),
                      b));
            ve.Add(new ColoredVertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 12 * this.Scale,
                  new Vector3((((float)0) / odp.Count), 0, 1),
                  b));
            for (int i = 1; i < odp.Count; i++)
            {
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 12 * this.Scale,
                      new Vector3((((float)i) / odp.Count), 1, 1),
                      b * (SameAlpha ? 1 : ((odp.Count - i - 1) / (float)odp.Count))));
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 12 * this.Scale,
                      new Vector3((((float)i) / odp.Count), 0, 1),
                      b * (SameAlpha ? 1 : ((odp.Count - i - 1) / (float)odp.Count))));
            }
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                gd.Textures[0] = this.Texture;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
        }
    }
    public class TrailGunShot : EParticle
    {
        internal Color ColorFunction(float completionRatio, Vector2 vertex)
        {
            completionRatio = 1 - completionRatio;
            float fadeOpacity = Math.Min(Lifetime / (float)trailLength, 1f);
            return Color.PaleGoldenrod * fadeOpacity;
        }
        public int trailLength = 6;
        internal float WidthFunction(float completionRatio, Vector2 vertex)
        {
            float width = completionRatio * 8f;
            return width > 0 ? width : 0;
        }
        public List<Vector2> trailPositions = new List<Vector2>();
        public override void AI()
        {
            base.AI();
            trailPositions.Add(Position);
            if (trailPositions.Count > trailLength)
            {
                trailPositions.RemoveAt(0);
            }
        }

        public override void Draw()
        {
            if (trailPositions is null)
                return;

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail"));
            PrimitiveRenderer.RenderTrail(trailPositions, new(WidthFunction, ColorFunction, (_, _) => Vector2.One * this.Scale * 0.5f, false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), trailLength);
        }
    }
}
