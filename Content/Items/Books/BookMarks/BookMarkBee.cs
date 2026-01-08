using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            for (int i = 0; i < 1; i++)
            {
                Vector2 shotDir = CEUtils.randomRot().ToRotationVector2();
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center + shotDir * 16, shotDir * 12, ModContent.ProjectileType<SmallBee>(), (damageDone / 6).Softlimitation(50), projectile.knockBack / 3, projectile.owner).ToProj().DamageType = projectile.DamageType;
            }
        }
    }
}