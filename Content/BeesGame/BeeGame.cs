using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.BeesGame
{
    public static class BeeGame
    {
        public static bool Active = false;
        public static int maxWidth = 960;
        public static int maxHeight = 540;
        public static RenderTarget2D screen;
        public static int x;
        public static List<BeeGameProjectile> projectiles;
        public static List<BeeGameEnemy> enemies;
        public static Texture2D Load(string name)
        {
            return ModContent.Request<Texture2D>("CalamityEntropy/Content/BeesGame/" + name).Value;
        }
        public static PlayerBee player;
        public static int gameCounter = 0;
        public static void init()
        {
            gameCounter = 0;
            projectiles = new List<BeeGameProjectile>();
            enemies = new List<BeeGameEnemy>();
            screen = new RenderTarget2D(Main.graphics.GraphicsDevice, maxWidth, maxHeight);
            player = new PlayerBee();
            x = 0;
        }

        public static void update()
        {
            player.update();
            for(int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].update();
                if (projectiles[i].timeLeft <= 0)
                {
                    projectiles.RemoveAt(i);
                }
            }
            for(int i = enemies.Count - 1;i >= 0; i--)
            {
                enemies[i].update();
                if (enemies[i].life <= 0)
                {
                    enemies.RemoveAt(i);
                }
            }
            x += 5;
            if(gameCounter % 180 == 40)
            {
                Enemy1 e = new Enemy1();
                e.spawnAt(new Vector2(2000, Main.rand.Next(50, 1030)), Vector2.Zero);
            }
            gameCounter++;
        }

        public static void draw()
        {
            Texture2D sky = Load("sky");
            Texture2D b1 = Load("back_front");
            Texture2D b2 = Load("back_behind");
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(sky, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.Draw(b2, new Vector2(-x / 2 % b2.Width, 0), Color.White);
            spriteBatch.Draw(b2, new Vector2(-x / 2 % b2.Width + b2.Width, 0), Color.White);

            spriteBatch.Draw(b1, new Vector2(-x % b1.Width, 0), Color.White);
            spriteBatch.Draw(b1, new Vector2(-x % b1.Width + b1.Width, 0), Color.White);
            foreach (BeeGameProjectile p in projectiles) {
                p.draw();
            }
            foreach (BeeGameEnemy enemy in enemies)
            {
                enemy.draw();
            }
            player.Draw();
            
        }
    }
}
