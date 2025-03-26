using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkSilva : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Silva");
        public override Color tooltipColor => Color.Green;
        public override EBookProjectileEffect getEffect()
        {
            return new SilvaBMEffect();
        }
    }

    public class SilvaBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(6))
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Util.Util.randomRot().ToRotationVector2() * 12, ModContent.ProjectileType<SilvaSoul>(), 0, projectile.knockBack, projectile.owner);
            }
        }
    }
}