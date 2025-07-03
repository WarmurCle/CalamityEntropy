using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class SolarArrowSpawner : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, penetrate: -1);
            Projectile.timeLeft = 21;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void AI()
        {
            Projectile.Center = Projectile.GetOwner().Center;
            if(Projectile.timeLeft % 2 == 0 && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Main.MouseWorld - Projectile.GetOwner().Center).normalize().RotatedBy((float)Math.Cos(Main.GameUpdateCount * 0.2f) * 0.8f) * -10, ModContent.ProjectileType<SolarArrow>(), Projectile.damage, 1, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    public class SolarArrow : ModProjectile
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
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 12;
            Projectile.extraUpdates = 1;
        }
        public int counter = 0;
        public bool std = false;
        public int homingTime = 60;
        public StarTrailParticle spt = null;
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 9)
            {
                return false;
            }
            return null;
        }
        public override void AI()
        {
            if (spt == null)
            {
                spt = new StarTrailParticle() { maxLength = 18 };
                EParticle.NewParticle(spt, Projectile.Center, Vector2.Zero, Color.OrangeRed, 1f, 1, true, BlendState.Additive, 0);
            }
            spt.Velocity = Projectile.velocity * 0.2f;
            spt.Lifetime = 30;
            spt.Position = Projectile.Center;
            counter++;
            Projectile.ai[0]++;
            

            if (counter > 9 && Projectile.ai[2] == 0)
            {
                if (Projectile.velocity.Length() > 3)
                {
                    Projectile.velocity *= 0.995f - homing * 0.018f;
                }
                if (homing < 8)
                {
                    homing += 0.14f;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        float homing = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = 1;
            Projectile.netUpdate = true;
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.OrangeRed, new Vector2(2f, 2f), 0, 0.02f, 0.6f * 0.4f, 18);
            GeneralParticleHandler.SpawnParticle(pulse);
            CEUtils.PlaySound(Main.rand.NextBool() ? "scholarStaffImpact" : "scholarStaffImpact2", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
        }

        public int tofs;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }

    }
}
