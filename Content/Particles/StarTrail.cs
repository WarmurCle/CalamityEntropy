using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class StarTrailParticle : EParticle
    {
        public List<Vector2> odp = new List<Vector2>();
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/StarTrail").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 30;
        }
        public int maxLength = 8;
        public bool addPoint = true;
        public override void AI()
        {
            base.AI();
            if (addPoint)
            {
                AddPoint(this.Position);
            }
            this.Velocity = this.Velocity + gravity * Vector2.UnitY * gA;
            Rotation = Velocity.ToRotation();
            this.Velocity *= 0.94f;
        }
        public float gravity = 0;
        public float gA = 1;

        public void AddPoint(Vector2 pos)
        {
            odp.Insert(0, pos);
            if (odp.Count > maxLength)
            {
                odp.RemoveAt(odp.Count - 1);
            }
        }

        public override void PreDraw()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Texture, this.Position - Main.screenPosition, null, this.Color * ((float)this.Lifetime / this.TimeLeftMax), this.Rotation, Texture.Size() / 2f, new Vector2(1.4f, 0.8f) * 0.22f * Scale, SpriteEffects.None, 0);
            if (odp.Count < 3)
            {
                return;
            }
            List<ColoredVertex> ve = new List<ColoredVertex>();
            Color b = this.Color * ((float)this.Lifetime / this.TimeLeftMax);
            ve.Add(new ColoredVertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 4 * this.Scale,
                      new Vector3((((float)0) / odp.Count), 1, 1),
                      b));
            ve.Add(new ColoredVertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 4 * this.Scale,
                  new Vector3((((float)0) / odp.Count), 0, 1),
                  b));
            for (int i = 1; i < odp.Count; i++)
            {
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 4 * this.Scale,
                      new Vector3((((float)i) / odp.Count), 1, 1),
                      b * ((odp.Count - i) / (float)odp.Count)));
                ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 4 * this.Scale,
                      new Vector3((((float)i) / odp.Count), 0, 1),
                      b * ((odp.Count - i) / (float)odp.Count)));
            }
            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                gd.Textures[0] = CEUtils.getExtraTex("Streak1");
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
        }
    }
}
