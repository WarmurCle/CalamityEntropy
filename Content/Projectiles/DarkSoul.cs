using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class DarkSoul : EBookBaseProjectile
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
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 260;
            Projectile.ArmorPenetration = 12;
        }
        public int counter = 0;
        public bool std = false;

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
                NPC target = Projectile.FindTargetWithinRange(1000, false);
                if (target != null)
                {
                    if (l < 6)
                    {
                        l += l < 2 ? 0.014f : 0.01f;
                    }
                    Projectile.velocity = new Vector2(Projectile.velocity.Length() + 1.4f, 0).RotatedBy(Util.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 0.5f * l, false));
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Util.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 1.2f * l, true));
                }
                Projectile.velocity *= 0.97f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (counter < 32)
            {
                return false;
            }
            return base.CanHitNPC(target);
        }
        public override void PostAI()
        {
            base.PostAI();
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 16)
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
        public float l = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Projectile.timeLeft > 3)
            {
                for (int i = 0; i < 32; i++)
                {
                    EParticle.spawnNew(new GlowSpark(), Projectile.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(2, 7), Color.Red, Main.rand.NextFloat(0.1f, 0.16f), 1, true, BlendState.Additive, 0);
                }
                for (int i = 0; i < odp.Count; i++)
                {
                    for (int i_ = 0; i_ < 6; i_++)
                    {
                        EParticle.spawnNew(new GlowSpark(), odp[i], Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(2, 7) * ((float)i / odp.Count), Color.Red, Main.rand.NextFloat(0.1f, 0.16f) * ((float)i / odp.Count), 1, true, BlendState.Additive, 0);
                    }
                }
                Util.Util.PlaySound("soulexplode", 1.2f, Projectile.Center, maxIns: 4, volume: 0.8f);
                Projectile.timeLeft = 2;
                Projectile.Resize(256, 256);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 6;
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(target.Center, Vector2.Zero, Color.DarkRed, new Vector2(2f, 2f), 0, 0.1f, 1 * 0.85f, 36);
                GeneralParticleHandler.SpawnParticle(pulse);
                CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(target.Center, Vector2.Zero, Color.DarkRed, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 1 * 0.65f, 26);
                GeneralParticleHandler.SpawnParticle(explosion2);
            }
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
                List<Vertex> ve = new List<Vertex>();
                Color b = this.color;
                float a = 0;
                float lr = 0;
                for (int i = 1; i < mp.odp.Count; i++)
                {
                    a += 1f / (float)mp.odp.Count;

                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 14 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 14 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                          b * a));
                    lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                }
                a = 1;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/DarkSoul").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                }


            }

        }
    }

}