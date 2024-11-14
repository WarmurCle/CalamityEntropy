using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
namespace CalamityEntropy.Projectiles
{
    
    public class VoidMonsterShoot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 5000;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.extraUpdates = 1;
            Projectile.ArmorPenetration = 36;
        }
        public float ap = 0;
        public override void AI(){
            Particle p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.1f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.2f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.3f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.4f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.5f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.6f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.7f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.8f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.alpha = 0.14f;
            p.position = Projectile.Center - Projectile.velocity * 0.9f;
            VoidParticles.particles.Add(p);

            NPC target = Projectile.FindTargetWithinRange(900, false);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (target != null)
            {
                Projectile.velocity *= 0.9f;
                Vector2 v = target.Center - Projectile.Center;
                v.Normalize();

                Projectile.velocity += v * 2.6f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 120, 4, 800, 16);
        }

        public override bool PreDraw(ref Color lightColor)
        {return false;
        }
    }

}