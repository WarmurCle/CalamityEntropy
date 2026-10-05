using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CalamityEntropy.Core.Integrations.BossLog
{
    /// <summary>粒子用场景坐标,超过容量就丢掉新粒子</summary>
    internal sealed class CEPortraitMotes
    {
        private struct Mote
        {
            public Vector2 Pos;
            public Vector2 Vel;
            public Vector2 Size;
            public float Life;
            public float MaxLife;
            public float Rot;
            public float RotVel;
            public float Gravity;
            public float Drag;
            public Color Color;
            /// <summary>A 为 0 时按加色读,否则按不透明颗粒读</summary>
            public bool Additive;
        }

        private const int Cap = 220;
        private readonly List<Mote> motes = new(96);

        public int Count => motes.Count;

        public void Clear() => motes.Clear();

        public void Spawn(Vector2 pos, Vector2 vel, Vector2 size, Color color, float lifeSeconds,
            float gravity = 0f, float drag = 1f, float rot = 0f, float rotVel = 0f, bool additive = false) {
            if (motes.Count >= Cap) {
                return;
            }
            motes.Add(new Mote {
                Pos = pos,
                Vel = vel,
                Size = size,
                Color = color,
                Life = lifeSeconds,
                MaxLife = MathF.Max(lifeSeconds, 0.01f),
                Gravity = gravity,
                Drag = drag,
                Rot = rot,
                RotVel = rotVel,
                Additive = additive,
            });
        }

        /// <summary>frames 是 dt 乘 60,Life 按秒扣</summary>
        public void Update(float frames) {
            for (int i = motes.Count - 1; i >= 0; i--) {
                Mote m = motes[i];
                m.Life -= frames / 60f;
                if (m.Life <= 0f) {
                    motes.RemoveAt(i);
                    continue;
                }
                m.Vel.Y += m.Gravity * frames;
                m.Vel *= MathF.Pow(m.Drag, frames);
                m.Pos += m.Vel * frames;
                m.Rot += m.RotVel * frames;
                motes[i] = m;
            }
        }

        public void Draw(SpriteBatch sb, in CEPortraitFrame frame) {
            Texture2D pixel = VaultAsset.placeholder2?.Value;
            if (pixel == null || motes.Count == 0) {
                return;
            }
            Rectangle src = new(0, 0, 1, 1);
            Vector2 origin = new(0.5f, 0.5f);
            foreach (Mote m in motes) {
                float a = MathHelper.Clamp(m.Life / m.MaxLife, 0f, 1f);
                Color c = frame.Tint(m.Additive ? m.Color with { A = 0 } : m.Color) * a;
                sb.Draw(pixel, m.Pos, src, c, m.Rot, origin, m.Size, SpriteEffects.None, 0f);
            }
        }
    }
}
