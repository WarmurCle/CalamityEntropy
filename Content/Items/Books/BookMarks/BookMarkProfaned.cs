using CalamityEntropy.Common;
using CalamityMod.Items;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkProfaned : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Profaned");
        public override EBookProjectileEffect getEffect()
        {
            return new ProfanedBMEffect();
        }
        public override Color tooltipColor => Color.Firebrick;
    }

    public class ProfanedBMEffect : EBookProjectileEffect
    {
        public override void UpdateProjectile(Projectile projectile, bool ownerClient)
        {
            projectile.Entropy().ShootCount += 0.025f;
            if (projectile.Entropy().ShootCount > 0 && projectile.Entropy().counter % 15 == 0 && ownerClient)
            {
                projectile.Entropy().ShootCount--;
                NPC target = projectile.FindTargetWithinRange(700);
                if (target != null)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, (target.Center - projectile.Center).normalize() * 9, ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(projectile.damage * 0.18f), projectile.knockBack, projectile.owner).ToProj().DamageType = projectile.DamageType;
                }
            }
        }
    }
}
