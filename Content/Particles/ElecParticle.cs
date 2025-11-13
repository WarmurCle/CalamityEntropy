using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ElecParticle : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/UpdraftParticle").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 30;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = Lifetime / 30f;
        }
        public override void Draw()
        {
            List<Vector2> lol = new List<Vector2>();
            for (int i = 0; i < 9; i++)
            {
                lol.Add(Position + CEUtils.randomPointInCircle(32 * Scale));
            }
            CEUtils.DrawLines(lol, Color * Opacity, 4);
        }
    }
}
