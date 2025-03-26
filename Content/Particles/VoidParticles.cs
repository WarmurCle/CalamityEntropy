using System.Collections.Generic;

namespace CalamityEntropy.Content.Particles
{
    public class Particle
    {
        public Vector2 position = new Vector2();
        public Vector2 velocity = new Vector2();
        public float alpha = 1f;
        public float vd = 0.99f;
        public bool flag1 = false;
        public int shape = 0;
        public float rotation = 0;
        public float ad = 0.014f;
    }
    public static class VoidParticles
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
