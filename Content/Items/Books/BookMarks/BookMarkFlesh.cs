using CalamityMod.Items;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkFlesh : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Flesh");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.PenetrateAddition += 1;
            modifer.lifeSteal += 1;
        }
        public override Color tooltipColor => Color.Red;
        public override EBookProjectileEffect getEffect()
        {
            return new FleshBMEffect();
        }
    }
    public class FleshBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (((EBookBaseProjectile)projectile.ModProjectile).hitCount == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Util.Util.randomRot().ToRotationVector2() * 4, ModContent.ProjectileType<BloodBeam>(), damageDone / 9, projectile.knockBack / 3, projectile.owner);
                }
            }
        }
    }
}