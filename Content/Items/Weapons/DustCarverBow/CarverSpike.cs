using CalamityMod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.DustCarverBow
{
    public class CarverSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.width = Projectile.height = 32;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 256;
        }

        public override void AI()
        {
            GeneralParticleHandler.SpawnParticle(new AltSparkParticle(Projectile.Center - Projectile.velocity * 4, Projectile.velocity * 4, false, 8, Main.rand.NextFloat(0.2f, 0.4f), new Color(200, 0, 0)));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("GrassSwordHit2", Main.rand.NextFloat(1.2f, 1.6f), target.Center, volume: 0.6f);
            for(int i = 0; i < 6; i++)
                GeneralParticleHandler.SpawnParticle(new AltSparkParticle(target.Center, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.2f, 2), false, 18, Main.rand.NextFloat(0.3f, 0.6f), new Color(200, 0, 0)));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }
}
