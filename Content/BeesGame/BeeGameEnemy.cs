using Microsoft.Xna.Framework;

namespace CalamityEntropy.Content.BeesGame
{
    public class BeeGameEnemy
    {
        public Vector2 position = new Vector2();
        public Vector2 velocity = new Vector2();
        public float rotation = 0;
        public int width = 36;
        public int height = 36;
        public float lifeMax = 20;
        public float life = 20;
        public float knockback = 1;
        public virtual void update()
        {
            counter++;
            position += velocity;
        }
        public int counter = 0;
        public virtual void draw()
        {

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
        public virtual void onSpawn()
        {

        }
        public virtual Rectangle getRect()
        {
            return new Rectangle((int)(position.X - 6), (int)(position.Y - 6), 12, 12);
        }
        public virtual void spawnAt(Vector2 position, Vector2 vel, float rot = 0)
        {
            this.position = position;
            this.rotation = rot;
            this.velocity = vel;
            BeeGame.enemies.Add(this);
            this.onSpawn();
        }
    }
}
