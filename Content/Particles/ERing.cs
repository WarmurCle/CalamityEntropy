
using CalamityEntropy.Content.Items.Vanity;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ERing : EParticle
    {
        public override Texture2D Texture => CEUtils.pixelTex;
        public Texture2D texture = CEUtils.getExtraTex("MegaStreakBacking2");
        public int PointCount = 64;
        public int LineWidth = 4;
        private float distance = 0;

        public override void AI()
        {
            base.AI();
            distance = Scale * (1-(((float)Lifetime) / TimeLeftMax));
        }
        public (List<Vector2>, List<Vector2>) calPoints()
        {
            List<Vector2> inside = new List<Vector2>();
            List<Vector2> outside = new List<Vector2>();

            float rAdd = MathHelper.TwoPi / PointCount;
            for (int i = 0; i <= PointCount; i++)
            {
                float rot = rAdd * i;
                float inA = distance - LineWidth / 2f;
                float outA = distance + LineWidth / 2f;

                if (inA < 0)
                    inA = 0;
                inside.Add(rot.ToRotationVector2() * inA);
                outside.Add(rot.ToRotationVector2() * outA);
            }
            return (inside, outside);
        }
        public override void Draw()
        {
            var points = calPoints();
            var pIn = points.Item1;
            var pOut = points.Item2;
            float Alpha = (((float)Lifetime) / TimeLeftMax);
            List<ColoredVertex> vertices = new List<ColoredVertex>();
            for(int i = 0; i < pIn.Count; i++)
            {
                Color c = Color * Alpha * Opacity;
                vertices.Add(new ColoredVertex(Position + pIn[i] - Main.screenPosition, new Vector3((float)i / (pIn.Count - 1), 1, 1), c));
                vertices.Add(new ColoredVertex(Position + pOut[i] - Main.screenPosition, new Vector3((float)i / (pIn.Count - 1), 0, 1), c));
            }
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            if (vertices.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = texture;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            }
        }
    }
}
