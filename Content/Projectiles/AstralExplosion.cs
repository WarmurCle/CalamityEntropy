using CalamityMod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class AstralExplosion : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 400;
            Projectile.height = 400;
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
                Util.Util.PlaySound("explosion", 1, Projectile.Center, 4);
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, new Color(160, 120, 255), new Vector2(2f, 2f), 0, 0.1f, 2 * 0.85f, 46);
                GeneralParticleHandler.SpawnParticle(pulse);
                CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, new Color(180, 156, 255), Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 2.2f * 0.65f, 30);
                GeneralParticleHandler.SpawnParticle(explosion2);
            }
            Projectile.ai[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

}