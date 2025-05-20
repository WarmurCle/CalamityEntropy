using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class SpiritLightSoul : EBookBaseProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public Vector2 dscp = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionShot[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
            Projectile.ArmorPenetration = 12;
        }
        public int counter = 0;
        public bool std = false;
        public float l = 0;
        public float c = 0.2f;
        public override void AI()
        {
            if (Projectile.timeLeft < 3)
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }
            base.AI();
            if (Projectile.timeLeft < 3)
            {
                return;
            }
            counter++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (counter < 20)
            {
                Projectile.velocity *= 0.96f;
            }
            else
            {
                NPC target = Utilities.Util.findTarget(Projectile.getOwner(), Projectile, 8000);
                if (target != null)
                {
                    if (l < 16)
                    {
                        l += 0.01f;
                    }
                    Projectile.velocity = new Vector2(Projectile.velocity.Length() + 2f, 0).RotatedBy(Utilities.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 0.6f * l, false));
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Utilities.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 1.4f * l, true));
                    if (Utilities.Util.getDistance(Projectile.Center, target.Center) < 100)
                    {
                        if (c < 1)
                        {
                            c += 0.1f;
                        }
                        Projectile.velocity = new Vector2(Projectile.velocity.Length() + 2f, 0).RotatedBy(Utilities.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), c, false));

                    }
                }
                else
                {
                    if (Projectile.timeLeft > 5)
                    {
                        Projectile.timeLeft -= 3;
                    }
                    Projectile.velocity = new Vector2(Projectile.velocity.Length() + 0.7f, 0).RotatedBy(Projectile.velocity.ToRotation() + 0.05f);
                }
                Projectile.velocity *= 0.96f;
            }
        }

        public override void PostAI()
        {
            base.PostAI();
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 10)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }
        }
        public int tofs;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft < 3)
            {
                return false;
            }
            drawT();
            DrawHead();
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.timeLeft > 2)
            {
                Utilities.Util.PlaySound("soulexplode", 1.2f, Projectile.Center, maxIns: 3, volume: 0.4f);
                Projectile.timeLeft = 2;
                Projectile.Resize(256, 256);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower += 1.6f;
                //CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(target.Center, Vector2.Zero, new Color(90, 150, 150), new Vector2(2f, 2f), 0, 0.1f, 0.65f * 0.8f, 18);
                //GeneralParticleHandler.SpawnParticle(pulse);
                //CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(target.Center, Vector2.Zero, new Color(80, 120, 160), Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.6f * 0.65f, 10);
                //GeneralParticleHandler.SpawnParticle(explosion2);
                for (int i = 0; i < 42; i++)
                {
                    EParticle.spawnNew(new Smoke() { timeLeft = 20, timeleftmax = 20 }, target.Center, Utilities.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 24) * 0.3f, Color.DarkGreen, 0.09f, 1, true, BlendState.Additive);
                    EParticle.spawnNew(new Smoke() { timeLeft = 20, timeleftmax = 20 }, target.Center, Utilities.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(6, 24) * 0.3f, Color.DarkGray, 0.09f, 1, true, BlendState.Additive);
                }
            }
        }
        public void DrawHead()
        {
            if (odp.Count >= 3)
            {
                Texture2D head = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/SpiritHead").Value;
                Main.EntitySpriteDraw(head, odp[odp.Count - 1] - Main.screenPosition, null, Color.White, (odp[odp.Count - 1] - odp[odp.Count - 2]).ToRotation(), head.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
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
                    Texture2D tx = Projectile.GetTexture();
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }


            }

        }
    }

}