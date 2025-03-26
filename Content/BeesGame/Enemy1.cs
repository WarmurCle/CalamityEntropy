using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.BeesGame
{
    public class Enemy1 : BeeGameEnemy
    {
        public override void onSpawn()
        {
            life = 16;
            lifeMax = 16;
            knockback = 0.1f;
            this.width = this.height = 62;
        }

        public override void update()
        {
            if (this.position.X > 1800)
            {
                this.velocity.X -= 0.6f;
            }

            this.velocity *= 0.96f;
            base.update();

        }

        public override void draw()
        {
            Texture2D tex = BeeGame.Load("Enemy1");
            Main.spriteBatch.Draw(tex, this.position, null, Color.White, this.rotation, tex.Size() / 2, 2, SpriteEffects.None, 0);
        }
    }
}
