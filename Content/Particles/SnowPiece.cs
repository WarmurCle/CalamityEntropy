using CalamityEntropy.Content.Items.Books.BookMarks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class SnowPiece : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/SnowPiece").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 26;
        }
        public Player owner = null;
        public static int ProjType = -1;
        public override void AI()
        {
            base.AI();
            if (ProjType == -1) ProjType = ModContent.ProjectileType<Snowgrave>();
            if(owner != null && owner.ownedProjectileCounts[ProjType] < 1)
            {
                this.Opacity *= 0.7f;
            }
            if (this.Lifetime < 8)
                Color = Color.White;
            else
                this.Color = Color.Lerp(new Color(110, 110, 255), Color.White, 1 - ((this.Lifetime - 8) / 18));
        }
    }
    public class SnowStorm : EParticle
    {
        public override Texture2D Texture => CEUtils.pixelTex;
        public override void OnSpawn()
        {
            this.Lifetime = 26 * 4;
        }
        public Player owner = null;
        public static int ProjType = -1;
        public override void AI()
        {
            base.AI();
            if (ProjType == -1) ProjType = ModContent.ProjectileType<Snowgrave>();
            if (owner != null && owner.ownedProjectileCounts[ProjType] < 1)
            {
                this.Opacity *= 0.9f;
            }

        }
    }
}
