using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class VoidStarF : ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public float Hue => 0.55f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.CheckProjs.Add(Projectile);
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 400;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.extraUpdates = 1;
            Projectile.ArmorPenetration = 40;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (counter < 16)
            {
                return false;
            }
            return null;
        }
        public bool setv = true;
        public int counter = 0;
        public override void AI()
        {
            if (Projectile.ai[2] > 0)
            {
                Projectile.DamageType = DamageClass.Magic;
                EParticle.spawnNew(new HeavenfallStar() { xScale = 0.14f }, Projectile.Center, Projectile.velocity.normalize(), new Color(255, 120, 120), Main.rand.NextFloat(0.6f, 1.3f) * 1.6f, 1, true, BlendState.Additive, Projectile.velocity.ToRotation(), 14);
                EParticle.spawnNew(new HeavenfallStar() { xScale = 0.14f }, Projectile.Center - Projectile.velocity / 2f, Projectile.velocity.normalize(), new Color(255, 120, 120), Main.rand.NextFloat(0.6f, 1.3f) * 1.6f, 1, true, BlendState.Additive, Projectile.velocity.ToRotation(), 14);

            }
            else
            {
                Projectile p = Projectile;
                if (Main.rand.NextBool(2))
                {
                    var smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Color.Lerp(Color.DodgerBlue.MultiplyRGB(p.ai[2] > 0 ? new Color(255, 80, 80) : Color.White), Color.MediumVioletRed.MultiplyRGB(Projectile.DamageType == DamageClass.Magic ? new Color(255, 140, 140) : Color.White), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 20, Main.rand.NextFloat(0.6f, 1.2f) * Projectile.scale, 0.28f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.NextBool(3))
                    {
                        var smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1, 0.7f).MultiplyRGB(p.ai[2] > 0 ? new Color(255, 80, 80) : Color.White), 15, Main.rand.NextFloat(0.4f, 0.7f) * Projectile.scale, 0.8f, 0, true, 0.05f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }
                }
            }
            counter++;
            if (setv)
            {
                setv = false;
                Projectile.velocity *= 0.5f;
            }
            odp.Add(Projectile.Center);
            if (odp.Count > 24)
            {
                odp.RemoveAt(0);
            }
            Projectile.velocity *= 0.999f;
            if (Projectile.timeLeft < 360)
            {
                NPC target = Projectile.FindTargetWithinRange(4000, false);
                if (target != null)
                {
                    Projectile.velocity *= 0.99f;
                    Vector2 v = target.Center - Projectile.Center;
                    v.Normalize();

                    Projectile.velocity += v * 0.4f;
                    if (Utilities.Util.getDistance(Projectile.Center, target.Center) < 180)
                    {
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy((target.Center - Projectile.Center).ToRotation());
                    }
                }
            }
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 255 / 40;
            }
            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);
            
            
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.ModNPC is AresBody || target.ModNPC is AresGaussNuke || target.ModNPC is AresLaserCannon || target.ModNPC is AresPlasmaFlamethrower || target.ModNPC is AresTeslaCannon)
            {
                modifiers.SourceDamage += 0.28f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
    }

}