using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkForesee : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Foresee");
        public override Color tooltipColor => Color.SkyBlue;
        public override EBookProjectileEffect getEffect()
        {
            return new ForeseeBMEffect();
        }
    }

    public class ForeseeBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 4 : 6) && BMCooldowns.CheckCD(ref BMCooldowns.BMProphecy, 30))
            {
                float jj = 360f / Main.rand.Next(3, 8);
                if (projectile.HasEBookEffect<APlusBMEffect>())
                {
                    jj = 360f / Main.rand.Next(Main.rand.Next(3, 8), 8);
                }
                for (float j = 0; j < 354f; j += jj)
                {
                    int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ProphecyRuneBM>(), ((int)(projectile.damage * 0.24f)).Softlimitation(200), projectile.knockBack, projectile.owner, MathHelper.ToRadians(j), 0, Main.rand.Next(1, 12));
                }
            }
        }
    }
}