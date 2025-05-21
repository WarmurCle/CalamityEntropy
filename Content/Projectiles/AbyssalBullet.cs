using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class AbyssalBullet : ModProjectile
    {
        public override string Texture => Util.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 400;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
        }
        NPC homing = null;
        public override void AI()
        {
            Projectile.localAI[0]++;
            if(homing == null)
            {
                homing = Projectile.FindTargetWithinRange(1200);
            }
            else
            {
                if (!homing.active)
                {
                    homing = null;
                }
            }
            if (homing != null && Projectile.localAI[0] > 19)
            {
                Projectile.velocity += (homing.Center - Projectile.Center).normalize() * 0.5f;
                Projectile.velocity *= 0.97f;
            }
            if(Projectile.timeLeft < 40)
            {
                Projectile.Opacity -= 1 / 40f;
            }
            for (int i = 0; i < 5; i++)
            {
                Particle p = new Particle();
                p.vd = 0.96f;
                p.ad = 0.05f;
                p.alpha = 0.38f * Main.rand.NextFloat(0.8f, 1);
                p.position = Projectile.Center;
                p.velocity = Utilities.Util.randomPointInCircle(3);
                AbyssalParticles.particles.Add(p);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
            for (int i = 0; i < 16; i++)
            {
                Particle p = new Particle();
                p.alpha = 0.6f * Main.rand.NextFloat(0.4f, 1);
                p.position = Projectile.Center;
                p.vd = 0.98f;
                p.velocity = Utilities.Util.randomPointInCircle(6);
                AbyssalParticles.particles.Add(p);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }


}