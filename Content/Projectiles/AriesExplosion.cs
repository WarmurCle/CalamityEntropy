using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class AriesExplosion : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 126;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                CEUtils.PlaySound("explosion", 1, Projectile.Center, 4);
                CalamityMod.Particles.Particle pulse = new TeslaExplosion(Projectile.Center, Vector2.Zero, new Color(160, 120, 255), new Vector2(2f, 2f), 0, 0f, 0.032f, 46);
                GeneralParticleHandler.SpawnParticle(pulse);
                CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, new Color(180, 156, 255), Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.4f, 30);
                GeneralParticleHandler.SpawnParticle(explosion2);
                for (int i = 0; i < 28; i++)
                {
                    EParticle.NewParticle(new Smoke() { Lifetime = 26, timeleftmax = 26 }, Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 16) * 0.2f, new Color(140, 140, 255), 0.06f, 1, true, BlendState.Additive);
                    EParticle.NewParticle(new Smoke() { Lifetime = 26, timeleftmax = 26 }, Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 16) * 0.2f, Color.LightGoldenrodYellow, 0.06f, 1, true, BlendState.Additive);
                }
            }
            Projectile.ai[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

}