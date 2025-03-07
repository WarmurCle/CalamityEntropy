using System;
using System.Collections.Generic;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class SilvaSoul : EBookBaseProjectile
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
            EParticle.spawnNew(new LifeLeaf(), Projectile.Center, Util.Util.randomVec(6), Color.White, Main.rand.NextFloat(0.6f, 1.4f), 1, false, BlendState.AlphaBlend, Util.Util.randomRot());
            if (Projectile.timeLeft < 3)
            {
                return;
            }
            counter++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (counter < 32)
            {
                Projectile.velocity *= 0.96f;
            }
            else
            {
                Player target = Projectile.getOwner();
                if (l < 9)
                {
                    l += 0.03f;
                }
                Projectile.velocity = new Vector2(Projectile.velocity.Length() + 1.4f, 0).RotatedBy(Util.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 0.5f * l, false));
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Util.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 1f * l, true));
                if (Projectile.getRect().Intersects(target.getRect()))
                {
                    for (int i = 0; i < 42; i++)
                    {
                        EParticle.spawnNew(new GlowSpark(), Projectile.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(2, 7), Color.Gold, Main.rand.NextFloat(0.08f, 0.12f), 1, true, BlendState.Additive, 0);
                    }
                    Util.Util.PlaySound("soulshine", 1f, Projectile.Center, maxIns: 6, volume: 0.6f);
                    Projectile.Kill();
                    Projectile.getOwner().Heal(Projectile.getOwner().statLifeMax2 / 80);
                    Projectile.getOwner().Entropy().temporaryArmor += 4.5f;
                    return;
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
        public void drawT() {
            if(Projectile.timeLeft < 3)
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

                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 12 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 12 * Projectile.scale,
                          new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                          b * a));
                    lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                }
                a = 1;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/SilvaSoul").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                }


            }

        }
    }

}