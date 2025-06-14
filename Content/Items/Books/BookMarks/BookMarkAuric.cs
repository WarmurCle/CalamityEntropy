using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.SamsaraCasket;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkAuric : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Auric");
        public override Color tooltipColor => Color.Goldenrod;
        public override EBookProjectileEffect getEffect()
        {
            return new BookMarkAuricBMEffect();
        }
    }

    public class BookMarkAuricBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (projectile.HasEBookEffect<APlusBMEffect>() ? true : Main.rand.NextBool(2))
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, CEUtils.randomRot().ToRotationVector2() * 22, ModContent.ProjectileType<DragonGoldenFire>(), projectile.damage / 5, projectile.knockBack, projectile.owner).ToProj().DamageType = projectile.DamageType;
            }
            for (int i = 0; i < (projectile.HasEBookEffect<APlusBMEffect>() ? 3 : 5); i++)
            {
                if (Main.rand.NextBool(2))
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, CEUtils.randomRot().ToRotationVector2() * 16, ModContent.ProjectileType<ZeratosBullet0>(), projectile.damage / 5, projectile.knockBack, projectile.owner).ToProj().DamageType = projectile.DamageType;
                }
            }
        }
    }
}