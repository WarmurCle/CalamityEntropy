using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class LightningBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 1024;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 9;
        }
        public TrailParticle t1 = new TrailParticle();
        public TrailParticle t2 = new TrailParticle();
        public List<TrailParticle> ts = new List<TrailParticle>();
        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    var pt = new TrailParticle();
                    pt.maxLength = 11;
                    ts.Add(pt);
                    EParticle.NewParticle(pt, Projectile.Center, Vector2.Zero, (Projectile.ai[1] == 1 ? Color.Red : Color.White), Projectile.scale * 0.56f, 1, true, BlendState.NonPremultiplied);
                }
                t1.maxLength *= 4;
                t2.maxLength *= 4;

                EParticle.NewParticle(t1, Projectile.Center, Vector2.Zero, (Projectile.ai[1] == 1 ? Color.Red : Color.White), Projectile.scale * 0.5f, 1, true, BlendState.NonPremultiplied);
                EParticle.NewParticle(t2, Projectile.Center, Vector2.Zero, (Projectile.ai[1] == 1 ? Color.Red : Color.White), Projectile.scale * 0.5f, 1, true, BlendState.NonPremultiplied);
            }
            ++Projectile.localAI[1];
            t1.Lifetime = 13;
            t2.Lifetime = 13;
            foreach (var p in ts)
            {
                p.Lifetime = 13;
                if (Main.rand.NextBool(8))
                {
                    p.AddPoint(Projectile.Center + CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(-26, 26) * Projectile.scale);
                }
            }
            t1.AddPoint(Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2).normalize() * (float)(Math.Cos(Projectile.localAI[1] * 0.1f)) * (7 * Projectile.scale));
            t2.AddPoint(Projectile.Center + Projectile.velocity.RotatedBy(-MathHelper.PiOver2).normalize() * (float)(Math.Cos(Projectile.localAI[1] * 0.1f)) * (7 * Projectile.scale));
        }

        public override void OnKill(int timeLeft)
        {
            CEUtils.PlaySound("ofhit", 1, Projectile.Center);
            for (int i = 0; i < 16; i++)
            {
                EParticle.NewParticle(new TrailSparkParticle(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(2, 14), (Projectile.ai[1] == 1 ? Color.Red : Color.White), Projectile.scale, 1, true, BlendState.NonPremultiplied);
            }
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, (Projectile.ai[1] == 1 ? Color.Red : Color.White), new Vector2(2f, 2f), 0, 0.1f, (false ? 2 : 1) * 0.85f * 0.4f, (false ? 46 : 36));
            GeneralParticleHandler.SpawnParticle(pulse);
            CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, (Projectile.ai[1] == 1 ? Color.Red : Color.White), Vector2.One, Main.rand.NextFloat(-5, 5), 0f, (false ? 2.2f : 1) * 0.65f * 0.4f, (false ? 30 : 26));
            GeneralParticleHandler.SpawnParticle(explosion2);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile projectile = Projectile;
                for (int i = 0; i < 3 + Projectile.owner.ToPlayer().Entropy().WeaponBoost; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, new Vector2(30, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2), ModContent.ProjectileType<Lightning>(), (int)(projectile.damage * 0.45f), 4, projectile.owner, 0, 0, Projectile.ai[1]).ToProj().DamageType = DamageClass.Magic;
                }
            }
            SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/ExoTwinsEject"), Projectile.Center);
            if (CEUtils.getDistance(Projectile.Center, Projectile.getOwner().Center) < 1200)
            {
                Projectile.getOwner().Calamity().GeneralScreenShakePower = 7;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D light = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Glow2").Value;
            Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, (Projectile.ai[1] == 1 ? Color.Red : Color.White), 0, light.Size() / 2, 0.6f * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }


}