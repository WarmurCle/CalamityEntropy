using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.LuminarisShoots
{

    public class LuminarisMinionAstralShoot : ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 300;
            Projectile.localNPCHitCooldown = 20;
            Projectile.usesLocalNPCImmunity = true;
        }
        public float counter = 0;
        public override void AI()
        {
            if (counter == 0)
                CEUtils.PlaySound("ksLand", 2f, Projectile.Center, 16, 0.2f);
            counter++;
            if (counter > 8)
            {
                NPC target = Projectile.FindMinionTarget();
                if (target != null)
                {
                    float homing = 0.4f;
                    if(CEUtils.getDistance(target.Center, Projectile.Center) < 360)
                    {
                        homing = Utils.Remap(CEUtils.getDistance(target.Center, Projectile.Center), 0, 360, 12, 0.4f);
                    }
                    Projectile.velocity += (target.Center - Projectile.Center).normalize() * homing * 4;
                    Projectile.velocity *= 1 - homing * 0.052f;
                }
            }
            odp.Add(Projectile.Center);
            if (odp.Count > 6)
            {
                odp.RemoveAt(0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var tex = CEUtils.getExtraTex("StarTexture_White");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Color color = Projectile.whoAmI % 2 == 0 ? new Color(190, 190, 80) : new Color(116, 200, 180);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, color * 0.8f, -Main.GlobalTimeWrappedHourly, tex.Size() / 2f, Projectile.scale * 0.2f * new Vector2(0.8f, 1f), SpriteEffects.None);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, color * 0.8f, 2 * Main.GlobalTimeWrappedHourly, tex.Size() / 2f, Projectile.scale * 0.2f * new Vector2(1.2f, 0.8f), SpriteEffects.None);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, color * 0.8f, 1.55f * Main.GlobalTimeWrappedHourly, tex.Size() / 2f, Projectile.scale * 0.2f * new Vector2(1, 0.8f), SpriteEffects.None);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, color * 0.8f, 1.55f * -Main.GlobalTimeWrappedHourly, tex.Size() / 2f, Projectile.scale * 0.2f * new Vector2(0.8f, 1f), SpriteEffects.None);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, color * 0.8f, -1.7f * Main.GlobalTimeWrappedHourly, tex.Size() / 2f, Projectile.scale * 0.2f * new Vector2(0.8f, 1.2f), SpriteEffects.None);
            drawT();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(target.Center, Projectile.velocity * 0.01f, Color.AliceBlue, new Vector2(0.7f, 1), Projectile.velocity.ToRotation(), 0.08f, 0.36f, 16));
        }

        public void drawT()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            odp.Add(Projectile.Center);
            if (odp.Count > 2)
            {
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    Color b = Projectile.whoAmI % 2 == 0 ? new Color(255, 255, 160) : new Color(160, 255, 220);

                    float a = 0;
                    float lr = 0;
                    for (int i = 1; i < odp.Count; i++)
                    {
                        a += 1f / (float)odp.Count;

                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 6 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 6 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                              b * a));
                        lr = (odp[i] - odp[i - 1]).ToRotation();
                    }
                    a = 1;

                    if (ve.Count >= 3)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MegaStreakBacking2").Value;
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    Color b = Color.White;

                    float a = 0;
                    float lr = 0;
                    for (int i = 1; i < odp.Count; i++)
                    {
                        a += 1f / (float)odp.Count;

                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 4 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 4 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                              b * a));
                        lr = (odp[i] - odp[i - 1]).ToRotation();
                    }
                    a = 1;

                    if (ve.Count >= 3)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1").Value;
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }

            }
            odp.RemoveAt(odp.Count - 1);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }

}