using System.Collections.Generic;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class RuneArrow : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        float angle;
        float speed = 30;
        bool htd = false;
        float exps = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ArmorPenetration = 30;
        }

        public override void AI(){

            
            Projectile.ai[0]++;
            if (htd)
            {
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
                if (odp.Count > 16)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }
                
                NPC target = Projectile.FindTargetWithinRange(1100, false);
                if (target != null)
                {
                    Projectile.velocity *= 0.9f;   
                    Vector2 v = target.Center - Projectile.Center;
                    v.Normalize();
                    
                    Projectile.velocity += v * 2f;
                }
            }
            exps *= 0.9f;
            Projectile.rotation = Projectile.velocity.ToRotation();
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
                Projectile.timeLeft = 20;
                htd = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            odp.Add(Projectile.Center);
            odr.Add(Projectile.rotation);
            Color cl = Color.Lerp(Color.Black, Color.White, Projectile.ai[0] / 30f);
            float c = 0;

            
            c = 0;
            if (odp.Count > 1) {
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                List<Vertex> ve = new List<Vertex>();
                Color b = Color.White * 0.7f;
                ve.Add(new Vertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 10,
                          new Vector3((float)0, 1, 1),
                          b));
                ve.Add(new Vertex(odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 10,
                      new Vector3((float)0, 0, 1),
                      b));
                for (int i = 1; i < odp.Count; i++)
                {
                    

                    c += 1f / odp.Count;
                    ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 20,
                          new Vector3((float)i / odp.Count, 1, 1),
                          b));
                    ve.Add(new Vertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 20,
                          new Vector3((float)i / odp.Count, 0, 1),
                          b));
                        
                }

                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)                 {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/rvslash").Value;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
                if (!htd)
                {
                    Texture2D light = Util.Util.getExtraTex("lightball");
                    Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, Color.White * 0.4f, Projectile.rotation, light.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                if (htd)
                {
                    return false;

                }
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size()/2, Projectile.scale, SpriteEffects.None, 0);
                
            }
            odp.RemoveAt(odp.Count - 1);
            odr.RemoveAt(odr.Count - 1);
            return false;
        }

    }

}