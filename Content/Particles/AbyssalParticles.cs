using System.Collections.Generic;

namespace CalamityEntropy.Content.Particles
{
    public static class AbyssalParticles
    {
        public static List<Particle> particles = new List<Particle>();
        public static void Update()
        {
            foreach (Particle p in particles)
            {
                p.position += p.velocity;
                p.alpha -= p.ad;
                p.velocity *= p.vd;
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].alpha < 0.05f)
                {
                    particles.RemoveAt(i);
                }
            }
        }
    }
}
