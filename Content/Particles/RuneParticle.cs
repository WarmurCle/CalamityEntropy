using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class RuneParticle : EParticle
    {
        public int frame = Main.rand.Next(0, 14);
        public override Texture2D texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/Runes/r" + frame.ToString()).Value;
        public override void onSpawn()
        {
            this.timeLeft = 42;
        }
        public override void update()
        {
            base.update();
            this.alpha = this.timeLeft / 42f;
            this.color = Color.Lerp(new Color(110, 120, 255), Color.White, this.alpha);
        }
    }
}
