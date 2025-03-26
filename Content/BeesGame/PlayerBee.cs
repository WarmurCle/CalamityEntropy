using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace CalamityEntropy.Content.BeesGame
{
    public class PlayerBee
    {
        public Vector2 position = new Vector2();
        public Vector2 velocity = new Vector2();
        public float lifeMax = 40;
        public float life = 40;
        public void update()
        {
            float speed = 2f;
            counter++;
            if (counter % this.getAttackSpeedTickRate() == 0)
            {
                this.Shoot();
            }
            if (counter % 4 == 0)
            {
                frame++;
                if (frame >= 3)
                {
                    frame = 0;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity.X -= speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity.X += speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                velocity.Y -= speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                velocity.Y += speed;
            }
            velocity *= 0.84f;
            if (position.X < 16)
            {
                position.X = 16;
                velocity.X = 0;
            }
            if (position.Y < 16)
            {
                position.Y = 16;
                velocity.Y = 0;
            }
            if (position.X > 1920 - 16)
            {
                position.X = 1920 - 16;
                velocity.X = 0;
            }
            if (position.Y > 1080 - 16)
            {
                position.Y = 1080 - 16;
                velocity.Y = 0;
            }
            position += velocity;
        }

        public int getAttackSpeedTickRate()
        {
            return 15;
        }
        public int getShootSpeed()
        {
            return 18;
        }
        public bool hurt(float amount)
        {
            this.life -= amount;
            if (this.life < 0)
            {
                this.life = 0;
            }
            return true;
        }
        public float getDamage()
        {
            return 4;
        }
        public void Shoot()
        {
            new BeeSpike().spawnAt(position, new Vector2(this.getShootSpeed(), 0) + this.velocity * 0.4f, this.getDamage());
        }

        public Rectangle getRect()
        {
            return new Rectangle((int)(position.X - 12), (int)(position.Y - 12), 24, 24);
        }
        public int frame = 0;
        public int counter = 0;
        public void Draw()
        {
            Main.spriteBatch.Draw(BeeGame.Load("player"), position, new Rectangle(0, 40 * frame, 48, 38), Color.White, 0, new Vector2(24, 19), 2, SpriteEffects.None, 0);
        }
    }
}
