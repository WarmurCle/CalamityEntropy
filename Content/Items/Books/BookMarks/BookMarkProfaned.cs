using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
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
            if (projectile.Entropy().counter % 14 == 0 && ownerClient)
            {
                NPC target = projectile.FindTargetWithinRange(800);
                if (target != null)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, (target.Center - projectile.Center).normalize() * 9, ModContent.ProjectileType<HolyColliderHolyFire>(), projectile.damage / 10, projectile.knockBack, projectile.owner).ToProj().DamageType = projectile.DamageType;
                }
            }
        }
    }
}