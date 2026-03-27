using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class SakuraPetalsParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>($"CalamityEntropy/Content/Particles/SakuraPetalsParticle{tex}").Value;
        public override void OnSpawn()
        {
            this.Lifetime = Main.rand.Next(8 * 60, 14 * 60);
        }
        public int tex = Main.rand.Next(4);
        public bool FallThrough = Main.rand.NextBool(5);
        public bool check = true;
        public override void AI()
        {
            if(Lifetime % 22 == 0 && Velocity.X != 0)
            {
                FallThrough = Main.rand.NextBool(5);
            }
            if (Lifetime < 60)
                Opacity -= 1 / 60f;
            if(check)
            {
                check = false;
                if (CEUtils.CheckSolidTile(Position.getRectCentered(10, 10)))
                    Lifetime = 0;
            }
            Lifetime--;
            int w = 6; 
            if (FallThrough ? CEUtils.CheckSolidTileOrPlatform(Position.getRectCentered(w + 2, w + 2)) : CEUtils.CheckSolidTile(Position.getRectCentered(w + 2, w + 2)))
            {
                Velocity.X = 0;
            }
            if(Velocity.X == 0 && Lifetime > 60)
            {
                Lifetime -= 2;
            }
            Position += Collision.TileCollision(Position - new Vector2(w * 0.5f, w * 0.5f), Velocity, w, w, !FallThrough, !FallThrough);
            Rotation += Velocity.X * 0.02f;
        }
        public override void Draw()
        {
            base.Draw();
            CEUtils.DrawGlow(Position, Color.Pink * 0.46f * Opacity, 0.54f * Scale);
        }
    }
}
