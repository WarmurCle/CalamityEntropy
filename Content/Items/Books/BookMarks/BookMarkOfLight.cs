using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkOfLight : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Light");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.armorPenetration += 8;
        }
        public override Color tooltipColor => Color.GreenYellow;
        public override EBookProjectileEffect getEffect()
        {
            return new LightSoulBMEffect();
        }
    }
    public class LightSoulBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 4 : 6))
            {
                (Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Utilities.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(30, 34), ModContent.ProjectileType<LightSoul>(), damageDone / 2, projectile.knockBack / 3, projectile.owner).ToProj().ModProjectile as EBookBaseProjectile).homing = 0;
            }
        }
    }
}