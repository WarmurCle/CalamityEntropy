using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkTerra : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Terra");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Damage += 0.15f;
            modifer.PenetrateAddition += 3;
        }
        public override int modifyBaseProjectile()
        {
            return ModContent.ProjectileType<TerraBoulder>();
        }
        public override Color tooltipColor => Color.YellowGreen;
    }

    public class TerraBoulder : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = true;
            this.gravity = 0.6f;
            Projectile.extraUpdates = 1;
            Projectile.width = Projectile.height = 32;
        }

        public override void AI()
        {
            base.AI();
            if (Projectile.velocity.X > 0)
            {
                Projectile.rotation += 0.1f;
            }
            else
            {
                Projectile.rotation -= 0.1f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.X != 0 && Projectile.velocity.X == 0)
            {
                Projectile.velocity.X = oldVelocity.X * -1;
            }
            if (oldVelocity.Y != 0 && Projectile.velocity.Y == 0)
            {
                Projectile.velocity.Y = oldVelocity.Y * -1f;
            }
            if (Main.rand.NextBool(5))
            {
                Projectile.penetrate -= 1;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = this.color;
            Utilities.Util.DrawAfterimage(Projectile.GetTexture(), Projectile.Entropy().odp, Projectile.Entropy().odr, Projectile.scale);
            return base.PreDraw(ref lightColor);
        }
    }

}