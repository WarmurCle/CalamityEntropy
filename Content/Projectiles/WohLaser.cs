using CalamityEntropy.Common;
using CalamityMod.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class WohLaser : ModProjectile
    {
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.CheckProjs.Add(Projectile);
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public int length = 2400;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 80;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.ai[1] = 1;
            Projectile.ArmorPenetration = 36;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = 1;

            }

            if (Projectile.ai[0] > 10)
            {
                Projectile.ai[1] *= 0.955f;
            }
            Projectile.ai[0]++;
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.netUpdate = true;
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] > 4)
            {
                return false;
            }
            float laserLength = length * Projectile.scale;
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.One) * laserLength, targetHitbox, 12);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float sparkCount = 16;
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 sparkVelocity2 = Projectile.velocity.RotateRandom(0.2f) * Main.rand.NextFloat(0.5f, 1.8f);
                int sparkLifetime2 = Main.rand.Next(20, 24);
                float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                Color sparkColor2 = Color.DarkBlue;

                float velc = 1f;
                LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? Color.Purple : Color.Purple);
                GeneralParticleHandler.SpawnParticle(spark);

            }
            if (Projectile.ai[2] > 0)
            {
                EGlobalNPC.AddVoidTouch(target, 50, 5, 600, (int)Projectile.ai[2]);
            }
            else
            {
                EGlobalNPC.AddVoidTouch(target, 50, 5);
            }
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

    }

}