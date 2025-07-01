using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkAbyss : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Abyss");
        public override EBookProjectileEffect getEffect()
        {
            return new AbyssBMEffect();
        }
        public override Color tooltipColor => new Color(93, 134, 196);
    }

    public class AbyssBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 2 : 4) && BMCooldowns.CheckCD(ref BMCooldowns.BMAbyss, 30))
            {
                int damage = projectile.damage / 12;
                Vector2 p = target.Center + CEUtils.randomRot().ToRotationVector2() * 300;
                Projectile.NewProjectile(projectile.GetSource_FromThis(), p, (target.Center - p).SafeNormalize(Vector2.One), ModContent.ProjectileType<AbyssBookmarkCrack>(), (int)projectile.GetOwner().GetTotalDamage(projectile.DamageType).ApplyTo(1250), projectile.knockBack, projectile.owner);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;
                CEUtils.PlaySound("crack", 1, projectile.Center, 3);
            }
        }
    }
}
