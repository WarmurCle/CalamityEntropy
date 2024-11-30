using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CalamityEntropy.BeesGame
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
