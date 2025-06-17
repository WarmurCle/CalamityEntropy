using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class Starblight : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ArmorPenetration = 12;
        }
        public int counter = 0;
        public bool std = false;
        public int homingTime = 60;
        public StarTrailParticle spt = null;
        public override void AI()
        {
            if (spt == null)
            {
                spt = new StarTrailParticle();
                EParticle.NewParticle(spt, Projectile.Center, Vector2.Zero, Color.LightBlue, 1.6f, 1, true, BlendState.Additive, 0);
            }
            spt.Velocity = Projectile.velocity;
            spt.Lifetime = 30;
            counter++;
            Projectile.ai[0]++;

            NPC target = Projectile.FindTargetWithinRange(1600, false);
            if (target != null && CEUtils.getDistance(target.Center, Projectile.Center) < 200 && counter > 16)
            {
                homingTime = 0;
                Projectile.velocity *= 0.9f;
                Vector2 v = target.Center - Projectile.Center;
                v.Normalize();

                Projectile.velocity += v * 1.5f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();


            if (Projectile.velocity.Length() > 3)
            {
                Projectile.velocity *= 0.995f - homing * 0.018f;
            }
            if (counter > 2)
            {
                if (homing < 4)
                {
                    homing += 0.015f;
                }
                NPC targett = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 1200);

                if (targett != null)
                {
                    if (Projectile.timeLeft < 60)
                    {
                        Projectile.timeLeft = 60;
                    }
                    Projectile.velocity += (targett.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * homing * 2;
                }
            }
        }
        float homing = 0;

        public override void OnKill(int timeLeft)
        {
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.LightBlue, new Vector2(2f, 2f), 0, 0.02f, 0.85f * 0.4f, 18);
            GeneralParticleHandler.SpawnParticle(pulse);
            for (int i = 0; i < 5; i++)
            {
                EParticle.NewParticle(new StarTrailParticle(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(16, 36), Color.White, Main.rand.NextFloat(0.6f, 1.2f), 1, true, BlendState.Additive, 0);
            }
            CEUtils.PlaySound(Main.rand.NextBool() ? "scholarStaffImpact" : "scholarStaffImpact2", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
        }

        public int tofs;
        float alpha_ = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }


    }
    public class StarblightRogue : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ArmorPenetration = 12;
        }
        public int counter = 0;
        public bool std = false;
        public int homingTime = 60;
        public StarTrailParticle spt = null;
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 20)
            {
                return false;
            }
            return null;
        }
        public override void AI()
        {
            if (spt == null)
            {
                spt = new StarTrailParticle() { maxLength = 18};
                EParticle.NewParticle(spt, Projectile.Center, Vector2.Zero, Color.LightBlue, 1.8f, 1, true, BlendState.Additive, 0);
            }
            spt.Velocity = Projectile.velocity;
            spt.Lifetime = 30;
            counter++;
            Projectile.ai[0]++;

            NPC target = Projectile.FindTargetWithinRange(1600, false);
            if (target != null && CEUtils.getDistance(target.Center, Projectile.Center) < 200 && counter > 20)
            {
                homingTime = 0;
                Projectile.velocity *= 0.9f;
                Vector2 v = target.Center - Projectile.Center;
                v.Normalize();

                Projectile.velocity += v * 1.5f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();


            if (Projectile.velocity.Length() > 3)
            {
                Projectile.velocity *= 0.995f - homing * 0.018f;
            }
            if (counter > 2)
            {
                if (homing < 4)
                {
                    homing += 0.015f;
                }
                NPC targett = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 1200);

                if (targett != null)
                {
                    if (Projectile.timeLeft < 60)
                    {
                        Projectile.timeLeft = 60;
                    }
                    Projectile.velocity += (targett.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * homing * 2;
                }
            }
        }
        float homing = 0;

        public override void OnKill(int timeLeft)
        {
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.LightBlue, new Vector2(2f, 2f), 0, 0.02f, 0.85f * 0.4f, 18);
            GeneralParticleHandler.SpawnParticle(pulse);
            for (int i = 0; i < 5; i++)
            {
                EParticle.NewParticle(new StarTrailParticle(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(16, 36), Color.White, Main.rand.NextFloat(0.6f, 1.2f), 1, true, BlendState.Additive, 0);
            }
            CEUtils.PlaySound(Main.rand.NextBool() ? "scholarStaffImpact" : "scholarStaffImpact2", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
        }

        public int tofs;
        float alpha_ = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }


    }
}