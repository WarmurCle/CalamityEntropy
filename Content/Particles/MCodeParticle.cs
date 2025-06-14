using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class MCodeParticle : EParticle
    {
        public int frame = Main.rand.Next(0, 16);
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MALICIOUSCODE/t" + frame.ToString()).Value;
        public override void OnSpawn()
        {
            this.Lifetime = 60;
        }
        public override void AI()
        {
            base.AI();
            if (Main.rand.NextBool(28))
            {
                if (Main.rand.NextBool())
                {
                    frame = Main.rand.Next(0, 16);
                }
                else
                {
                    this.Position += CEUtils.randomVec(16);
                }
            }
        }
    }
}
