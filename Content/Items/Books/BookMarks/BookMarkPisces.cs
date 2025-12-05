using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkPisces : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().stroke = true;
            Item.Entropy().NameColor = Color.LightBlue;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().tooltipStyle = 4;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Pisces");
        public override Color tooltipColor => Color.LightBlue;
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.armorPenetration += 6;
        }
        public override EBookProjectileEffect getEffect()
        {
            return new PiscesBMEffect();
        }
    }

    public class PiscesBMEffect : EBookProjectileEffect
    {
        public override void OnShoot(EntropyBookHeldProjectile book)
        {
            book.ShootSingleProjectile(book.baseProjectileType, book.Projectile.Center, (Main.MouseWorld - book.Projectile.Center).normalize().RotatedByRandom(0.8f), 0.22f, 0.28f, initAction: (proj) => { if (proj.ModProjectile is EBookBaseLaser ebbl) { ebbl.quickTime = 30; } }, MainProjectile: true);
        }
    }
}