using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Vanity;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class GravityGazeBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 80;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        float alpha = 1f;
        public override void AI()
        {
            if (s)
            {
                tail = Projectile.Center - Projectile.velocity;
                Projectile.ai[1] = Main.rand.Next(0, 1024);
                s = false;
            }
            if (Projectile.velocity.Length() > 14)
                Projectile.velocity *= 0.8f;
            else
                Projectile.velocity *= 0.96f;
            tail = tail + (Projectile.Center - tail) * 0.05f;
            if (Projectile.timeLeft < 30)
            {
                alpha -= 1f / 30f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (float i = 0.1f; i <= 1; i += 0.2f)
            {
                odp.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, i));
                rots.Add(Projectile.rotation);
                if (odp.Count > 42)
                {
                    rots.RemoveAt(0);
                    odp.RemoveAt(0);
                }
            }
        }

        Vector2 tail;
        public List<Vector2> odp = new List<Vector2>();
        public List<float> rots = new List<float>();
        public bool s = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.velocity.Length() >= 14f)
                CEUtils.PlaySound("CrystalBreak", 1.2f, target.Center, 8, 0.7f);
            float s = Projectile.velocity.Length() >= 14 ? 1 : 0.5f;
            for (int i = 0; i < 12; i++)
                GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(target.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(0.6f, 1) * 9 * s, false, 18, 0.03f * Main.rand.NextFloat(0.5f, 1f) * s, Main.rand.NextBool() ? Color.DeepSkyBlue : Color.LightGoldenrodYellow, new Vector2(0.6f, 1)), true);
            for(int i = 0; i < 4; i++)
                GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(target.Center, (i * MathHelper.PiOver2).ToRotationVector2() * 10 * s, false, 12, 0.05f * s, Main.rand.NextBool() ? Color.DeepSkyBlue : Color.LightGoldenrodYellow, new Vector2(1.5f, 1), true), true);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.velocity.Length() >= 14f)
                modifiers.SourceDamage *= 1.5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (s)
            {
                return false;
            }

            var gdv = Main.graphics.GraphicsDevice;
            Main.spriteBatch.End();
            EffectLoader.PreparePixelShader(gdv);
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Enchanted", AssetRequestMode.ImmediateLoad);

            if (odp != null && rots != null)
            {
                if (odp.Count > 1)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1").Value;
                        List<ColoredVertex> ve = new List<ColoredVertex>();
                        Color b = new Color(255, 255, 255) * 0.7f;
                        float p = -Main.GlobalTimeWrappedHourly * 1;
                        for (int i = 0; i < odp.Count; i++)
                        {
                            float pg = i / (odp.Count - 1f);
                            float w = CEUtils.Parabola(pg, 1);
                            float pgl = CEUtils.Frac(pg + Main.GlobalTimeWrappedHourly * 6f);
                            if (pgl < 0.5f)
                                b = Color.Lerp(Color.DeepSkyBlue, Color.LightGoldenrodYellow, pgl * 2);
                            else
                                b = Color.Lerp(Color.LightGoldenrodYellow, Color.DeepSkyBlue, (pgl - 0.5f) * 2);
                            b *= alpha;
                            ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + rots[i].ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 12 * Projectile.scale * w,
                                  new Vector3(p, 1, 1),
                                  b));
                            ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + rots[i].ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 12 * Projectile.scale * w,
                                  new Vector3(p, 0, 1),
                                  b));
                            if(i > 0)
                                p += (CEUtils.getDistance(odp[i], odp[i - 1]) / tx.Width) * 0.36f;
                        }

                        SpriteBatch sb = Main.spriteBatch;
                        GraphicsDevice gd = Main.graphics.GraphicsDevice;
                        if (ve.Count >= 3)
                        {
                            gd.Textures[0] = tx;
                            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

                        }
                    }
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D gx = CEUtils.getExtraTex("CrystalGlow");
            Main.spriteBatch.Draw(gx, Projectile.Center - Main.screenPosition, null, Color.AliceBlue * alpha, Projectile.rotation + MathHelper.PiOver2, gx.Size() * 0.5f, 2f * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();
            CEUtils.DrawGlow(Projectile.Center, Color.SkyBlue * 0.4f * alpha, 1.6f);
            CEUtils.DrawGlow(Projectile.Center, Color.SkyBlue * 0.8f * alpha, 0.7f);
            Main.spriteBatch.End();
            EffectLoader.ApplyPixelShader(gdv, state:BlendState.Additive);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }
    }


}