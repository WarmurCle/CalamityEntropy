using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class PixelParticle
    {
        public float lifePercent = 0;
        public Vector2 startPos;
        public Vector2 endPos;
        public Vector2 midPos;
        public float j = 0;
        public Color _startColor;
        public Color _endColor;
        public PixelParticle(Vector2 start, Vector2 mid, Vector2 end, float lifeTime, Color startColor, Color endColor)
        {
            startPos = start;
            midPos = mid;
            endPos = end;
            j = 1f / lifeTime;
            _startColor = startColor;
            _endColor = endColor;
        }
        public static List<PixelParticle> particles = new List<PixelParticle>();
        public static void Update()
        {
            foreach (var p in particles)
            {
                p.lifePercent += p.j;
                if (p.lifePercent > 1)
                {
                    p.lifePercent = 1;
                }
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].lifePercent >= 1f)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public void Draw()
        {
            Vector2 a = Vector2.Lerp(startPos, midPos, lifePercent);
            Vector2 b = Vector2.Lerp(midPos, endPos, lifePercent);
            Vector2 drawPos = Vector2.Lerp(a, b, lifePercent);
            Texture2D pixel = CEUtils.pixelTex;
            Main.spriteBatch.Draw(pixel, drawPos - Main.screenPosition, null, Color.Lerp(_startColor, _endColor, lifePercent), 0, pixel.Size() / 2, 2, SpriteEffects.None, 0);
        }

        public static void drawAll()
        {
            foreach (var p in particles)
            {
                p.Draw();
            }
        }
    }
}
