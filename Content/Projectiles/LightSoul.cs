using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class LightSoul : EBookBaseProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public Vector2 dscp = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 480;
            Projectile.ArmorPenetration = 12;
        }
        public int counter = 0;
        public bool std = false;
        public float l = 0;

        public override void AI()
        {
            base.AI();
            if (Projectile.timeLeft < 3)
            {
                return;
            }
            counter++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (counter < 32)
            {
                Projectile.velocity *= 0.95f;
            }
            else
            {
                Projectile target = null;
                float dist = 680;
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    float d = CEUtils.getDistance(p.Center, Projectile.Center);
                    if (p.hostile && Math.Max(p.width, p.height) < 100 && d < dist)
                    {
                        target = p;
                        dist = d;
                    }
                }
                if (target != null)
                {
                    if (l < 6)
                    {
                        l += 0.04f;
                    }
                    Projectile.velocity = new Vector2(Projectile.velocity.Length() + 1.4f, 0).RotatedBy(CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), l / 6f, false));
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 04f * l.ToRadians(), true));
                    if (Projectile.getRect().Intersects(target.getRect()))
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            EParticle.NewParticle(new GlowSpark(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(2, 7), Color.White, Main.rand.NextFloat(0.08f, 0.12f), 1, true, BlendState.Additive, 0);
                        }
                        CEUtils.PlaySound("soulexplode", 1.2f, Projectile.Center, maxIns: 2, volume: 0.6f);
                        Projectile.timeLeft = 2;
                        Projectile.Resize(256, 256);
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 6;
                        CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(target.Center, Vector2.Zero, Color.White, new Vector2(2f, 2f), 0, 0.1f, 0.85f * 0.5f, 18);
                        GeneralParticleHandler.SpawnParticle(pulse);
                        CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(target.Center, Vector2.Zero, Color.White, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.5f * 0.65f, 13);
                        GeneralParticleHandler.SpawnParticle(explosion2);
                        Projectile.Kill();
                        target.damage = (int)(target.damage * 0.4f);
                        Projectile.GetOwner()?.Entropy().TryHealMeWithCd(Projectile.GetOwner().statLifeMax2 / 400 + 1);
                        return;
                    }
                }
                else
                {
                    Projectile.velocity = new Vector2(Projectile.velocity.Length() + 0.7f, 0).RotatedBy(Projectile.velocity.ToRotation() + 0.05f);
                }
                Projectile.velocity *= 0.97f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void PostAI()
        {
            base.PostAI();
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 12)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
        }
        public int tofs;
        public Color TrailColor(float completionRatio)
        {
            Color result = new Color(255, 255, 255) * completionRatio;
            return result;
        }

        public float TrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0, 12 * Projectile.scale, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            drawT();
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override Color baseColor => new Color(255, 255, 255);
        public void drawT()
        {
            if (Projectile.timeLeft < 3)
            {
                return;
            }
            var mp = this;
            if (mp.odp.Count > 1)
            {
                Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = this.color;
                float a = 0;
                float lr = 0;
                for (int i = 1; i < mp.odp.Count; i++)
                {
                    a += 1f / (float)mp.odp.Count;

                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 9 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 9 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                          b * a));
                    lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                }
                a = 1;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/LightSoul").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                }


            }

        }
    }

}