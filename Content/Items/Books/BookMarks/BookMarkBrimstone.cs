using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkBrimstone : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Brimstone");
        public override Color tooltipColor => new Color(180, 6, 6);
        public override EBookProjectileEffect getEffect()
        {
            return new BrimstoneBMEffect();
        }
    }

    public class BrimstoneBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            if (!(projectile.ModProjectile is BrimstoneVortex))
            {
                if (ownerClient && Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 2 : 3))
                {
                    Vector2 pos = projectile.Center - projectile.velocity.normalize() * 190 + CEUtils.randomVec(128);
                    int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), pos, (Main.MouseWorld - pos).normalize() * 32, ModContent.ProjectileType<BrimstoneVortex>(), projectile.damage / 16, projectile.knockBack, projectile.owner);
                    (p.ToProj().ModProjectile as EBookBaseProjectile).homing = (projectile.ModProjectile as EBookBaseProjectile).homing;
                    (p.ToProj().ModProjectile as EBookBaseProjectile).ProjectileEffects = (projectile.ModProjectile as EBookBaseProjectile).ProjectileEffects;

                    p.ToProj().penetrate = projectile.penetrate;
                }
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.BrimstoneFlames>(), 300);
        }
    }
}