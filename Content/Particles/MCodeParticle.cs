using Microsoft.Xna.Framework.Graphics;
using System.Security.Policy;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class MCodeParticle : EParticle
    {
        public int frame = Main.rand.Next(0, 16);
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MALICIOUSCODE/t" + frame.ToString()).Value;
        public override void onSpawn()
        {
            this.timeLeft = 60;
        }
        public override void update()
        {
            base.update();
            if (Main.rand.NextBool(28))
            {
                if (Main.rand.NextBool())
                {
                    frame = Main.rand.Next(0, 16);
                }
                else
                {
                    this.position += Util.Util.randomVec(16);
                }
            }
        }
    }
}
