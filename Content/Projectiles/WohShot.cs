using CalamityEntropy.Common;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class WohShot : ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public float angle;
        public float speed = 30;
        public bool htd = false;
        public float exps = 0;
        public Vector2 dscp = Vector2.Zero;
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.extraUpdates = 2;
            Projectile.ArmorPenetration = 12;
        }
        public int counter = 0;
        public bool std = false;
        public int homingTime = 60;
        public override void AI()
        {
            /*if (!std)
            {
                std = true;
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 4; i < 11; i++)
                    {
                        if (i % 2 == 0) 
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity, Projectile.velocity * (float)i * 0.13f, ModContent.ProjectileType<VoidStarF>(), (int)(Projectile.damage * 0.6f), 6, Projectile.owner);
                        }
                    }
                }
            }*/
            counter++;
            if (Projectile.ai[0] == 0)
            {
                angle = Projectile.velocity.ToRotation();
            }
            if (speed < 0)
            {
                angle = (Projectile.Center - Main.player[Projectile.owner].Center).ToRotation();
                if (CEUtils.getDistance(Projectile.Center, Main.player[Projectile.owner].Center) < Projectile.velocity.Length() * 1.12f)
                {
                    Projectile.Kill();
                }
            }

            if (!htd)
            {
                Dust.NewDust(Projectile.Center, 16, 16, DustID.MagicMirror, Projectile.velocity.X * -0.1f, Projectile.velocity.Y * -0.1f);
                Dust.NewDust(Projectile.Center, 16, 16, DustID.MagicMirror, Projectile.velocity.X * -0.1f, Projectile.velocity.Y * -0.1f);
                if (counter % 14 == 0)
                {
                }
            }

            Projectile.ai[0]++;
            if (htd)
            {
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);

                }
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                odp.Add(Projectile.Center);
                odr.Add(Projectile.rotation);
                if (odp.Count > 34)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }

                NPC target = Projectile.FindTargetWithinRange(1600, false);
                if (target != null && CEUtils.getDistance(target.Center, Projectile.Center) < 200 && counter > 16)
                {
                    homingTime = 0;
                    Projectile.velocity *= 0.9f;
                    Vector2 v = target.Center - Projectile.Center;
                    v.Normalize();

                    Projectile.velocity += v * 1.5f;
                }
            }
            exps *= 0.9f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (homingTime > 0)
            {
                Vector2 targetPos = new Vector2(Projectile.ai[1], Projectile.ai[2]);
                if (CEUtils.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center) > CEUtils.getDistance(targetPos, Projectile.owner.ToPlayer().Center))
                {
                    homingTime = 0;
                }
            }
            if (homingTime > 0 && counter > 10)
            {
                homingTime--;
                Vector2 targetPos = new Vector2(Projectile.ai[1], Projectile.ai[2]);

                float nr = CEUtils.rotatedToAngle(Projectile.velocity.ToRotation(), (targetPos - Projectile.Center).ToRotation(), 1.5f, true);
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(nr);
                if (nr == Projectile.velocity.ToRotation())
                {
                }

            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (htd)
            {
                return false;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!htd)
            {
                float sparkCount = 12;
                for (int i = 0; i < sparkCount; i++)
                {
                    Vector2 sparkVelocity2 = new Vector2(16, 0).RotatedByRandom(3.14159f) * Main.rand.NextFloat(0.5f, 1.8f);
                    int sparkLifetime2 = Main.rand.Next(20, 24);
                    float sparkScale2 = Main.rand.NextFloat(0.95f, 1.8f);
                    Color sparkColor2 = Color.DarkBlue;

                    float velc = 0.6f;
                    if (Main.rand.NextBool())
                    {
                        AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, sparkColor2);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f) + Projectile.velocity * 1.2f, sparkVelocity2 * velc, false, (int)(sparkLifetime2 * 1), sparkScale2 * 1, Main.rand.NextBool() ? Color.Purple : Color.Purple);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
                EGlobalNPC.AddVoidTouch(target, 30, 1);
                Projectile.timeLeft = 20;
                htd = true;
                exps = 1;
                odp.Add(Projectile.Center);
                odr.Add(Projectile.rotation);
            }
        }
        public int tofs;
        public Color TrailColor(float completionRatio)
        {
            Color result = new Color(200, 200, 255);
            return result;
        }

        public float TrailWidth(float completionRatio)
        {
            if (completionRatio > 0.92f)
            {
                return 36 * Projectile.scale * MathHelper.SmoothStep(0, 1, (1 - (completionRatio - 0.92f) / 0.08f));
            }
            return MathHelper.Lerp(0, 36 * Projectile.scale, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            drawT();
            return false;
        }

        public void drawT()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            var mp = this;
            if (mp.odp.Count > 1)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = new Color(30, 66, 100);

                float a = 0;
                float lr = 0;
                for (int i = 1; i < mp.odp.Count; i++)
                {
                    a += 1f / (float)mp.odp.Count;

                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 28,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new ColoredVertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 28,
                          new Vector3((float)(i + 1) / mp.odp.Count, 0, 1),
                          b * a));
                    lr = (mp.odp[i] - mp.odp[i - 1]).ToRotation();
                }
                a = 1;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/wohslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }


            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            tofs++;
            Texture2D value = TextureAssets.Projectile[base.Projectile.type].Value;
            Vector2 position = base.Projectile.Center - Main.screenPosition + Vector2.UnitY * base.Projectile.gfxOffY;
            Vector2 origin = value.Size() * 0.5f;
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor, (float _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            Main.spriteBatch.ExitShaderRegion();
            Main.EntitySpriteDraw(value, position, null, base.Projectile.GetAlpha(Color.White), base.Projectile.rotation, origin, base.Projectile.scale, SpriteEffects.None);

        }

    }

}