using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class MultiSlash : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/AbyssalLine").Value;
        public float timemax => this.TimeLeftMax;
        public Color spawnColor = new Color(190, 190, 255);
        public Color endColor = Color.Blue;
        public float xscale = 0;
        public float xdec = 0.87f;
        public float xadd = 3.2f;
        public float lx = 3;
        public override void OnSpawn()
        {
            this.Lifetime = 18;
        }
        public override void AI()
        {
            base.AI();
            if (this.Lifetime % 2 == 0)
            {
                EParticle.NewParticle(new AbyssalLine() { lx = this.lx * Main.rand.NextFloat(0.6f, 1.4f), xadd = this.xadd * Main.rand.NextFloat(0.6f, 1.4f), xdec = this.xdec, xscale = this.xscale, Color = this.Color, endColor = this.endColor }, this.Position, Vector2.Zero, Color, this.Scale, this.Opacity, this.glow, this.useAdditive ? BlendState.Additive : BlendState.NonPremultiplied, CEUtils.randomRot(), 8);
            }
        }
        public override void Draw()
        {
        }
    }
}
