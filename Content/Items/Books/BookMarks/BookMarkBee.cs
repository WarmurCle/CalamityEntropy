using CalamityEntropy.Util;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkBee : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Bee");
        public override EBookProjectileEffect getEffect()
        {
            return new BeeBMEffect();
        }

        public override Color tooltipColor => Color.Orange;
    }

    public class BeeBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            for (int i = 0; i < 1; i++)
            {
                Vector2 shotDir = Util.Util.randomRot().ToRotationVector2();
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center + shotDir * 32, shotDir * 6, ProjectileID.Bee, damageDone / 6, projectile.knockBack / 3, projectile.owner).ToProj().DamageType = projectile.DamageType;
            }
        }
    }
}