using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class SlashDarkRed : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Sn2").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 16;
        }
        public override void AI()
        {
            base.AI();
            sW = float.Lerp(sW, 0, ((float)this.Lifetime / TimeLeftMax) * 0.1f);
            this.Velocity *= 0.9f;
            this.Opacity = (float)this.Lifetime / (TimeLeftMax/1.8f);
            if (Opacity > 1) Opacity = 1;
           
        }
        public float sW = 1;
        public float height = 0.4f;
        public override void Draw()
        {
            float scale = sW;// (this.Lifetime - 2f) / TimeLeftMax;
            for (float r = 0; r < 359; r += 45)
            {
                float rot = r.ToRadians();
                DrawSlash(Scale * 128, height, scale * scw, Rotation, Color * Opacity, Position + r.ToRotationVector2() * 2);
            }
        }
        public void DrawEffect()
        {
            Color clr = this.Color;
            if (!this.glow)
            {
                clr = Lighting.GetColor(((int)(this.Position.X / 16)), ((int)(this.Position.Y / 16)), clr);
            }
            if (!this.useAdditive && !this.useAlphaBlend)
            {
                clr.A = (byte)(clr.A * Opacity);
            }
            else
            {
                clr *= Opacity;
            }
            float scale = sW;// (this.Lifetime - 2f) / TimeLeftMax;
            DrawSlash(Scale * 128, height, scale * scw, Rotation, Color.Black * Opacity, Position);
        }
        public float scw = 0.36f;
        public void DrawSlash(float width, float HeightMult, float scale, float rotation, Color color, Vector2 center)
        {
            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<Vector2> p1 = new List<Vector2>();
            List<Vector2> p2 = new List<Vector2>();
            for (float i = -MathHelper.PiOver2; i <= MathHelper.PiOver2; i += MathHelper.Pi / 26f)
            {
                p2.Add(((i * 1f).ToRotationVector2() * width * new Vector2(1, HeightMult)).RotatedBy(rotation));
                p1.Add(((i * 1f).ToRotationVector2() * width * new Vector2(1 - scale, HeightMult)).RotatedBy(rotation));
            }
            for (int i = 0; i < p1.Count; i++)
            {
                Color b = color;
                ve.Add(new ColoredVertex(center - Main.screenPosition + p1[i],
                      new Vector3((i) / ((float)p1.Count - 1), 1, 1),
                      b));
                ve.Add(new ColoredVertex(center - Main.screenPosition + p2[i],
                      new Vector3((i) / ((float)p1.Count - 1), 0, 1),
                      b));
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                gd.Textures[0] = CEUtils.pixelTex;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

            }
        }
    }
}
