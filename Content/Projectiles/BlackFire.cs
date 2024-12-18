using System;
using System.Collections.Generic;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class BlackFire : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 800;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ArmorPenetration = 30;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            if (odp.Count > 40)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }

            NPC target = Projectile.FindTargetWithinRange(1100, false);
            if (target != null)
            {
                Projectile.velocity *= 0.99f;
                Vector2 v = target.Center - Projectile.Center;
                v.Normalize();

                Projectile.velocity += v * 0.06f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        float trailOffset = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            Color cl = Color.Lerp(Color.Black, Color.White, Projectile.ai[0] / 30f);
            float c = 0;
            trailOffset += 0.04f;
            
            c = 0;
            if (odp.Count > 1) {
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                List<Vertex> ve = new List<Vertex>();
                Color b = Color.Black;

                for (int i = 1; i < odp.Count; i++)
                {
                    float width = 0;
                    if(i > 26)
                    {
                        float x = (float)(i - 26) / 13f;
                        width = (float)Math.Sqrt(1 - x * x);

                    }
                    else
                    {
                        width = 1 - ((float)(26 - i) / 26f);
                    }

                    c += 1f / odp.Count;
                    ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 34 * width,
                          new Vector3((float)i / odp.Count + trailOffset, 1, 1),
                          b));
                    ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 34 * width,
                          new Vector3((float)i / odp.Count + trailOffset, 0, 1),
                          b));
                        
                }

                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    Texture2D tx = TextureAssets.Projectile[Projectile.type].Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
 
            }
            odp.RemoveAt(odp.Count - 1);
            odr.RemoveAt(odr.Count - 1);
            return false;
        }

    }

}