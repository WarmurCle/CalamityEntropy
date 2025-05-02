using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkTaurus : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().stroke = true;
            Item.Entropy().NameColor = Color.LightBlue;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().tooltipStyle = 4;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Taurus");
        public override Color tooltipColor => Color.LightBlue;
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Size += 1f;
        }
        public override EBookProjectileEffect getEffect()
        {
            return new TaurusBMEffect();
        }
    }

    public class TaurusBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (projectile.ModProjectile is EBookBaseLaser laser)
            {
                float r = Utilities.Util.randomRot();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vel = (MathHelper.ToRadians(i * 120) + r).ToRotationVector2() * projectile.velocity.Length();
                    int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, vel, projectile.type, projectile.damage / 12 + 1, projectile.knockBack / 3, projectile.owner);
                    var m = (p.ToProj().ModProjectile as EBookBaseLaser);
                    foreach (var effect in (projectile.ModProjectile as EBookBaseProjectile).ProjectileEffects)
                    {
                        if (!(effect is TaurusBMEffect))
                        {
                            m.ProjectileEffects.Add(effect);
                        }
                    }
                    EBookBaseLaser esb = (projectile.ModProjectile as EBookBaseLaser);
                    m.homing = esb.homing;
                    m.quickTime = 20;
                    m.ShooterModProjectile = esb.ShooterModProjectile;
                    if (m.penetrate > 0)
                    {
                        m.penetrate = esb.penetrate + 1;
                    }
                    p.ToProj().localNPCImmunity[target.whoAmI] = 1000;
                    p.ToProj().scale = projectile.scale * 0.4f;
                }
            }
            else
            {
                if (projectile.ModProjectile is EBookBaseProjectile eb)
                {
                    float r = Utilities.Util.randomRot();
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 vel = (MathHelper.ToRadians(i * 120) + r).ToRotationVector2() * projectile.velocity.Length();
                        int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, vel, projectile.type, projectile.damage / 12 + 1, projectile.knockBack / 3, projectile.owner);
                        var m = (p.ToProj().ModProjectile as EBookBaseProjectile);
                        foreach (var effect in (projectile.ModProjectile as EBookBaseProjectile).ProjectileEffects)
                        {
                            if (!(effect is TaurusBMEffect))
                            {
                                m.ProjectileEffects.Add(effect);
                            }
                        }
                        EBookBaseProjectile esb = (projectile.ModProjectile as EBookBaseProjectile);
                        p.ToProj().localNPCImmunity[target.whoAmI] = 1000;
                        if (p.ToProj().penetrate > 0)
                        {
                            p.ToProj().penetrate = projectile.penetrate + 1;
                        }
                        m.ShooterModProjectile = esb.ShooterModProjectile;
                        m.homing = esb.homing;
                        p.ToProj().scale = projectile.scale * 0.4f;
                    }
                    projectile.Kill();
                }
            }

        }
    }
}