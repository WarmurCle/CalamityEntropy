using CalamityEntropy.Content.Buffs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Cruiser
{

    public class VoidStar : ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public float Hue => 0.55f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VoidTouch>(), 160);
        }
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.CheckProjs.Add(Projectile);
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 520;
            Projectile.extraUpdates = 1;
        }
        public bool setv = true;

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
                Content.Particles.EParticle.spawnNew(new Content.Particles.CruiserWarn(), Projectile.Center, Projectile.velocity * 6, Color.White * 0.6f, 0.016f * Projectile.velocity.Length(), 1, true, BlendState.Additive, Projectile.velocity.ToRotation());
            Projectile.ai[0]++;
            if (Projectile.ai[2] == 1 && Projectile.ai[0] < 60)
            {
                return;
            }
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

            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 255 / 40;
            }

            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);

            if (Main.rand.NextBool(2))
            {
                Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 20, Main.rand.NextFloat(0.6f, 1.2f) * Projectile.scale, 0.28f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.NextBool(3))
                {
                    Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1, 0.7f), 15, Main.rand.NextFloat(0.4f, 0.7f) * Projectile.scale, 0.8f, 0, true, 0.05f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }
        public override bool ShouldUpdatePosition()
        {
            if (Projectile.ai[2] == 1 && Projectile.ai[0] < 60)
            {
                return false;
            }
            return base.ShouldUpdatePosition();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] == 1)
            {
                if (Projectile.ai[0] < 60)
                {
                    CEUtils.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, Projectile.Center, Projectile.Center + Projectile.velocity * 1000, Color.Purple * (0.8f * Projectile.ai[0] / 60f), 2);
                }
            }
            return false;
        }
    }

}