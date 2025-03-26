using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class OblivionArrow : ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public float angle;
        public float speed = 30;
        public bool htd = false;
        public float exps = 0;
        public Vector2 dscp = Vector2.Zero;
        float particlea = 1;
        float alpha = 1f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ArmorPenetration = 12;
            Projectile.extraUpdates = 1;
            Projectile.arrow = true;
            Projectile.ArmorPenetration = 120;
        }
        public int counter = 0;
        public bool std = false;
        public int homingTime = 60;
        public bool s = true;
        public override void AI()
        {
            if (s)
            {
                s = false;
                spawnrot = Projectile.velocity.ToRotation();
            }
            if (Projectile.timeLeft < 50)
            {
                alpha -= 0.02f;
                if (alpha < 0)
                {
                    alpha = 0;
                }
            }
            particlea *= 0.9f;
            counter++;
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
                odr.Add(Projectile.rotation - MathHelper.PiOver2);
                if (odp.Count > 64)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }

                NPC target = Projectile.FindTargetWithinRange(1600, false);
                if (target != null && Util.Util.getDistance(target.Center, Projectile.Center) < 200 && counter > 16)
                {
                    homingTime = 0;
                    Projectile.velocity *= 0.9f;
                    Vector2 v = target.Center - Projectile.Center;
                    v.Normalize();

                    Projectile.velocity += v * 1.5f;
                }
            }
            exps *= 0.9f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.velocity.Length() > 3)
            {
                Projectile.velocity *= 0.995f - homing * 0.018f;
            }
            if (counter > 40 && !htd)
            {
                if (homing < 6)
                {
                    homing += 0.014f;
                }
                NPC target = Projectile.FindTargetWithinRange(2600);

                if (target != null)
                {
                    if (Projectile.timeLeft < 60)
                    {
                        Projectile.timeLeft = 60;
                    }
                    Projectile.velocity += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * homing;
                }
            }
        }
        float homing = 0;
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
                Projectile.timeLeft = 20;
                htd = true;
                exps = 1;
                odp.Add(Projectile.Center);
                odr.Add(Projectile.rotation - MathHelper.PiOver2);
                odp.Add(Projectile.Center);
                odr.Add(Projectile.rotation - MathHelper.PiOver2);
            }
        }
        public int tofs;
        public Color TrailColor(float completionRatio)
        {
            Color result = new Color(30, 60, 160);
            return result * completionRatio * alpha;
        }

        public float TrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0, 46 * Projectile.scale, completionRatio);
        }
        float spawnrot = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            if (particlea > 0.02f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
                Texture2D tex = Util.Util.getExtraTex("obshot");

                Main.spriteBatch.Draw(tex, Projectile.owner.ToPlayer().Center - Main.screenPosition, null, Color.Blue, spawnrot, new Vector2(0, tex.Height / 2), 0.44f * new Vector2(2f - 2 * particlea, particlea), SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
            }
            drawT();
            return false;
        }

        public void drawT()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            var mp = this;
            if (mp.odp.Count > 1)
            {
                List<Vertex> ve = new List<Vertex>();
                Color b = new Color(16, 35, 85);

                float a = 0;
                float lr = 0;
                for (int i = 1; i < mp.odp.Count; i++)
                {
                    a += 1f / (float)mp.odp.Count;

                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 22,
                          new Vector3((float)(i + 1) / mp.odp.Count, 1, 1),
                        b * a));
                    ve.Add(new Vertex(mp.odp[i] - Main.screenPosition + (mp.odp[i] - mp.odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 22,
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
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth, TrailColor, (float _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            Main.spriteBatch.ExitShaderRegion();
            if (!htd)
            {
                Main.EntitySpriteDraw(value, position, null, base.Projectile.GetAlpha(Color.White) * alpha, Projectile.rotation, origin, base.Projectile.scale, SpriteEffects.None);
            }
        }

    }

}