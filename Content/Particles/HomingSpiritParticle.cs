using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class HomingSpiritParticle : EParticle
    {
        public override Texture2D Texture => CEUtils.getExtraTex("Glow2");
        public Vector2 TargetPos = Vector2.Zero;
        public float Opc = 0;
        public override void OnSpawn()
        {
            UpdateTimes = 6;
        }
        public float h = 0;
        public override void AI()
        {
            base.AI();
            Lifetime = 5;
            if(Opc < 1)
            {
                Opc += 0.05f;
            }
            h = 0.6f + Utils.Remap(CEUtils.getDistance(Position, TargetPos), 0, 900, 1, 0);
            Velocity *= 1f - h * 0.012f;
            Velocity += (TargetPos - Position).normalize() * 0.056f * h;
            if(CEUtils.getDistance(Position, TargetPos) < Velocity.Length() * 1.1f + 64)
            {
                Lifetime = 0;
                Velocity *= 0;
            }
            OldPos.Add(Position);
            if (OldPos.Count > 80)
            {
                OldPos.RemoveAt(0);
            }
        }
        public List<Vector2> OldPos = new List<Vector2>();
        public override void Draw()
        {
            var sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < (int)OldPos.Count; i++)
            {
                float s = (1f + i) / OldPos.Count;
                sb.Draw(Texture, OldPos[i] - Main.screenPosition, null, Color * Opc * s, 0, Texture.Size() * 0.5f, s * 0.16f, SpriteEffects.None, 0);
            }
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

        }
    }
}
