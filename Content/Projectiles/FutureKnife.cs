using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class FutureKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 260;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;

        }
        public TrailParticle trail;
        public override void AI()
        {
            if (!Main.dedServ)
            {
                if (trail == null)
                {
                    trail = new TrailParticle() { maxLength = 16 };
                    EParticle.spawnNew(trail, Projectile.Center, Vector2.Zero, Color.AliceBlue * 0.6f, Projectile.scale * 0.6f * (Projectile.Calamity().stealthStrike ? 2 : 1), 1, true, BlendState.Additive);
                }
                trail.Lifetime = 30;
                trail.AddPoint(Projectile.Center + Projectile.velocity);
            }
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.scale = 2.4f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 246 && !Projectile.Calamity().stealthStrike)
            {
                var target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 800);
                if (target != null)
                {
                    Projectile.velocity += (target.Center - Projectile.Center).normalize() * 4f;
                    Projectile.velocity *= 0.92f;
                }
            }
            if (Main.GameUpdateCount % 2 == 0 || Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft <= 256)
                {
                    EParticle.NewParticle(new Particles.RuneParticle(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(-0.6f, 0.6f), Color.White, 0.8f, 1, true, BlendState.AlphaBlend, 0);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            trail?.AddPoint(Projectile.Center);
            for (int i = 0; i < 6; i++)
            {
                EParticle.NewParticle(new Particles.RuneParticle(), target.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(-5f, 5f), Color.White, 0.5f, 1, true, BlendState.AlphaBlend, 0);
            }
            CEUtils.PlaySound("crystalsound" + Main.rand.Next(1, 3).ToString(), Main.rand.NextFloat(0.7f, 1.3f), target.Center, 10, 0.4f);
            target.AddBuff(ModContent.BuffType<SoulDisorder>(), 300);
            if (Projectile.Calamity().stealthStrike)
            {
                for (float j = 0; j < 360f; j += 60)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ProphecyRune>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, MathHelper.ToRadians(j), 0, Main.rand.Next(1, 12));
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].Calamity().stealthStrike = true;
                        p.ToProj().netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }


}