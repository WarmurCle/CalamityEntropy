using CalamityMod.Particles;
using CalamityMod.Projectiles.Turret;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class WaterExplosion : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 34;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = Util.CUtil.rogueDC;
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.6f;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 126;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.AliceBlue, new Vector2(2f, 2f), 0, 0.1f, 2 * 0.85f, 46);
                GeneralParticleHandler.SpawnParticle(pulse);
                CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.SkyBlue, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 2.2f * 0.65f, 30);
                GeneralParticleHandler.SpawnParticle(explosion2);
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 18), ModContent.ProjectileType<WaterShot>(), Projectile.damage / 10, Projectile.knockBack + 0.1f, Projectile.owner);
                    }
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