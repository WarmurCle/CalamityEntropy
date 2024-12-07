using Microsoft.Xna.Framework;

namespace CalamityEntropy.Content.BeesGame
{
    public class BeeGameProjectile
    {
        public Vector2 position = new Vector2();
        public Vector2 velocity = new Vector2();
        public float rotation = 0;
        public int timeLeft = 300;
        public float damage = 0;
        public virtual bool friendly()
        {
            return false;
        }
        public virtual void update()
        {
            timeLeft--;
            position += velocity;
            if (this.friendly())
            {
                foreach (BeeGameEnemy e in BeeGame.enemies)
                {
                    if(this.collide(e.getRect()))
                    {
                        e.hurt(this.damage);
                        e.velocity += this.velocity * e.knockback;
                        this.timeLeft = 0;
                        break;
                    }
                }
            }
            else
            {
                if (this.collide(BeeGame.player.getRect()))
                {
                    BeeGame.player.life -= this.damage;
                    this.timeLeft = 0;
                }
            }
        }
        public bool collide(Rectangle rect)
        {
            return this.getRect().Intersects(rect);
        }
        public virtual void draw()
        {

        }

        public virtual Color getColor()
        {
            return Color.White;
        }

        public virtual void onSpawn()
        {

        }
        public virtual Rectangle getRect()
        {
            return new Rectangle((int)(position.X - 16), (int)(position.Y - 16), 32, 32);
        }
        public virtual void spawnAt(Vector2 position, Vector2 vel, float damage, float rot = 0)
        {
            this.position = position;
            this.damage = damage;
            this.rotation = rot;
            this.velocity = vel;
            BeeGame.projectiles.Add(this);
            this.onSpawn();
        }
    }
}
