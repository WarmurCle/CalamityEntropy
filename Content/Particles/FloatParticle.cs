using System.Collections.Generic;

namespace CalamityEntropy.Content.Particles
{

    public static class FloatParticles
    {
        public static List<Particle> particles = new List<Particle>();
        public static void Update()
        {
            foreach (Particle p in particles)
            {
                p.position += p.velocity;
                if (p.flag1)
                {
                    p.alpha -= 0.5f;
                }
                else
                {
                    p.alpha += 0.3f;
                    if (p.alpha > 10)
                    {
                        p.flag1 = true;
                    }
                }
                p.velocity *= 0.99f;
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].alpha < 0f)
                {
                    particles.RemoveAt(i);
                }
            }
        }
    }
}
