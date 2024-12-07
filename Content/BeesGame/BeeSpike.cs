using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.BeesGame
{
    public class BeeSpike : BeeGameProjectile
    {
        public override void update()
        {
            base.update();
            rotation = this.velocity.ToRotation();

        }
        public override bool friendly()
        {
            return true;
        }
        public override void draw()
        {
            Texture2D tex = BeeGame.Load("pspike");
            Main.spriteBatch.Draw(tex, this.position, null, this.getColor(), this.rotation, tex.Size() / 2, 2, SpriteEffects.None, 0);
        }
    }
}
